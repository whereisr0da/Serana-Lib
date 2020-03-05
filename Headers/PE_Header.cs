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
using Serana.Engine.Exceptions;

using System.Collections.Generic;
using System;

namespace Serana.Engine.Headers
{
    /// <summary>
    /// Object that contain all PE header informations
    /// </summary>
    public class PE_Header
    {
        private Reader reader;

        private DOS_Header dosHeader;

        /// <summary>
        /// Number of section in PE
        /// </summary>
        public NumericEntry NumberOfSection;

        /// <summary>
        /// TODO
        /// </summary>
        public NumericEntry TimeDateStamp;

        /// <summary>
        /// Offset of symbols table
        /// </summary>
        public NumericEntry PointerToSymbolTable;

        /// <summary>
        /// Number of symbols
        /// </summary>
        public NumericEntry NumberOfSymbols;

        /// <summary>
        /// Size of optional header
        /// </summary>
        public NumericEntry SizeOfOptionalHeader;

        /// <summary>
        /// TODO
        /// </summary>
        public NumericEntry Characteristics;

        /// <summary>
        /// Architecture of the PE
        /// </summary>
        public TypeEntry<Machines> Architecture;

        /// <summary>
        /// PE header offset (simple proxy)
        /// </summary>
        public NumericEntry PeHeaderOffset;

        public List<Entry> entries;

        public int headerBaseAddress;

        /// <summary>
        /// Create a PE header from file
        /// </summary>
        /// <param name="reader">the reader</param>
        /// <param name="dosHeader">the previous DOS header</param>
        public PE_Header(Reader reader, DOS_Header dosHeader)
        {
            this.reader = reader;

            this.entries = new List<Entry>();

            this.dosHeader = dosHeader;

            if (!isValidPeHeader())
            {
                throw new BadPeHeaderException();
            }

            this.headerBaseAddress = this.dosHeader.peHeaderOffset.getValue() + HeaderSymbols.PE_HEADER.Length; // PE..

            setupStruct();

            // simple proxy
            this.PeHeaderOffset = dosHeader.peHeaderOffset;

            ///
            /// init values
            /// 

            foreach (var item in entries)
            {
                item.readValue(this.reader);
            }
        }

        /// <summary>
        /// Create a PE header from memory
        /// </summary>
        /// <param name="dosHeader">the previous DOS header</param>
        public PE_Header(DOS_Header dosHeader)
        {
            ///
            /// init header
            /// 

            this.entries = new List<Entry>();

            this.dosHeader = dosHeader;

            this.headerBaseAddress = this.dosHeader.peHeaderOffset.getValue() + HeaderSymbols.PE_HEADER.Length; // PE..

            setupStruct();

            // simple proxy
            this.PeHeaderOffset = dosHeader.peHeaderOffset;

            ///
            /// init values
            /// 

            // x32 bit as default
            this.Architecture.setValue(Machines.INTEL386);

            // new PE objects are empty
            this.NumberOfSection.setValue(0);

            // indicates when the file was created
            // TODO
            this.TimeDateStamp.setValue(0);

            // new PE objects don't have symbols
            this.PointerToSymbolTable.setValue(0);

            // new PE objects don't have symbols
            this.NumberOfSymbols.setValue(0);

            // will be written after creating it
            this.SizeOfOptionalHeader.setValue(0);

            int characteristics = (int)Types.Characteristics.IMAGE_FILE_EXECUTABLE_IMAGE + (int)Types.Characteristics.IMAGE_FILE_LARGE_ADDRESS_AWARE;

            // general characteristics
            this.Characteristics.setValue(characteristics);
        }

        private void setupStruct()
        {
            // TODO : clear the code

            // x32 and x64 don't change anything here

            this.Architecture = new TypeEntry<Machines>(entries, true, "Architecture", headerBaseAddress, EntrySize._16Bits);
            this.NumberOfSection = new NumericEntry(entries, true, "NumberOfSection", headerBaseAddress, EntrySize._16Bits);
            this.TimeDateStamp = new NumericEntry(entries, true, "TimeDateStamp", headerBaseAddress, EntrySize._32Bits);
            this.PointerToSymbolTable = new NumericEntry(entries, true, "PointerToSymbolTable", headerBaseAddress, EntrySize._32Bits);
            this.NumberOfSymbols = new NumericEntry(entries, true, "NumberOfSymbols", headerBaseAddress, EntrySize._32Bits);
            this.SizeOfOptionalHeader = new NumericEntry(entries, true, "SizeOfOptionalHeader", headerBaseAddress, EntrySize._16Bits);
            this.Characteristics = new NumericEntry(entries, true, "Characteristics", headerBaseAddress, EntrySize._16Bits);
        }

        /// <summary>
        /// Check if the PE header is valid
        /// </summary>
        /// <returns>True if the PE header is valid</returns>
        private bool isValidPeHeader()
        {
            // TODO : MORE CHECKS
            return Utils.bytesCompare(reader.readBytes(this.dosHeader.peHeaderOffset.getValue(), 4), HeaderSymbols.PE_HEADER);
        }

        /// <summary>
        /// Export all header items to raw bytes
        /// </summary>
        /// <returns>Array of byte representing the raw PE header</returns>
        public List<byte> export()
        {
            List<byte> headerBuffer = new List<byte>();

            // adding PE header
            Utils.addArrayToList<byte>(headerBuffer, HeaderSymbols.PE_HEADER);

            // adding all items
            foreach (Entry item in entries)
            {
                Utils.addArrayToList<byte>(headerBuffer, item.export());
            }

            return headerBuffer;
        }
    }
}
