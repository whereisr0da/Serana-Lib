/**
 * Serana - Copyright (c) 2018 - 2020 r0da [r0da@protonmail.ch]
 *
 * This work is licensed under the Creative Commons Attribution-NonCommercial-NoDerivatives 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by-nc-nd/4.0/ or send a letter to
 * Creative Commons, PO Box 1866, Mountain View, CA 94042, USA.
 *
 * By using Serana, you agree to the above license and its terms.
 *
 *      Attribution - You must give appropriate credit, provide a link to the license and indicate if changes were
 *                    made. You must do so in any reasonable manner, but not in any way that suggests the licensor
 *                    endorses you or your use.
 *
 *   Non-Commercial - You may not use the material (Serana) for commercial purposes.
 *
 *   No-Derivatives - If you remix, transform, or build upon the material (Serana), you may not distribute the
 *                    modified material. You are, however, allowed to submit the modified works back to the original
 *                    Serana project in attempt to have it added to the original project.
 *
 * You may not apply legal terms or technological measures that legally restrict others
 * from doing anything the license permits.
 *
 * No warranties are given.
 */

using Serana.Engine.Headers;
using Serana.Engine.Headers.Types;
using Serana.Engine.Streams;
using Serana.Engine.Section.Types;

using System;
using System.Collections.Generic;

namespace Serana.Engine.Section
{
    public class Sections
    {
        private Reader reader;

        private Header header;

        // simple proxy
        public int sectionHeaderBaseAddress;

        public readonly List<SectionEntry> sectionEntries;

        public List<Entry> entries;

        private bool isInMemory = false;

        /// <summary>
        /// Create Section collector from file
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="header"></param>
        public Sections(Reader reader, Header header)
        {
            this.reader = reader;
            this.header = header;

            this.sectionEntries = new List<SectionEntry>();

            this.entries = new List<Entry>();

            this.sectionHeaderBaseAddress = header.sectionHeaderBaseAddress;

            for (int i = 0; i < this.header.peHeader.NumberOfSection.getValue(); i++)
            {
                this.sectionEntries.Add(new SectionEntry(this.entries, this.reader, this.header));
            }
        }

        /// <summary>
        /// Create Section collector from memory
        /// </summary>
        /// <param name="header"></param>
        public Sections(Header header)
        {
            this.header = header;

            this.sectionEntries = new List<SectionEntry>();

            this.entries = new List<Entry>();

            this.sectionHeaderBaseAddress = header.sectionHeaderBaseAddress;

            this.isInMemory = true;
        }

        /// <summary>
        /// Get the section data start address
        /// NOTE : dynamic so it's a function
        /// </summary>
        /// <returns>The address of the sections data</returns>
        public int getSectionDataBaseAddress()
        {
            // there is 16 bytes of padding before section's data
            return this.sectionHeaderBaseAddress + exportHeaders().Count + 16;
        }

        /// <summary>
        /// Get the last section of the executable
        /// </summary>
        /// <returns>SectionEntry of the last section</returns>
        public SectionEntry getLastEntry()
        {
            return this.sectionEntries[this.header.peHeader.NumberOfSection.getValue() - 1];
        }

        /// <summary>
        /// Get the resource section if exists
        /// </summary>
        /// <returns>The resource section entry</returns>
        public SectionEntry getResourceSection()
        {
            int resourceDirVirtualAddress = this.header.dataDirectoryHeader.resourceDirectory.getVirtualAddress();
            int resourceDirSize = this.header.dataDirectoryHeader.resourceDirectory.getSize();

            if (resourceDirVirtualAddress == 0 || resourceDirSize == 0)
                return null;

            return getSectionFromVirtualAddress(resourceDirVirtualAddress);
        }

        /// <summary>
        /// Checks if a file offset is in the section
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool isFileOffsetInSection(int offset, SectionEntry section)
        {
            return offset >= section.header.pointerToRawData.getValue() && offset <= (section.header.pointerToRawData.getValue() + section.header.sizeOfRawData.getValue());
        }

        /// <summary>
        /// Checks if a memory offset is in the section
        /// NOTE : useless if ASLR
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool isVirtualAddressInSection(int offset, SectionEntry section)
        {
            return offset >= section.header.virtualAddress.getValue() && offset <= (section.header.virtualAddress.getValue() + section.header.virtualSize.getValue());
        }

