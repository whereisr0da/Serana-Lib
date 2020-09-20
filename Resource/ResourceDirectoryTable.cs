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

using Serana.Engine.Headers.Types;
using Serana.Engine.Streams;

using System;
using System.Collections.Generic;
using System.Text;

namespace Serana.Engine.Resource
{
    public class ResourceDirectoryTable
    {
        private Reader reader;

        private Resources resources;

        public NumericEntry Characteristics;

        public NumericEntry TimeDateStamp;

        public NumericEntry MajorVersion;

        public NumericEntry MinorVersion;

        public NumericEntry NumberOfNamedEntries;

        public NumericEntry NumberOfIdEntries;

        public List<ResourceDirectoryEntry> resourceEntries;

        private List<Entry> entries;

        private bool isInMemory = false;

        public ResourceDirectoryTable(Resources resources, Reader reader, List<Entry> entries, ref int offset)
        {
            this.reader = reader;
            this.resources = resources;
            this.entries = entries;

            //this.entries = new List<Entry>();
            this.resourceEntries = new List<ResourceDirectoryEntry>();

            setupStruct(ref offset);

            this.Characteristics.readValue(this.reader);
            this.TimeDateStamp.readValue(this.reader);
            this.MajorVersion.readValue(this.reader);
            this.MinorVersion.readValue(this.reader);
            this.NumberOfNamedEntries.readValue(this.reader);
            this.NumberOfIdEntries.readValue(this.reader);
        }

        public void readEntries(ref int offset, bool firstTime)
        {
            // read named entries + id entries
            for (int i = 0; i < this.NumberOfNamedEntries.getValue() + this.NumberOfIdEntries.getValue(); i++)
            {
                var newEntry = new ResourceDirectoryEntry(this.resources, this.reader, entries, ref offset);

                if (firstTime)
                    newEntry.firstNode = true;

                this.resourceEntries.Add(newEntry);
            }

            // read sub branchs
            foreach (ResourceDirectoryEntry entry in resourceEntries)
            {
                entry.readInformations(ref offset);

                foreach (ResourceDirectoryTable table in entry.resourceTables)
                {
                    table.readEntries(ref offset, false);
                }
            } 
        }

        private void setupStruct(ref int offset)
        {
            // TODO : clear the code

            this.Characteristics = new NumericEntry(null, true, "Characteristics", offset, EntrySize._32Bits);
            offset += this.Characteristics.getRawSize();

            this.TimeDateStamp = new NumericEntry(null, true, "TimeDateStamp", offset, EntrySize._32Bits);
            offset += (int)this.TimeDateStamp.getRawSize();

            this.MajorVersion = new NumericEntry(null, true, "MajorVersion", offset, EntrySize._16Bits);
            offset += (int)this.MajorVersion.getRawSize();

            this.MinorVersion = new NumericEntry(null, true, "MinorVersion", offset, EntrySize._16Bits);
            offset += (int)this.MinorVersion.getRawSize();

            this.NumberOfNamedEntries = new NumericEntry(null, true, "NumberOfNamedEntries", offset, EntrySize._16Bits);
            offset += (int)this.NumberOfNamedEntries.getRawSize();

            this.NumberOfIdEntries = new NumericEntry(null, true, "NumberOfIdEntries", offset, EntrySize._16Bits);
            offset += (int)this.NumberOfIdEntries.getRawSize();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(String.Format("0x{0} Characteristics 0x{1}{2}", Characteristics.getOffset().ToString("X"), Characteristics.getValue().ToString("X"), Environment.NewLine));
            stringBuilder.Append(String.Format("0x{0} TimeDateStamp 0x{1}{2}", TimeDateStamp.getOffset().ToString("X"), TimeDateStamp.getValue().ToString("X"), Environment.NewLine));
            stringBuilder.Append(String.Format("0x{0} MajorVersion 0x{1}{2}", MajorVersion.getOffset().ToString("X"), MajorVersion.getValue().ToString("X"), Environment.NewLine));
            stringBuilder.Append(String.Format("0x{0} MinorVersion 0x{1}{2}", MinorVersion.getOffset().ToString("X"), MinorVersion.getValue().ToString("X"), Environment.NewLine));
            stringBuilder.Append(String.Format("0x{0} NumberOfNamedEntries 0x{1}{2}", NumberOfNamedEntries.getOffset().ToString("X"), NumberOfNamedEntries.getValue().ToString("X"), Environment.NewLine));
            stringBuilder.Append(String.Format("0x{0} NumberOfIdEntries 0x{1}{2}", NumberOfIdEntries.getOffset().ToString("X"), NumberOfIdEntries.getValue().ToString("X"), ""));

            return stringBuilder.ToString();
        }
    }
}
