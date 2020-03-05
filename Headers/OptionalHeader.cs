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
    /// Object that contain all Optional header informations
    /// </summary>
    public class OptionalHeader
    {
        private Reader reader;

        private PE_Header peHeader;

        /// <summary>
        /// The header start address 
        /// </summary>
        public int headerBaseAddress;

        /// <summary>
        /// Magic
        /// TODO : explainations
        /// </summary>
        public TypeEntry<MagicNumber> Magic;

        /// <summary>
        /// Major Linker Version
        /// TODO : explainations
        /// </summary>
        public NumericEntry MajorLinkerVersion;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry MinorLinkerVersion;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry SizeOfCode;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry SizeOfInitializedData;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry SizeOfUninitializedData;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry AddressOfEntryPoint;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry BaseOfCode;

        /// <summary>
        /// 
        /// NOTE : only used on x86
        /// TODO : explainations
        /// </summary>
        public NumericEntry BaseOfData;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry ImageBase;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry SectionAlignment;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry FileAlignment;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry MajorOSVersion;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry MinorOSVersion;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry MajorImageVersion;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry MinorImageVersion;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry MajorSubsystemVersion;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry MinorSubsystemVersion;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry Win32VersionValue;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry SizeOfImage;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry SizeOfHeaders;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry Checksum;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry Subsystem;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry DLLCharacteristics;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry SizeOfStackReserve;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry SizeOfStackCommit;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry SizeOfHeapReserve;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry SizeOfHeapCommit;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry LoaderFlags;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry NumberOfRvaAndSizes;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry peHeaderImageBase;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public NumericEntry peRelativeEntryPointOffset;

        /// <summary>
        /// TODO : explainations
        /// </summary>
        public TypeEntry<SubSystem> peSubSystem;

        /// <summary>
        /// End address of the header
        /// Now the header as dynamic size of we have to calculate it
        /// </summary>
        public int endOfHeader;

        public bool is32Bit;

        public List<Entry> entries;

        /// <summary>
        /// Create a Optional header from file
        /// </summary>
        /// <param name="reader">the reader</param>
        /// <param name="peHeader">the previous PE header</param>
        public OptionalHeader(Reader reader, PE_Header peHeader)
        {
            ///
            /// init header
            /// 

            this.reader = reader;

            this.entries = new List<Entry>();

            this.peHeader = peHeader;

            this.is32Bit = this.peHeader.Architecture.value == Machines.INTEL386;

            headerBaseAddress = this.peHeader.headerBaseAddress + this.peHeader.export().Count;

            setupStruct();

            ///
            /// init values
            /// 

            foreach (var item in entries)
            {
                item.readValue(this.reader);
            }

            // now the header is dynamic (is32bit)

            int sizeTmp = headerBaseAddress;

            entries.ForEach(e => sizeTmp += ((int)e.getEntrySize()) / 8);

            this.endOfHeader = sizeTmp;
        }

        /// <summary>
        /// Create a Optional header from memory
        /// </summary>
        /// <param name="peHeader">the previous PE header</param>
        public OptionalHeader(PE_Header peHeader)
        {
            ///
            /// init header
            /// 

            this.entries = new List<Entry>();

            this.peHeader = peHeader;

            // TODO : improve 32 bit detection
            this.is32Bit = this.peHeader.Architecture.value == Machines.INTEL386;

            headerBaseAddress = this.peHeader.headerBaseAddress + this.peHeader.export().Count;

            setupStruct();

            ///
            /// init values
            /// 

            // 32 bit by default
            this.Magic.setValue(MagicNumber.PE32);

            // linker indicate info about compiler
            // so I just fill random value
            this.MajorLinkerVersion.setValue(1);
            this.MinorLinkerVersion.setValue(0);

            // will be set by the user depending of its actions
            this.SizeOfCode.setValue(0);
            this.SizeOfInitializedData.setValue(0);
            this.SizeOfUninitializedData.setValue(0);
            this.AddressOfEntryPoint.setValue(0);
            this.BaseOfCode.setValue(0);

            // BaseOfData is not in x64
            if (this.is32Bit)
                this.BaseOfData.setValue(0);

            // default image base
            this.ImageBase.setValue(0x00010000);

            // will be set by the user depending of its actions
            this.SectionAlignment.setValue(0);
            this.FileAlignment.setValue(0);

            // default versions (discontinue)
            this.MajorOSVersion.setValue(4);
            this.MinorOSVersion.setValue(0);
            this.MajorImageVersion.setValue(0);
            this.MinorImageVersion.setValue(0);
            this.MajorSubsystemVersion.setValue(6);
            this.MinorSubsystemVersion.setValue(0);
            this.Win32VersionValue.setValue(0);

            // will be set by the user depending of its actions
            this.SizeOfImage.setValue(0);

            // will be written after
            this.SizeOfHeaders.setValue(0);

            // useless unless you create a driver
            this.Checksum.setValue(0);

            // default windows app
            this.peSubSystem.setValue(SubSystem.WINDOWS_GUI);

            // default (ASLR + DEP + TERMINAL_SERVER_AWARE)
            int characteristics = (int)DllCharacteristics.DYNAMIC_BASE + (int)DllCharacteristics.NX_COMPAT
                + (int)DllCharacteristics.TERMINAL_SERVER_AWARE;

            this.DLLCharacteristics.setValue(characteristics);

            // defaults
            this.SizeOfStackReserve.setValue(0x100000);
            this.SizeOfStackCommit.setValue(0x1000);
            this.SizeOfHeapReserve.setValue(0x100000);
            this.SizeOfHeapCommit.setValue(0x1000);

            // "Reserved, must be zero", probably driver stuff
            this.LoaderFlags.setValue(0);

            // in general its always 16 but, the user can decide
            this.NumberOfRvaAndSizes.setValue(16);

            // now the header is dynamic (is32bit)
            int sizeTmp = headerBaseAddress;

            entries.ForEach(e => sizeTmp += ((int)e.getEntrySize()) / 8);

            this.endOfHeader = sizeTmp;
        }

        private void setupStruct()
        {
            this.Magic = new TypeEntry<MagicNumber>(entries, this.is32Bit, "Magic", headerBaseAddress, EntrySize._16Bits);

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