        public SectionEntry getEntrypointSection()
        {
            return getSectionFromVirtualAddress(this.header.optionalHeader.AddressOfEntryPoint.getValue());
        }

        /// <summary>
        /// Get all code typed sections
        /// </summary>
        /// <returns>List of code typed section</returns>
        public List<SectionEntry> getCodeSections()
        {
            return this.sectionEntries.FindAll(delegate (SectionEntry s) { return s.type == SectionTypes.CODE_SECTION; });
        }

        /// <summary>
        /// Get all data typed sections
        /// </summary>
        /// <returns>List of data typed section</returns>
        public List<SectionEntry> getDataSections()
        {
            return this.sectionEntries.FindAll(delegate (SectionEntry s) { return s.type == SectionTypes.DATA_SECTION; });
        }

        /// <summary>
        /// Add a new section to executable
        /// NOTE : if is the first section the user has to handle the virtualAddress
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public SectionEntry addSection(string name, byte[] data, SectionTypes type)
        {
            // setup the virtual address of the section
            int virtualAddressBase = this.header.optionalHeader.BaseOfCode.getValue();

            // BaseOfData doesn't exist in x64
            if (this.header.is32Bit)
                virtualAddressBase = type == SectionTypes.CODE_SECTION ? this.header.optionalHeader.BaseOfCode.getValue() : this.header.optionalHeader.BaseOfData.getValue();

            // create the section
            SectionEntry section = new SectionEntry(this.entries, this.header);

            // set the name
            section.header.setName(name);

            bool isFirstSection = this.header.peHeader.NumberOfSection.getValue() < 1;

            // there is already a section
            if (!isFirstSection)
            {
                SectionEntry lastSection = this.getLastEntry();

                int newSectionAddress = lastSection.header.pointerToRawData.getValue() 
                    + lastSection.header.sizeOfRawData.getValue();

                // set the new section address
                section.header.pointerToRawData.setValue(newSectionAddress);
            }

            // the PE is empty
            else
            {
                // 16 bytes of padding before section's data
                int newSectionAddress = this.sectionHeaderBaseAddress + section.header.export().Count + 16;

                // set the new section address
                section.header.pointerToRawData.setValue(newSectionAddress);
            }

            // set the section size
            section.header.sizeOfRawData.setValue(data.Length);

            // zero if there is no relocation
            section.header.pointerToRelocations.setValue(0);
            section.header.numberOfRelocations.setValue(0);
            section.header.numberOfLinenumbers.setValue(0);

            // set the virtual address
            if (!isFirstSection)
            {
                SectionEntry lastSection = this.getLastEntry();

                int virtualAddress = calcVirtualAddress(lastSection.header.virtualAddress.getValue(), lastSection.header.virtualSize.getValue(), this.header.optionalHeader.SectionAlignment.getValue());

                section.header.virtualAddress.setValue(virtualAddress);
            }
            else
            {
                // set the virtualAddressBase to BaseOfCode or BaseOfData by default
                section.header.virtualAddress.setValue(virtualAddressBase);
            }

            // set the section virtual size as the same as raw size
            // not optimized but I can't find an accurate size without analysing the data
            section.header.virtualSize.setValue(data.Length);

            // set characteristics
            // TODO : fix uint

            // the section should be read by default
            section.header.characteristics += (int)SectionFlags.IMAGE_SCN_MEM_READ;

            // flag related to code sections
            if (type == SectionTypes.CODE_SECTION)
            {
                section.header.characteristics += (int)SectionFlags.IMAGE_SCN_CNT_CODE;
                section.header.characteristics += (int)SectionFlags.IMAGE_SCN_MEM_EXECUTE;
            }

            // flag related to "data" sections (not specificly ".data" sections)
            else
            {
                section.header.characteristics += (int)SectionFlags.IMAGE_SCN_CNT_INITIALIZED_DATA;
            }

            // set the type
            // TODO : fix order characteristics check (see how it handle .data dir)
            section.type = type;

            // set the data of the section
            section.rawData = data;

            // adding the section to the list
            this.sectionEntries.Add(section);

            // increase number of section
            this.header.peHeader.NumberOfSection += 1;

            // header fixing part
            updateHeader(type);

            // return the section object
            return section;
        }

