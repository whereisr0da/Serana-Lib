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

using Serana.Engine.Streams;
using System.Collections.Generic;

namespace Serana.Engine.Headers
{
    /// <summary>
    /// Object that contain all headers of the PE
    /// </summary>
    public class Header
    {   
        private Reader reader;

        /// <summary>
        /// The DOS header
        /// Contain all DOS program informations including the DOS program
        /// </summary>
        public DOS_Header dosHeader;

        /// <summary>
        /// The PE header
        /// Contain 
        /// </summary>
        public PE_Header peHeader;

        /// <summary>
        /// The Optional Header
        /// </summary>
        public OptionalHeader optionalHeader;

        public DataDirectoryHeader dataDirectoryHeader;

        // simple proxies
        public readonly bool is32Bit;
        public readonly int sectionHeaderBaseAddress;

        /// <summary>
        /// Create a Header from file
        /// </summary>
        /// <param name="reader"></param>
        public Header(Reader reader)
        {
            this.reader = reader;

            this.dosHeader = new DOS_Header(this.reader);

            this.peHeader = new PE_Header(this.reader, this.dosHeader);

            this.optionalHeader = new OptionalHeader(this.reader, this.peHeader);

            this.dataDirectoryHeader = new DataDirectoryHeader(this.reader, this.optionalHeader);

            this.is32Bit = this.optionalHeader.is32Bit;

            this.sectionHeaderBaseAddress = this.dataDirectoryHeader.endOfHeader;
        }

        /// <summary>
        /// Create a Header from file
        /// </summary>
        /// <param name="baseOfCode"></param>
        /// <param name="baseOfData"></param>
        /// <param name="fileAlignment"></param>
        /// <param name="sectionAlignment"></param>
        public Header(int baseOfCode, int baseOfData, int fileAlignment, int sectionAlignment)
        {
            this.dosHeader = new DOS_Header();

            this.peHeader = new PE_Header(this.dosHeader);

            this.optionalHeader = new OptionalHeader(this.peHeader);

            this.dataDirectoryHeader = new DataDirectoryHeader(this.optionalHeader);

            // set base addresses
            this.optionalHeader.BaseOfCode.setValue(baseOfCode);
            this.optionalHeader.BaseOfData.setValue(baseOfData);

            // set the optional headers size in the header
            // section header padding is included (8)
            this.peHeader.SizeOfOptionalHeader.setValue(this.optionalHeader.export().Count + this.dataDirectoryHeader.export().Count);

            // set the default alignment
            this.optionalHeader.FileAlignment.setValue(fileAlignment);
            this.optionalHeader.SectionAlignment.setValue(sectionAlignment);

            // simple proxies
            this.is32Bit = this.optionalHeader.is32Bit;
            this.sectionHeaderBaseAddress = this.dataDirectoryHeader.endOfHeader;
        }

        public List<byte> export()
        {
            List<byte> headerBuffer = new List<byte>();

            // adding DOS header
            Utils.addArrayToList<byte>(headerBuffer, this.dosHeader.export().ToArray());

            // adding PE header
            Utils.addArrayToList<byte>(headerBuffer, this.peHeader.export().ToArray());

            // adding Optional header
            Utils.addArrayToList<byte>(headerBuffer, this.optionalHeader.export().ToArray());

            // adding Directory Header
            Utils.addArrayToList<byte>(headerBuffer, this.dataDirectoryHeader.export().ToArray());

            return headerBuffer;
        }
    }
}
