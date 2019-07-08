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

using System;
using System.Collections.Generic;

namespace Serana.Engine.Headers
{
    public class DOS_Header
    {
        private Reader reader;

        public readonly NumericEntry lastsize;

        public readonly NumericEntry nblocks;

        public readonly NumericEntry nreloc;

        public readonly NumericEntry hdrsize;

        public readonly NumericEntry minalloc;

        public readonly NumericEntry maxalloc;

        public readonly NumericEntry ss;

        public readonly NumericEntry sp;

        public readonly NumericEntry checksum;

        public readonly NumericEntry ip;

        public readonly NumericEntry cs;

        public readonly NumericEntry relocpos;

        public readonly NumericEntry noverlay;

        public readonly DataEntry reserved1;

        public readonly NumericEntry oem_id;

        public readonly NumericEntry oem_info;

        public readonly DataEntry reserved2;

        public readonly DataEntry dosProgram;

        public List<Entry> entries;

        public readonly NumericEntry peHeaderOffset;

        public DOS_Header(Reader reader)
        {
            this.reader = reader;

            this.entries = new List<Entry>();

            if (!isValidDosHeader())
            {
                throw new BadDosHeaderException();
            }

            int baseOffset = 0x2;

            // first address so statics

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

            // init values
            foreach (var item in entries)
            {
                item.readValue(this.reader);
            }

            this.dosProgram = new DataEntry(entries, true, "dosProgram", baseOffset, this.peHeaderOffset.getValue() - 0x40, EntrySize._8Bits);
            this.dosProgram.readValue(this.reader);
        }

        private bool isValidDosHeader()
        {
            // TODO : MORE CHECKS
            return Utils.bytesCompare(reader.readBytes(0, 2), HeaderSymbols.MSDOS_HEADER);
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
    }
}
