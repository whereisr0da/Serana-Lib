﻿/**
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

using Serana.Engine.Streams;
using System.Collections.Generic;

namespace Serana.Engine.Headers
{
    public class Header
    {
        private Reader reader;

        public DOS_Header dosHeader;

        public PE_Header peHeader;

        public OptionalHeader optionalHeader;

        public DataDirectoryHeader dataDirectoryHeader;

        // simple proxy
        public readonly bool is32Bit;

        public Header(Reader reader)
        {
            this.reader = reader;

            this.dosHeader = new DOS_Header(this.reader);

            this.peHeader = new PE_Header(this.reader, this.dosHeader);

            this.optionalHeader = new OptionalHeader(this.reader, this.peHeader);

            this.dataDirectoryHeader = new DataDirectoryHeader(this.reader, this.optionalHeader);

            this.is32Bit = this.optionalHeader.is32Bit;
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
            
            // adding Optional header
            Utils.addArrayToList<byte>(headerBuffer, this.dataDirectoryHeader.export().ToArray());

            return headerBuffer;
        }
    }
}
