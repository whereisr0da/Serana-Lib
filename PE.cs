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

using Serana.Engine.Headers;
using Serana.Engine.Headers.Types;
using Serana.Engine.Resource;
using Serana.Engine.Section;
using Serana.Engine.Streams;
using Serana.Engine.Exceptions;
using Serana.Engine.Import;

using System.Collections.Generic;
using System.IO;
using System;

namespace Serana.Engine
{
    public class PE
    {
        /// <summary>
        /// True if the PE object is not loaded from a file
        /// </summary>
        public readonly bool isMemoryPE;

        /// <summary>
        /// The header object of the PE file
        /// </summary>
        public Header header;

        public Sections sections;

        public Imports imports;

        public Resources resources;

        private Reader reader;

        public readonly string fileName;

        // TODO : BIG FILES
        public readonly int endOfData;

        /// <summary>
        /// True if there is more data after the sections
        /// </summary>
        public readonly bool EOFOverflow = false;

        /// <summary>
        /// Size of the overflow data
        /// TODO : BIG FILES
        /// </summary>
        public readonly int overflowSize = 0;

        /// <summary>
        /// Create a PE object from a file
        /// </summary>
        /// <param name="filePath"></param>
        public PE(string filePath)
        { 
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Fail to open the executable");

            this.fileName = new FileInfo(filePath).Name;

            this.reader = new Reader(filePath);

            this.header = new Header(this.reader);

            this.sections = new Sections(this.reader, this.header);

            // some times, executable could be import less (nasm compiled asm)
            // so idk how to handle this ...
            // throw new Exception("The import address table is not in any sections");
            if (isImportPresent())
                this.imports = new Imports(this.reader, this.header, this.sections);

            if(isResourcePresent())
                this.resources = new Resources(this.reader, this.header, this.sections);

            SectionEntry lastEntry = this.sections.getLastEntry();

            // calculate the EOF address
            this.endOfData = lastEntry.header.pointerToRawData.getValue() + lastEntry.header.sizeOfRawData.getValue();

            // check if there is more data
            this.EOFOverflow = this.endOfData != (int)this.reader.size();

            if (this.EOFOverflow)
                overflowSize = (int)this.reader.size() - this.endOfData;

            this.isMemoryPE = false;
        }

        /// <summary>
        /// Indicate if import information exists
        /// </summary>
        /// <returns>True if import directory is valid</returns>
        public bool isImportPresent()
        {
            var importVirtualAddress = this.header.dataDirectoryHeader.importTableAddressDirectory.getVirtualAddress();

            // get the section that content the import address table
            SectionEntry importAddressTableSection = this.sections.getSectionFromVirtualAddress(importVirtualAddress);

            return importAddressTableSection != null;
        }

        /// <summary>
        /// Create a PE object from memory
        /// </summary>
        /// <param name="baseOfCode"></param>
        /// <param name="baseOfData"></param>
        /// <param name="fileAlignment"></param>
        /// <param name="sectionAlignment"></param>
        public PE(int baseOfCode, int baseOfData, int fileAlignment, int sectionAlignment)
        {
            if (baseOfCode > baseOfData)
                throw new Exception("baseOfCode is upper than baseOfData");

            if (sectionAlignment < fileAlignment)
                throw new Exception("fileAlignment is upper than sectionAlignment");

            this.isMemoryPE = true;

            // create a default header
            this.header = new Header(baseOfCode, baseOfData, fileAlignment, sectionAlignment);

            this.sections = new Sections(this.header);

            // TODO : handle imports
            //this.imports = new Imports(this.reader, this.header, this.sections);

            // TODO : handle resources
            //this.resources = new Resources(this.reader, this.header, this.sections);
        }

        /// <summary>
        /// Indicate if ASLR is enable in the file
        /// </summary>
        /// <returns>True if ASLR is enable</returns>
        public bool isASLR()
        {
            return ((this.header.optionalHeader.DLLCharacteristics.getValue()) & (int)DllCharacteristics.DYNAMIC_BASE) > 0;
        }

        /// <summary>
        /// Indicate if DEP is enable in the file
        /// </summary>
        /// <returns>True if DEP is enable</returns>
        public bool isDEP()
        {
            return ((this.header.optionalHeader.DLLCharacteristics.getValue()) & (int)DllCharacteristics.NX_COMPAT) > 0;
        }

        /// <summary>
        /// Indicate if there is resources in the file
        /// </summary>
        /// <returns>True if resources is the executable</returns>
        public bool isResourcePresent()
        {
            return this.header.dataDirectoryHeader.resourceDirectory.getVirtualAddress() > 0
                && this.header.dataDirectoryHeader.resourceDirectory.getSize() > 0;
        }

        /// <summary>
        /// Indicate if the executable is .NET
        /// </summary>
        /// <returns>True if is .NET</returns>
        public bool isDOTNET()
        {
            return this.header.dataDirectoryHeader.netHeaderDirectory.getVirtualAddress() > 0 
                && this.header.dataDirectoryHeader.netHeaderDirectory.getSize() > 0;
        }

        /// <summary>
        /// Indicate if PE is 32 bit
        /// NOTE : Simple proxy
        /// </summary>
        /// <returns>True if is 32 bit</returns>
        public bool is32Bit()
        {
            return this.header.is32Bit;
        }

        /// <summary>
        /// Set and calculate the file checksum
        /// </summary>
        public void fixChecksum()
        {
            // TODO
        }

        /// <summary>
        /// Export raw bytes if the executable
        /// </summary>
        /// <returns>Array of byte representing the executable</returns>
        public List<byte> export()
        {
            List<byte> peBuffer = new List<byte>();

            // adding Commun Headers
            Utils.addArrayToList<byte>(peBuffer, this.header.export().ToArray());

            // adding Section Headers
            Utils.addArrayToList<byte>(peBuffer, this.sections.exportHeaders().ToArray());

            // TODO : handle imports
            // TODO : handle resources

            // adding Sections
            Utils.addArrayToList<byte>(peBuffer, this.sections.exportSectionsData().ToArray());

            // adding overflowed data
            if(EOFOverflow)
                Utils.addArrayToList<byte>(peBuffer, this.overflowData());

            return peBuffer;
        }

        /// <summary>
        /// Return overflowed data
        /// </summary>
        /// <returns>Array of byte of overflowed data</returns>
        public byte[] overflowData()
        {
            if (!this.EOFOverflow)
                throw new NoOverflowDataException();

            if (this.isMemoryPE)
                throw new Exception("Overflow data is not yet supported");

            return this.reader.readBytes(this.endOfData, this.overflowSize);
        }

        /// <summary>
        /// Dispose the stream
        /// </summary>
        public void Dispose()
        {
            if(!this.isMemoryPE)
                this.reader.Dispose();
        }
    }
}