        /// <summary>
        /// Update the headers when adding a section to the executable
        /// </summary>
        /// <param name="type">The section type</param>
        private void updateHeader(SectionTypes type)
        {
            int virtualAddressBase = this.header.optionalHeader.BaseOfCode.getValue();

            // BaseOfData doesn't exist in x64
            if (this.header.is32Bit)
            {
                // select the virtual address from section type
                virtualAddressBase = type == SectionTypes.CODE_SECTION ? this.header.optionalHeader.BaseOfCode.getValue() : this.header.optionalHeader.BaseOfData.getValue();
            }

            bool isFirstSection = this.header.peHeader.NumberOfSection.getValue() < 1;

            // fix PE header size in the header
            this.header.optionalHeader.SizeOfHeaders.setValue(getSectionDataBaseAddress());

            int sizeOfAllCodeSections = 0;
            int sizeOfAllDataSections = 0;

            // collects all raw section sizes
            getCodeSections().ForEach(s => sizeOfAllCodeSections += s.header.sizeOfRawData.getValue());
            getDataSections().ForEach(s => sizeOfAllDataSections += s.header.sizeOfRawData.getValue());

            // update all sections size in PE header
            this.header.optionalHeader.SizeOfCode.setValue(sizeOfAllCodeSections);
            this.header.optionalHeader.SizeOfInitializedData.setValue(sizeOfAllDataSections);

            // update the image size
            if (!isFirstSection)
            {
                // last section virtualAddress = size of virtual section's allocation segment
                // + padding due to the last section size

                int sizeOfImage = getLastEntry().header.virtualAddress.getValue() + getLastEntry().header.virtualSize.getValue();
                
                // update the image size
                this.header.optionalHeader.SizeOfImage.setValue(sizeOfImage);
            }
            else
            {
                // set the default image size
                this.header.optionalHeader.SizeOfImage.setValue(virtualAddressBase);
            }
        }

        /// <summary>
        /// Calculating the next section virtual address
        /// TODO : add padding
        /// </summary>
        /// <param name="lastVirtualAddress"></param>
        /// <param name="lastVirtualSize"></param>
        /// <param name="alignment"></param>
        /// <returns></returns>
        private int calcVirtualAddress(int lastVirtualAddress, int lastVirtualSize, int alignment)
        {
            /*

            List<SectionEntry> selectedSections = type == SectionTypes.CODE_SECTION ? getCodeSections() : getDataSections();
           
            SectionEntry lastSection = selectedSections[selectedSections.Count - 1];

            // by default the padding is 1000
            float paddingAmount = lastSection.header.sizeOfRawData.getValue() / 1000;

            int padding = this.header.optionalHeader.SectionAlignment.getValue();

            if (paddingAmount > 0)
            {
                padding = ((int)paddingAmount) * this.header.optionalHeader.SectionAlignment.getValue();
            }

            */

            // A better implementation
            return lastVirtualAddress + (((lastVirtualSize - 1) / alignment + 1) * alignment);
        }

        /*
        // I keep it for later
        // TODO : clean the code
        private int calcVirtualAddressDynamic(SectionTypes type)
        {
            List<SectionEntry> selectedSections = type == SectionTypes.CODE_SECTION ? getCodeSections() : getDataSections();

            SectionEntry lastSection = selectedSections[selectedSections.Count - 1];

            return lastSection.header.virtualAddress.getValue() + calcVirtualAddressPadding(type);
        }
        */

        /// <summary>
        /// Set the entrypoint in the executable
        /// </summary>
        /// <param name="codeSection"></param>
        /// <param name="entrypointOffsetInSection"></param>
        public void setEntrypoint(SectionEntry codeSection, int entrypointOffsetInSection)
        {
            if (codeSection.type != SectionTypes.CODE_SECTION)
                throw new Exception("Trying to set the entrypoint on a section that is not a code section");

            int entrypointVirtual = codeSection.header.virtualAddress.getValue() + entrypointOffsetInSection;

            // set the entrypoint
            this.header.optionalHeader.AddressOfEntryPoint.setValue(entrypointVirtual);
        }

        // TODO
        private void removeSection(int id)
        {
            this.sectionEntries.Remove(getSection(id));

            // section describe the header and data is grabbed from the object so
            // dont need to remove header

            // this.header.peHeader.NumberOfSection - 1
        }

