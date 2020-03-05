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

using System;
using System.Collections.Generic;

namespace Serana.Engine.Headers
{
    /// <summary>
    /// Object that contain all DOS header informations
    /// </summary>
    public class DOS_Header
    {
        private Reader reader;

        public NumericEntry lastsize;

        public NumericEntry nblocks;

        public NumericEntry nreloc;

        public NumericEntry hdrsize;

        public NumericEntry minalloc;

        public NumericEntry maxalloc;

        public NumericEntry ss;

        public NumericEntry sp;

        public NumericEntry checksum;

        public NumericEntry ip;

        public NumericEntry cs;

        public NumericEntry relocpos;

        public NumericEntry noverlay;

        public DataEntry reserved1;

        public NumericEntry oem_id;

        public NumericEntry oem_info;

        public DataEntry reserved2;

        public DataEntry dosProgram;

        public List<Entry> entries;

        public NumericEntry peHeaderOffset;

        /// <summary>
        /// Create a DOS header from file
        /// </summary>
        /// <param name="reader">The reader</param>
        public DOS_Header(Reader reader)
        {
            this.reader = reader;

            this.entries = new List<Entry>();

            if (!isValidDosHeader())
            {
                throw new BadDosHeaderException();
            }

            // first address so statics
            int baseOffset = 0x2;

            // init header
            setupStruct(ref baseOffset);

            ///
            /// init values
            /// 

            foreach (var item in entries)
            {
                item.readValue(this.reader);
            }

            // we need to know the peHeaderOffset
            this.dosProgram = new DataEntry(entries, true, "dosProgram", baseOffset, this.peHeaderOffset.getValue() - 0x40, EntrySize._8Bits);
            this.dosProgram.readValue(this.reader);
        }

        /// <summary>
        /// Create a DOS header from memory
        /// </summary>
        public DOS_Header()
        {
            this.entries = new List<Entry>();

            // first address so statics
            int baseOffset = 0x2;

            // init header
            setupStruct(ref baseOffset);

            ///
            /// init values
            /// 

            // TODO : Comment

            this.lastsize.setValue(0x90);
            this.nblocks.setValue(3);
            this.nreloc.setValue(0);
            this.hdrsize.setValue(4);
            this.minalloc.setValue(0);
            this.maxalloc.setValue(0xffff);
            this.ss.setValue(0);
            this.sp.setValue(0xb8);
            this.checksum.setValue(0);
            this.ip.setValue(0);
            this.cs.setValue(0);
            this.relocpos.setValue(0x40);
            this.noverlay.setValue(0);

            Int16[] buffer8 = new Int16[4];

            this.reserved1.setValue(buffer8);
            this.oem_id.setValue(0);
            this.oem_info.setValue(0);

            Int16[] buffer10 = new Int16[10];

            this.reserved2.setValue(buffer10);

            // DOS Header + defaultDOSProgram
            this.peHeaderOffset.setValue(0x80);

            // we need to know the peHeaderOffset
            this.dosProgram = new DataEntry(entries, true, "dosProgram", baseOffset, this.peHeaderOffset.getValue() - 0x40, EntrySize._8Bits);

            // A default DOS program that every PE file has
            this.dosProgram.setValue(defaultDOSProgram);
        }

        private void setupStruct(ref int baseOffset)
        {
            // TODO : clear the code

            // we don't care about x32 or x64
            this.lastsize = new NumericEntry(entries, true, "lastsize", baseOffset, EntrySize._16Bits);
            this.nblocks = new NumericEntry(entries, true, "nblocks", baseOffset, EntrySize._16Bits);
            this.nreloc = new NumericEntry(entries, true, "nreloc", baseOffset, EntrySize._16Bits);
            this.hdrsize = new NumericEntry(entries, true, "hdrsize", baseOffset, EntrySize._16Bits);
            this.minalloc = new NumericEntry(entries, true, "minalloc", baseOffset, EntrySize._16Bits);
            this.maxalloc = new NumericEntry(entries, true, "maxalloc", baseOffset, EntrySize._16Bits);
            this.ss = new NumericEntry(entries, true, "ss", baseOffset, EntrySize._16Bits);
            this.sp = new NumericEntry(entries, true, "sp", baseOffset, EntrySize._16Bits);
            this.checksum = new NumericEntry(entries, true, "checksum", baseOffset, EntrySize._16Bits);
            this.ip = new NumericEntry(entries, true, "ip", baseOffset, EntrySize._16Bits);
            this.cs = new NumericEntry(entries, true, "cs", baseOffset, EntrySize._16Bits);
            this.relocpos = new NumericEntry(entries, true, "relocpos", baseOffset, EntrySize._16Bits);
            this.noverlay = new NumericEntry(entries, true, "noverlay", baseOffset, EntrySize._16Bits);
            this.reserved1 = new DataEntry(entries, true, "reserved1", baseOffset, 4, EntrySize._16Bits);
            this.oem_id = new NumericEntry(entries, true, "oem_id", baseOffset, EntrySize._16Bits);
            this.oem_info = new NumericEntry(entries, true, "oem_info", baseOffset, EntrySize._16Bits);
            this.reserved2 = new DataEntry(entries, true, "reserved2", baseOffset, 10, EntrySize._16Bits);
            this.peHeaderOffset = new NumericEntry(entries, true, "peHeaderOffset", baseOffset, EntrySize._32Bits);
        }

        private bool isValidDosHeader()
        {
            bool check = true;

            check &= Utils.bytesCompare(reader.readBytes(0, 2), HeaderSymbols.MSDOS_HEADER);

            // TODO : MORE CHECKS

            return check;
        }

        public List<byte> export()
        {
            List<byte> dosHeaderBuffer = new List<byte>();

            // adding MZ header
            Utils.addArrayToList<byte>(dosHeaderBuffer, HeaderSymbols.MSDOS_HEADER);

            foreach (Entry item in entries)
            {
                Utils.addArrayToList<byte>(dosHeaderBuffer, item.export());
            }

            return dosHeaderBuffer;
        }

        public static byte[] defaultDOSProgram = {
            0x0E, 0x1F, 0xBA, 0x0E, 0x00, 0xB4, 0x09, 0xCD, 0x21, 0xB8, 0x01, 0x4C,
            0xCD, 0x21, 0x54, 0x68, 0x69, 0x73, 0x20, 0x70, 0x72, 0x6F, 0x67, 0x72,
            0x61, 0x6D, 0x20, 0x63, 0x61, 0x6E, 0x6E, 0x6F, 0x74, 0x20, 0x62, 0x65,
            0x20, 0x72, 0x75, 0x6E, 0x20, 0x69, 0x6E, 0x20, 0x44, 0x4F, 0x53, 0x20,
            0x6D, 0x6F, 0x64, 0x65, 0x2E, 0x0D, 0x0D, 0x0A, 0x24, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00
        };
    }
}
