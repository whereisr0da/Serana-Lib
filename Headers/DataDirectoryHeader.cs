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

using Serana.Engine.Headers.Types;
using Serana.Engine.Streams;
using System.Collections.Generic;

namespace Serana.Engine.Headers
{
    public class DataDirectoryHeader
    {
        private Reader reader;

        private OptionalHeader opHeader;

        public readonly int headerBaseAddress;

        public readonly DataDirectory exportDirectory;
        public readonly DataDirectory importDirectory;
        public readonly DataDirectory resourceDirectory;
        public readonly DataDirectory exceptionDirectory;
        public readonly DataDirectory securityDirectory;
        public readonly DataDirectory baseRelocationDirectory;
        public readonly DataDirectory debugDirectory;
        public readonly DataDirectory architectureDirectory;
        public readonly DataDirectory relativesAddressDirectory;
        public readonly DataDirectory tlsDirectory;
        public readonly DataDirectory loadConfigDirectory;
        public readonly DataDirectory boundImportsDirectory;
        public readonly DataDirectory importTableAddressDirectory;
        public readonly DataDirectory delayLoadDescriptorDirectory;
        public readonly DataDirectory netHeaderDirectory;

        public readonly int endOfHeader;

        public List<Entry> entries;
        public List<DataDirectory> dirs;

        public DataDirectoryHeader(Reader reader, OptionalHeader opHeader)
        {
            this.reader = reader;

            this.entries = new List<Entry>();
            this.dirs = new List<DataDirectory>();

            this.opHeader = opHeader;

            headerBaseAddress = this.opHeader.endOfHeader;

            // 16 dirs in PE files

            // we don't care about x32

            this.exportDirectory = new DataDirectory(dirs, new DataEntry(entries, true, "exportDirectory", headerBaseAddress, 2, EntrySize._32Bits));
            this.importDirectory = new DataDirectory(dirs, new DataEntry(entries, true, "importDirectory", headerBaseAddress, 2, EntrySize._32Bits));
            this.resourceDirectory = new DataDirectory(dirs, new DataEntry(entries, true, "resourceDirectory", headerBaseAddress, 2, EntrySize._32Bits));
            this.exceptionDirectory = new DataDirectory(dirs, new DataEntry(entries, true, "exceptionDirectory", headerBaseAddress, 2, EntrySize._32Bits));
            this.securityDirectory = new DataDirectory(dirs, new DataEntry(entries, true, "securityDirectory", headerBaseAddress, 2, EntrySize._32Bits));
            this.baseRelocationDirectory = new DataDirectory(dirs, new DataEntry(entries, true, "baseRelocationDirectory", headerBaseAddress, 2, EntrySize._32Bits));
            this.debugDirectory = new DataDirectory(dirs, new DataEntry(entries, true, "debugDirectory", headerBaseAddress, 2, EntrySize._32Bits));
            this.architectureDirectory = new DataDirectory(dirs, new DataEntry(entries, true, "architectureDirectory", headerBaseAddress, 2, EntrySize._32Bits));
            this.relativesAddressDirectory = new DataDirectory(dirs, new DataEntry(entries, true, "relativesAddressDirectory", headerBaseAddress, 2, EntrySize._32Bits));
            this.tlsDirectory = new DataDirectory(dirs, new DataEntry(entries, true, "tlsDirectory", headerBaseAddress, 2, EntrySize._32Bits));
            this.loadConfigDirectory = new DataDirectory(dirs, new DataEntry(entries, true, "loadConfigDirectory", headerBaseAddress, 2, EntrySize._32Bits));
            this.boundImportsDirectory = new DataDirectory(dirs, new DataEntry(entries, true, "boundImportsDirectory", headerBaseAddress, 2, EntrySize._32Bits));
            this.importTableAddressDirectory = new DataDirectory(dirs, new DataEntry(entries, true, "importTableAddressDirectory", headerBaseAddress, 2, EntrySize._32Bits));
            this.delayLoadDescriptorDirectory = new DataDirectory(dirs, new DataEntry(entries, true, "delayLoadDescriptorDirectory", headerBaseAddress, 2, EntrySize._32Bits));
            this.netHeaderDirectory = new DataDirectory(dirs, new DataEntry(entries, true, "netHeaderDirectory", headerBaseAddress, 2, EntrySize._32Bits));

            // init values
            foreach (var item in entries)
            {
                item.readValue(this.reader);
            }

            int sizeTmp = headerBaseAddress;

            entries.ForEach(e => sizeTmp += (((int)e.getEntrySize()) / 8)*2);

            this.endOfHeader = sizeTmp + 0x8;
        }

        public List<byte> export()
        {
            List<byte> headerBuffer = new List<byte>();

            foreach (Entry item in entries)
            {
                Utils.addArrayToList<byte>(headerBuffer, item.export());
            }

            // 8 bytes before section headers
            Utils.addArrayToList<byte>(headerBuffer, new byte[8]);

            return headerBuffer;
        }
    }
}