        // TODO
        private void removeSection(SectionEntry section)
        {
            this.sectionEntries.Remove(section);

            // this.header.peHeader.NumberOfSection - 1
        }

        /// <summary>
        /// Know if a section is present by his name
        /// </summary>
        /// <param name="name">Name of the function</param>
        /// <returns>True if exist, otherwise false</returns>
        public bool isSectionPresent(string name)
        {
            return getSection(name) != null;
        }

        /// <summary>
        /// Get section object from name
        /// </summary>
        /// <param name="name">section name</param>
        /// <returns>the result section object, return null if not found</returns>
        public SectionEntry getSection(string name)
        {
            SectionEntry result = null;

            this.sectionEntries.ForEach(s => { if (s.header.name.ToString() == name) result = s; });

            return result;
        }

        /// <summary>
        /// Get section object from ordinal
        /// </summary>
        /// <param name="ordinal">section ordinal</param>
        /// <returns>the result section object, return null if not found</returns>
        public SectionEntry getSection(int ordinal)
        {
            SectionEntry result = null;

            this.sectionEntries.ForEach(s => { if (s.sectionId == ordinal) result = s; });

            return result;
        }

        /// <summary>
        /// Export all the data of the section block
        /// </summary>
        /// <returns>Array of byte that represent the sections</returns>
        public List<byte> exportSectionsData()
        {
            if (this.header.peHeader.NumberOfSection.getValue() < 1)
                throw new Exception("Trying to export sections without sections");

            List<byte> section = new List<byte>();

            int sectionHeaderSize = exportHeaders().Count;
             
            // 16 bytes before sections
            Utils.addArrayToList<byte>(section, new byte[16]);

            int sectionStartAddress = this.header.sectionHeaderBaseAddress + section.Count + sectionHeaderSize;

            SectionEntry lastSection = this.getLastEntry();

            int sectionBufferEndAddress = (int)lastSection.header.pointerToRawData.getValue() + (int)lastSection.header.sizeOfRawData.getValue();

            int sectionBufferSize = sectionBufferEndAddress - sectionStartAddress;

            // a buffer to write all sections intro it
            byte[] sectionBuffers = new byte[sectionBufferSize];

            // sections
            foreach (SectionEntry item in sectionEntries)
            {
                byte[] sectionBuffer = item.getSectionBuffer();

                // the raw address already respect the file alignment
                int rawAddress = item.header.pointerToRawData.getValue() - sectionStartAddress;

                // write section in the right address
                for (int i = 0; i < sectionBuffer.Length; i++)
                {
                    sectionBuffers[rawAddress + i] = sectionBuffer[i];
                }
            }

            // add the sections buffer
            Utils.addArrayToList<byte>(section, sectionBuffers);

            return section;
        }

        public List<byte> exportHeaders()
        {
            List<byte> sectionsHeaderBuffer = new List<byte>();

            foreach (Entry e in entries)
            {
                Utils.addArrayToList<byte>(sectionsHeaderBuffer, e.export());
            }

            return sectionsHeaderBuffer;
        }

        /// <summary>
        /// Get a section from a vitual address that is in the mapped virtual memory
        /// </summary>
        /// <param name="address">The virtual address</param>
        /// <returns>The section that contain this address, return null if not found</returns>
        public SectionEntry getSectionFromVirtualAddress(int address)
        {
            SectionEntry section = null;

            this.sectionEntries.ForEach(s => {

                if (address >= s.header.virtualAddress.getValue() &&
                    address < (s.header.virtualAddress.getValue() + s.header.virtualSize.getValue()))
                { section = s; }
            });

            return section;
        }

        /// <summary>
        /// Get a section from a file offset
        /// </summary>
        /// <param name="address">The file address</param>
        /// <returns>The section that contain this address, return null if not found</returns>
        public SectionEntry getSectionFromFileAddress(int address)
        {
            SectionEntry section = null;

            this.sectionEntries.ForEach(s => {

                if (address >= s.header.pointerToRawData.getValue() &&
                    address < (s.header.pointerToRawData.getValue() + s.header.sizeOfRawData.getValue()))
                { section = s; }
            });

            return section;
        }
    }
}
