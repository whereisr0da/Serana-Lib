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
using Serana.Engine.Resource;
using Serana.Engine.Section;
using Serana.Engine.Streams;
using Serana.Engine.Exceptions;

using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;

namespace Serana.Engine
{
    public class PE
    {
        public readonly Header header;

        public readonly Resources resources;

        public readonly Sections sections;

        private Reader reader;

        public readonly string fileName;

        // TODO : BIG FILES
        public readonly int endOfFile;

        public readonly bool EOFOverflow = false;

        // TODO : BIG FILES
        public readonly int overflowSize = 0;

        public PE(string filePath)
        { 
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Fail to open the executable file");

            this.fileName = new FileInfo(filePath).Name;

            this.reader = new Reader(filePath);

            this.header = new Header(this.reader);

            this.sections = new Sections(this.reader, this.header);

            SectionEntry lastEntry = this.sections.sectionEntries[this.sections.sectionCount - 1];

            this.endOfFile = lastEntry.pointerToRawData.getValue() + lastEntry.sizeOfRawData.getValue();

            this.EOFOverflow = this.endOfFile != (int)this.reader.size();

            if (this.EOFOverflow)
                overflowSize = (int)this.reader.size() - this.endOfFile;
        }

        private bool isASLR()
        {
            // TODO : Make it work because it's not the right way to detect it :D
            // TODO : TypeEntry<DllCharacteristics>

            int part1 = (((this.header.optionalHeader.DLLCharacteristics.getValue() / 16) % 16) << 4);

            //if (part1 == (int)DllCharacteristics.DYNAMIC_BASE)
            //    return true;

            return false;
        }

        public List<byte> export()
        {
            List<byte> peBuffer = new List<byte>();

            // adding Header
            Utils.addArrayToList<byte>(peBuffer, this.header.export().ToArray());

            // adding Sections
            Utils.addArrayToList<byte>(peBuffer, this.sections.export(peBuffer.Count).ToArray());

            // adding overflowed data
            if(EOFOverflow)
                Utils.addArrayToList<byte>(peBuffer, this.overflowData());

            return peBuffer;
        }

        public byte[] overflowData()
        {
            if (!EOFOverflow)
                throw new NoOverflowDataException();

            return this.reader.readBytes(this.endOfFile, this.overflowSize);
        }

        public void Dispose()
        {
            this.reader.Dispose();
        }
    }
}
