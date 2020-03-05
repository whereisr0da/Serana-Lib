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
using Serana.Engine.Section;
using Serana.Engine.Streams;

using System;

namespace Serana.Engine.Import
{
    public class Imports
    {
        public SectionEntry section;

        private Reader reader;
        private Header header;
        private Sections sections;

        public ImportAddressTable importAddressTable;
        public ImportDirectory importDirectory;

        /// <summary>
        /// Create a Import object from file and setup the import dir
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="header"></param>
        /// <param name="sections"></param>
        public Imports(Reader reader, Header header, Sections sections)
        {
            this.reader = reader;
            this.header = header;
            this.sections = sections;

            var importVirtualAddress = this.header.dataDirectoryHeader.importTableAddressDirectory.getVirtualAddress();

            // get the section that content the import address table
            SectionEntry importAddressTableSection = this.sections.getSectionFromVirtualAddress(importVirtualAddress);

            if (importAddressTableSection == null)
                throw new Exception("The import address table is not in any sections");

            // get the section that content the import directory
            SectionEntry importDirectorySection = this.sections.getSectionFromVirtualAddress(this.header.dataDirectoryHeader.importDirectory.getVirtualAddress());

            if (importDirectorySection == null)
                throw new Exception("The import directory is not in any sections");

            // check that the two data are in the same section
            if(importAddressTableSection.sectionId != importDirectorySection.sectionId)
                throw new Exception("The import directory is not in the same section of import address table");

            // set the section that content the imports data
            this.section = importAddressTableSection;
        }
    }
}
