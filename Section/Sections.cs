/**
 * Serana - Copyright (c) 2018 - 2019 r0da [r0da@protonmail.ch]
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serana.Engine.Section
{
    public class Sections
    {
        private Reader reader;

        private Header header;

        public readonly int sectionHeaderBaseAddress;

        public readonly int sectionCount;

        public readonly List<SectionEntry> sectionEntries;

        public List<Entry> entries;

        public Sections(Reader reader, Header header)
        {
            this.reader = reader;
            this.header = header;

            this.sectionEntries = new List<SectionEntry>();

            this.entries = new List<Entry>();

            this.sectionHeaderBaseAddress = header.dataDirectoryHeader.endOfHeader;

            this.sectionCount = header.peHeader.NumberOfSection.getValue();

            for (int i = 0; i < this.sectionCount; i++)
            {
                sectionEntries.Add(processSection());
            }

            foreach (var item in entries)
            {
                item.readValue(this.reader);
            }
        }

        private SectionEntry processSection()
        {
            DataEntry name = new DataEntry(entries, this.header.is32Bit, "name", this.sectionHeaderBaseAddress, SectionSymbols.SECTION_NAME_SIZE, EntrySize._8Bits);

            NumericEntry virtualSize = new NumericEntry(entries, this.header.is32Bit, "virtualSize", this.sectionHeaderBaseAddress, EntrySize._32Bits);

            NumericEntry virtualAddress = new NumericEntry(entries, this.header.is32Bit, "virtualAddress", this.sectionHeaderBaseAddress, EntrySize._32Bits);

            NumericEntry sizeOfRawData = new NumericEntry(entries, this.header.is32Bit, "sizeOfRawData", this.sectionHeaderBaseAddress, EntrySize._32Bits);

            NumericEntry pointerToRawData = new NumericEntry(entries, this.header.is32Bit, "pointerToRawData", this.sectionHeaderBaseAddress, EntrySize._32Bits);

            NumericEntry pointerToRelocations = new NumericEntry(entries, this.header.is32Bit, "pointerToRelocations", this.sectionHeaderBaseAddress, EntrySize._32Bits);

            NumericEntry pointerToLineNumbers = new NumericEntry(entries, this.header.is32Bit, "pointerToLineNumbers", this.sectionHeaderBaseAddress, EntrySize._32Bits);

            NumericEntry numberOfRelocations = new NumericEntry(entries, this.header.is32Bit, "numberOfRelocations", this.sectionHeaderBaseAddress, EntrySize._16Bits);

            NumericEntry numberOfLinenumbers = new NumericEntry(entries, this.header.is32Bit, "numberOfLinenumbers", this.sectionHeaderBaseAddress, EntrySize._16Bits);

            NumericEntry characteristics = new NumericEntry(entries, this.header.is32Bit, "characteristics", this.sectionHeaderBaseAddress, EntrySize._32Bits);

            return new SectionEntry(reader, name, virtualSize, virtualAddress, sizeOfRawData, pointerToRawData, pointerToRelocations,
                pointerToLineNumbers, numberOfRelocations, numberOfLinenumbers, characteristics);
        }

        // TODO
        private void addSection(byte[] data)
        {

        }

        // TODO
        private void removeSection(int id)
        {
            this.sectionEntries.Remove(getSectionFromId(id));

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

        public SectionEntry getSectionFromId(int id)
        {
            SectionEntry result = null;

            this.sectionEntries.ForEach(s => { if (s.sectionId == id) result = s; });

            return result;
        }

        public List<byte> export(int sectionHeaderOffset)
        {
            List<byte> section= new List<byte>();
            
            // sections headers
            foreach (Entry item in entries)
            {
                Utils.addArrayToList<byte>(section, item.export());
            }

            // 16 bytes before sections
            Utils.addArrayToList<byte>(section, new byte[16]);

            int sectionStartAddress = sectionHeaderOffset + section.Count;

            SectionEntry lastSection = this.sectionEntries[this.sectionCount - 1];

            int sectionBufferEndAddress = (int)lastSection.pointerToRawData.getValue() + (int)lastSection.sizeOfRawData.getValue();

            int sectionBufferSize = sectionBufferEndAddress - sectionStartAddress;

            // a buffer to write all sections intro it
            byte[] sectionBuffers = new byte[sectionBufferSize];

            // sections
            foreach (SectionEntry item in sectionEntries)
            {
                byte[] sectionBuffer = item.getSectionBuffer();

                int rawAddress = item.pointerToRawData.getValue() - sectionStartAddress;

                // write section in the right address
                for (int i = 0; i < sectionBuffer.Length; i++)
                {
                    sectionBuffers[rawAddress + i] = sectionBuffer[i];
                }
            }

            // add the section buffer
            Utils.addArrayToList<byte>(section, sectionBuffers);

            return section;
        }
    }
}
