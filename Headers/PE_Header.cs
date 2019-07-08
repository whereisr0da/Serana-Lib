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
using Serana.Engine.Exceptions;

using System.Collections.Generic;
using System;

namespace Serana.Engine.Headers
{
    public class PE_Header
    {
        private Reader reader;

        private DOS_Header dosHeader;

        /// <summary>
        /// Number of section in PE
        /// </summary>
        public readonly NumericEntry NumberOfSection;

        /// <summary>
        /// TODO
        /// </summary>
        public readonly NumericEntry TimeDateStamp;

        /// <summary>
        /// Offset of symbols table
        /// </summary>
        public readonly NumericEntry PointerToSymbolTable;

        /// <summary>
        /// Number of symbols
        /// </summary>
        public readonly NumericEntry NumberOfSymbols;

        /// <summary>
        /// Size of optional header
        /// </summary>
        public readonly NumericEntry SizeOfOptionalHeader;

        /// <summary>
        /// TODO
        /// </summary>
        public readonly NumericEntry Characteristics;

        /// <summary>
        /// Architecture of the PE
        /// </summary>
        public TypeEntry<Machines> Architecture;

        /// <summary>
        /// PE header offset (simple proxy)
        /// </summary>
        public readonly NumericEntry PeHeaderOffset;

        public List<Entry> entries;

        public readonly int headerBaseAddress;

        public PE_Header(Reader reader, DOS_Header dosHeader)
        {
            this.reader = reader;

            this.entries = new List<Entry>();

            this.dosHeader = dosHeader;

            if (!isValidPeHeader())
            {
                throw new BadPeHeaderException();
            }

            this.headerBaseAddress = this.dosHeader.peHeaderOffset.getValue() + 0x4; // PE..

            // x32 and x64 don't change anything here

            this.Architecture = new TypeEntry<Machines>(entries, true, "Architecture", headerBaseAddress, EntrySize._16Bits);

            this.NumberOfSection = new NumericEntry(entries, true, "NumberOfSection", headerBaseAddress, EntrySize._16Bits);

            this.TimeDateStamp = new NumericEntry(entries, true, "TimeDateStamp", headerBaseAddress, EntrySize._32Bits);

            this.PointerToSymbolTable = new NumericEntry(entries, true, "PointerToSymbolTable", headerBaseAddress, EntrySize._32Bits);

            this.NumberOfSymbols = new NumericEntry(entries, true, "NumberOfSymbols", headerBaseAddress, EntrySize._32Bits);

            this.SizeOfOptionalHeader = new NumericEntry(entries, true, "SizeOfOptionalHeader", headerBaseAddress, EntrySize._16Bits);

            this.Characteristics = new NumericEntry(entries, true, "Characteristics", headerBaseAddress, EntrySize._16Bits);

            // simple proxy
            this.PeHeaderOffset = dosHeader.peHeaderOffset;

            // init values
            foreach (var item in entries)
            {
                item.readValue(this.reader);
            }
        }

        private bool isValidPeHeader()
        {
            // TODO : MORE CHECKS
            return Utils.bytesCompare(reader.readBytes(this.dosHeader.peHeaderOffset.getValue(), 4), HeaderSymbols.PE_HEADER);
        }

        public List<byte> export()
        {
            List<byte> headerBuffer = new List<byte>();

            // adding PE header
            Utils.addArrayToList<byte>(headerBuffer, HeaderSymbols.PE_HEADER);

            foreach (Entry item in entries)
            {
                Utils.addArrayToList<byte>(headerBuffer, item.export());
            }

            return headerBuffer;
        }
    }
}
