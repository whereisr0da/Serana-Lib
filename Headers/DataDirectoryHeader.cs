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
using System.Collections.Generic;

namespace Serana.Engine.Headers
{
    /// <summary>
    /// Object that contain all Data directory headers informations
    /// </summary>
    public class DataDirectoryHeader
    {
        private Reader reader;

        private OptionalHeader opHeader;

        public int headerBaseAddress;

        public DataDirectory exportDirectory;
        public DataDirectory importDirectory;
        public DataDirectory resourceDirectory;
        public DataDirectory exceptionDirectory;
        public DataDirectory securityDirectory;
        public DataDirectory baseRelocationDirectory;
        public DataDirectory debugDirectory;
        public DataDirectory architectureDirectory;
        public DataDirectory relativesAddressDirectory;
        public DataDirectory tlsDirectory;
        public DataDirectory loadConfigDirectory;
        public DataDirectory boundImportsDirectory;
        public DataDirectory importTableAddressDirectory;
        public DataDirectory delayLoadDescriptorDirectory;
        public DataDirectory netHeaderDirectory;

        public int endOfHeader;

        public List<Entry> entries;
        public List<DataDirectory> dirs;

        /// <summary>
        /// Create a Data directory header from file
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="opHeader">The optional header</param>
        public DataDirectoryHeader(Reader reader, OptionalHeader opHeader)
        {
            this.reader = reader;

            this.entries = new List<Entry>();
            this.dirs = new List<DataDirectory>();

            this.opHeader = opHeader;

            headerBaseAddress = this.opHeader.endOfHeader;

            setupStruct();

            ///
            /// init values
            /// 

            foreach (var item in entries)
            {
                item.readValue(this.reader);
            }

            int sizeTmp = headerBaseAddress;

            entries.ForEach(e => sizeTmp += (((int)e.getEntrySize()) / 8)*2);

            this.endOfHeader = sizeTmp + 0x8;
        }

        /// <summary>
        /// Create a Data directory header from memory
        /// </summary>
        /// <param name="opHeader">The optional header</param>
        public DataDirectoryHeader(OptionalHeader opHeader)
        {
            this.entries = new List<Entry>();
            this.dirs = new List<DataDirectory>();

            this.opHeader = opHeader;

            headerBaseAddress = this.opHeader.endOfHeader;

            setupStruct();

            ///
            /// init values
            /// 

            // exe as default, so export anything
            this.exportDirectory.setVirtualAddress(0);
            this.exportDirectory.setSize(0);

            // defined by the user
            this.importDirectory.setVirtualAddress(0);
            this.importDirectory.setSize(0);
            this.resourceDirectory.setVirtualAddress(0);
            this.resourceDirectory.setSize(0);
            this.exceptionDirectory.setVirtualAddress(0);
            this.exceptionDirectory.setSize(0);
            this.securityDirectory.setVirtualAddress(0);
            this.securityDirectory.setSize(0);
            this.baseRelocationDirectory.setVirtualAddress(0);
            this.baseRelocationDirectory.setSize(0);

            // debug info
            this.debugDirectory.setVirtualAddress(0);
            this.debugDirectory.setSize(0);

            this.architectureDirectory.setVirtualAddress(0);
            this.architectureDirectory.setSize(0);
            this.relativesAddressDirectory.setVirtualAddress(0);
            this.relativesAddressDirectory.setSize(0);
            this.tlsDirectory.setVirtualAddress(0);
            this.tlsDirectory.setSize(0);

            // null as default
            this.loadConfigDirectory.setVirtualAddress(0);
            this.loadConfigDirectory.setSize(0);
            this.boundImportsDirectory.setVirtualAddress(0);
            this.boundImportsDirectory.setSize(0);

            this.importTableAddressDirectory.setVirtualAddress(0);
            this.importTableAddressDirectory.setSize(0);

            // null as default
            this.delayLoadDescriptorDirectory.setVirtualAddress(0);
            this.delayLoadDescriptorDirectory.setSize(0);

            // defined by the user if is .NET executable
            this.netHeaderDirectory.setVirtualAddress(0);
            this.netHeaderDirectory.setSize(0);

            int sizeTmp = headerBaseAddress;

            entries.ForEach(e => sizeTmp += (((int)e.getEntrySize()) / 8) * 2);

            this.endOfHeader = sizeTmp + 0x8;
        }

        private void setupStruct()
        {
            // TODO : clear the code

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
