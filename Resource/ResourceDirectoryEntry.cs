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
using Serana.Engine.Resource.Types;
using Serana.Engine.Streams;

using System;
using System.Collections.Generic;
using System.Text;

namespace Serana.Engine.Resource
{
    public class ResourceDirectoryEntry
    {
        private Reader reader;

        private Resources resources;

        public NumericEntry Name;

        public NumericEntry OffsetToData;

        public List<ResourceDirectoryTable> resourceTables;

        public ResourceDataEntry dataEntry;

        public bool isNamedDirectory = false;

        public bool firstNode = false;

        public ResourceTypes directoryType;

        public int directoryId;

        public string directoryName = "";

        private List<Entry> localEntries;

        private List<Entry> entries;

        public ResourceDirectoryEntry(Resources resources, Reader reader, List<Entry> entries, ref int offset)
        {
            this.reader = reader;
            this.resources = resources;
            this.entries = entries;
            this.localEntries = new List<Entry>();
            this.resourceTables = new List<ResourceDirectoryTable>();

            this.dataEntry = null;

            this.Name = new NumericEntry(null, true, "Name", offset, EntrySize._32Bits);

            offset += (int)this.Name.getRawSize();

            this.OffsetToData = new NumericEntry(null, true, "OffsetToData", offset, EntrySize._32Bits);

            offset += (int)this.OffsetToData.getRawSize();

            this.Name.readValue(this.reader);
            this.localEntries.Add(this.Name);

            this.OffsetToData.readValue(this.reader);
            this.localEntries.Add(this.OffsetToData);
        }

        public void readInformations(ref int offset)
        {
            UInt32 nameOffset = (UInt32)this.Name.getValue();

            // the directory is named
            if (nameOffset >= 0x80000000)
            {
                isNamedDirectory = true;

                // insure the size
                UInt32 nameDefinitionBlock = (nameOffset - (UInt32)0x80000000);

                int nameBlockOffset = (int)nameDefinitionBlock + resources.resourceBaseAddress;

                // read the name length
                int unicodeStringSize = reader.readByte(nameBlockOffset);

                // read the unicode name 
                directoryName = reader.readUnicodeString(nameBlockOffset + 1, unicodeStringSize);
            }

            // the directory has a common id
            else 
            {
                // the directory id is typed
                if (firstNode)
                {
                    // don't know how I can handle properly exceptions here
                    directoryType = (ResourceTypes)Enum.Parse(typeof(ResourceTypes), Enum.GetName(typeof(ResourceTypes), (int)nameOffset), true);
                    directoryId = (int)nameOffset;
                }

                // the directory id is an ordinal
                else
                {
                    directoryId = (int)nameOffset;
                }
            }

            UInt32 tableOffset = (UInt32)this.OffsetToData.getValue();

            if (tableOffset >= 0x80000000)
            {
                // insure the size
                UInt32 nextResourceDirectoryTable = (tableOffset - (UInt32)0x80000000);

                // probably the wrost way to do it
                offset = (int)nextResourceDirectoryTable + resources.resourceBaseAddress;

                ResourceDirectoryTable newEntry = new ResourceDirectoryTable(this.resources, this.reader, entries, ref offset);

                this.resourceTables.Add(newEntry);
            }
            else
            {
                // probably the wrost way to do it
                offset = (int)tableOffset + resources.resourceBaseAddress;

                this.dataEntry = new ResourceDataEntry(this.resources, this.reader, entries, ref offset);
            }
        }

        public int rawSize()
        {
            int result = 0;

            foreach (Entry e in this.localEntries)
                result += e.getSize();

            return result;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            string directoryNameString = "";

            if (isNamedDirectory)
                directoryNameString = "(" + directoryName + ")";
            else if (firstNode)
                directoryNameString = "(" + Enum.GetName(typeof(ResourceTypes), directoryType) + ")";

            stringBuilder.Append(String.Format("0x{0} Name 0x{1} {2}{3}", Name.getOffset().ToString("X"), Name.getValue().ToString("X"), directoryNameString, Environment.NewLine));
            stringBuilder.Append(String.Format("0x{0} OffsetToData 0x{1}{2}", OffsetToData.getOffset().ToString("X"), OffsetToData.getValue().ToString("X"), ""));

            return stringBuilder.ToString();
        }
    }
}
