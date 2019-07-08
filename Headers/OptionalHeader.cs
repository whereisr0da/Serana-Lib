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
    public class OptionalHeader
    {
        private Reader reader;

        private PE_Header peHeader;

        public readonly int headerBaseAddress;

        public readonly NumericEntry Magic;

        public readonly NumericEntry MajorLinkerVersion;

        public readonly NumericEntry MinorLinkerVersion;

        public readonly NumericEntry SizeOfCode;

        public readonly NumericEntry SizeOfInitializedData;

        public readonly NumericEntry SizeOfUninitializedData;

        public readonly NumericEntry AddressOfEntryPoint;

        public readonly NumericEntry BaseOfCode;

        // only x32
        public readonly NumericEntry BaseOfData;

        public readonly NumericEntry ImageBase;

        public readonly NumericEntry SectionAlignment;

        public readonly NumericEntry FileAlignment;

        public readonly NumericEntry MajorOSVersion;

        public readonly NumericEntry MinorOSVersion;

        public readonly NumericEntry MajorImageVersion;

        public readonly NumericEntry MinorImageVersion;

        public readonly NumericEntry MajorSubsystemVersion;

        public readonly NumericEntry MinorSubsystemVersion;

        public readonly NumericEntry Win32VersionValue;

        public readonly NumericEntry SizeOfImage;

        public readonly NumericEntry SizeOfHeaders;

        public readonly NumericEntry Checksum;

        public readonly NumericEntry Subsystem;

        // TODO : TypeEntry<DllCharacteristics>
        public readonly NumericEntry DLLCharacteristics;

        public readonly NumericEntry SizeOfStackReserve;

        public readonly NumericEntry SizeOfStackCommit;

        public readonly NumericEntry SizeOfHeapReserve;

        public readonly NumericEntry SizeOfHeapCommit;

        public readonly NumericEntry LoaderFlags;

        public readonly NumericEntry NumberOfRvaAndSizes;

        public readonly NumericEntry peHeaderImageBase;

        public readonly NumericEntry peRelativeEntryPointOffset;

        public readonly TypeEntry<SubSystem> peSubSystem;

        public readonly int endOfHeader;

        public readonly bool is32Bit;

        public List<Entry> entries;

        public OptionalHeader(Reader reader, PE_Header peHeader)
        {
            this.reader = reader;

            this.entries = new List<Entry>();

            this.peHeader = peHeader;

            this.is32Bit = this.peHeader.Architecture.value == Machines.INTEL386;

            headerBaseAddress = this.peHeader.PeHeaderOffset.getValue() + 0x18;

            this.Magic = new NumericEntry(entries, this.is32Bit, "Magic", headerBaseAddress, EntrySize._16Bits);
            
            this.MajorLinkerVersion = new NumericEntry(entries, this.is32Bit, "MajorLinkerVersion", headerBaseAddress, EntrySize._8Bits);

            this.MinorLinkerVersion = new NumericEntry(entries, this.is32Bit, "MinorLinkerVersion", headerBaseAddress, EntrySize._8Bits);

            this.SizeOfCode = new NumericEntry(entries, this.is32Bit, "SizeOfCode", headerBaseAddress, EntrySize._32Bits);

            this.SizeOfInitializedData = new NumericEntry(entries, this.is32Bit, "SizeOfInitializedData", headerBaseAddress, EntrySize._32Bits);

            this.SizeOfUninitializedData = new NumericEntry(entries, this.is32Bit, "SizeOfUninitializedData", headerBaseAddress, EntrySize._32Bits);

            this.AddressOfEntryPoint = new NumericEntry(entries, this.is32Bit, "AddressOfEntryPoint", headerBaseAddress, EntrySize._32Bits);

            this.BaseOfCode = new NumericEntry(entries, this.is32Bit, "BaseOfCode", headerBaseAddress, EntrySize._32Bits);

            // BaseOfData is not in x64
            if (this.is32Bit)
                this.BaseOfData = new NumericEntry(entries, this.is32Bit, "BaseOfData", headerBaseAddress, EntrySize._32Bits);

            this.ImageBase = new NumericEntry(entries, this.is32Bit, "ImageBase", headerBaseAddress, EntrySize._32Bits, EntrySize._64Bits);

            this.SectionAlignment = new NumericEntry(entries, this.is32Bit, "SectionAlignment", headerBaseAddress, EntrySize._32Bits);

            this.FileAlignment = new NumericEntry(entries, this.is32Bit, "FileAlignment", headerBaseAddress, EntrySize._32Bits);

            this.MajorOSVersion = new NumericEntry(entries, this.is32Bit, "MajorOSVersion", headerBaseAddress, EntrySize._16Bits);

            this.MinorOSVersion = new NumericEntry(entries, this.is32Bit, "MinorOSVersion", headerBaseAddress, EntrySize._16Bits);

            this.MajorImageVersion = new NumericEntry(entries, this.is32Bit, "MajorImageVersion", headerBaseAddress, EntrySize._16Bits);

            this.MinorImageVersion = new NumericEntry(entries, this.is32Bit, "MinorImageVersion", headerBaseAddress, EntrySize._16Bits);

            this.MajorSubsystemVersion = new NumericEntry(entries, this.is32Bit, "MajorSubsystemVersion", headerBaseAddress, EntrySize._16Bits);

            this.MinorSubsystemVersion = new NumericEntry(entries, this.is32Bit, "MinorSubsystemVersion", headerBaseAddress, EntrySize._16Bits);

            this.Win32VersionValue = new NumericEntry(entries, this.is32Bit, "Win32VersionValue", headerBaseAddress, EntrySize._32Bits);

            this.SizeOfImage = new NumericEntry(entries, this.is32Bit, "SizeOfImage", headerBaseAddress, EntrySize._32Bits);

            this.SizeOfHeaders = new NumericEntry(entries, this.is32Bit, "SizeOfHeaders", headerBaseAddress, EntrySize._32Bits);

            this.Checksum = new NumericEntry(entries, this.is32Bit, "Checksum", headerBaseAddress, EntrySize._32Bits);

            this.peSubSystem = new TypeEntry<SubSystem>(entries, this.is32Bit, "SubSystem", headerBaseAddress, EntrySize._16Bits);

            this.DLLCharacteristics = new NumericEntry(entries, this.is32Bit, "DLLCharacteristics", headerBaseAddress, EntrySize._16Bits);

            this.SizeOfStackReserve = new NumericEntry(entries, this.is32Bit, "SizeOfStackReserve", headerBaseAddress, EntrySize._32Bits, EntrySize._64Bits);

            this.SizeOfStackCommit = new NumericEntry(entries, this.is32Bit, "SizeOfStackCommit", headerBaseAddress, EntrySize._32Bits, EntrySize._64Bits);

            this.SizeOfHeapReserve = new NumericEntry(entries, this.is32Bit, "SizeOfHeapReserve", headerBaseAddress, EntrySize._32Bits, EntrySize._64Bits);

            this.SizeOfHeapCommit = new NumericEntry(entries, this.is32Bit, "SizeOfHeapCommit", headerBaseAddress, EntrySize._32Bits, EntrySize._64Bits);

            this.LoaderFlags = new NumericEntry(entries, this.is32Bit, "LoaderFlags", headerBaseAddress, EntrySize._32Bits);

            this.NumberOfRvaAndSizes = new NumericEntry(entries, this.is32Bit, "NumberOfRvaAndSizes", headerBaseAddress, EntrySize._32Bits);

            // init values
            foreach (var item in entries)
            {
                item.readValue(this.reader);
            }

            // now the header is dynamic (is32bit)

            int sizeTmp = headerBaseAddress;

            entries.ForEach(e => sizeTmp += ((int)e.getEntrySize()) / 8);

            this.endOfHeader = sizeTmp;
        }

        public List<byte> export()
        {
            List<byte> headerBuffer = new List<byte>();

            foreach (Entry item in entries)
            {
                Utils.addArrayToList<byte>(headerBuffer, item.export());
            }

            return headerBuffer;
        }
    }
}
