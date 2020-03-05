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
using System;
using System.Collections.Generic;

namespace Serana.Engine.Section
{
    public class SectionHeader
    {
        public DataEntry name;

        public NumericEntry virtualSize;
        public NumericEntry virtualAddress;

        public NumericEntry sizeOfRawData;
        public NumericEntry pointerToRawData;

        public NumericEntry pointerToRelocations;
        public NumericEntry pointerToLineNumbers;

        public NumericEntry numberOfRelocations;
        public NumericEntry numberOfLinenumbers;
        public NumericEntry characteristics;

        public List<Entry> entries;

        private Reader reader;
        private Header mainHeader;

        /// <summary>
        /// Create a section header from file
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="reader"></param>
        /// <param name="mainHeader"></param>
        public SectionHeader(List<Entry> entries, Reader reader, Header mainHeader)
        {
            this.reader = reader;
            this.mainHeader = mainHeader;
            this.entries = entries;

            this.name = new DataEntry(entries, this.mainHeader.is32Bit, "name", this.mainHeader.sectionHeaderBaseAddress, SectionSymbols.SECTION_NAME_SIZE, EntrySize._8Bits);

            this.virtualSize = processSectionValue("virtualSize");
            this.virtualAddress = processSectionValue("virtualAddress");
            this.sizeOfRawData = processSectionValue("sizeOfRawData");
            this.pointerToRawData = processSectionValue("pointerToRawData");
            this.pointerToRelocations = processSectionValue("pointerToRelocations");

            // TODO : understand where this is used
            //this.pointerToLineNumbers = processSectionValue("pointerToLineNumbers");

            this.numberOfRelocations = processSectionValue("numberOfRelocations");
            this.numberOfLinenumbers = processSectionValue("numberOfLinenumbers");
            this.characteristics = processSectionValue("characteristics");

            foreach (var item in entries)
            {
                item.readValue(this.reader);
            }
        }

        /// <summary>
        /// Create a section header from memory
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="mainHeader"></param>
        public SectionHeader(List<Entry> entries, Header mainHeader)
        {
            this.mainHeader = mainHeader;
            this.entries = entries;

            this.name = new DataEntry(entries, this.mainHeader.is32Bit, "name", this.mainHeader.sectionHeaderBaseAddress, SectionSymbols.SECTION_NAME_SIZE, EntrySize._8Bits);

            this.virtualSize = processSectionValue("virtualSize");
            this.virtualAddress = processSectionValue("virtualAddress");

            this.sizeOfRawData = processSectionValue("sizeOfRawData");

            this.pointerToRawData = processSectionValue("pointerToRawData");
            this.pointerToRelocations = processSectionValue("pointerToRelocations");

            //this.pointerToLineNumbers = processSectionValue("pointerToLineNumbers");

            this.numberOfRelocations = processSectionValue("numberOfRelocations");
            this.numberOfLinenumbers = processSectionValue("numberOfLinenumbers");

            this.characteristics = processSectionValue("characteristics");
        }

        /// <summary>
        /// Set the section's name
        /// </summary>
        /// <param name="name"></param>
        public void setName(string name)
        {
            int length = SectionSymbols.SECTION_NAME_SIZE;

            if (name.Length < length)
                length = name.Length;

            byte[] nameBuffer = new byte[SectionSymbols.SECTION_NAME_SIZE];

            for (int i = 0; i < length; i++)
            {
                nameBuffer[i] = (byte)name.ToCharArray()[i];
            }

            this.name.setValue(nameBuffer);
        }

        private NumericEntry processSectionValue(string name)
        {
            return new NumericEntry(entries, this.mainHeader.is32Bit, name, this.mainHeader.sectionHeaderBaseAddress, EntrySize._32Bits);
        }

        public List<byte> export()
        {
            List<byte> headerBuffer = new List<byte>();

            foreach (Entry item in entries)
            {
                Utils.addArrayToList<byte>(headerBuffer, item.export());
            }

            return headerBuffer;
        }
    }
}
