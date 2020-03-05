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
using Serana.Engine.Section.Types;
using Serana.Engine.Streams;
using System.Collections.Generic;

namespace Serana.Engine.Section
{
    /// <summary>
    /// Object that represent the section's data
    /// NOTE : I don't know exactly how properly handle it right now
    /// </summary>
    public class SectionEntry
    {
        /// <summary>
        /// Static section id counter
        /// </summary>
        private static int id = 0;

        private Reader reader;

        private bool isInMemory = false;

        public readonly int sectionId;

        public SectionHeader header;

        public SectionTypes type;

        /// <summary>
        /// Represent the raw data
        /// NOTE : searching the best way to hold section data
        /// TODO : MAKE SOMETHING MUCH MORE SMART
        /// </summary>
        public byte[] rawData = null;

        /// <summary>
        /// Create a section entry from file
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="reader"></param>
        /// <param name="mainHeader"></param>
        public SectionEntry(List<Entry> entries, Reader reader, Header mainHeader)
        {
            this.sectionId = id++;

            this.reader = reader;  

            this.header = new SectionHeader(entries, this.reader, mainHeader);

            this.rawData = this.reader.readBytes(this.header.pointerToRawData.getValue(), this.header.sizeOfRawData.getValue());

            // TODO : fix order characteristics check (see how it handle .data dir)

            this.type = (this.header.characteristics.getValue() & (int)SectionFlags.IMAGE_SCN_MEM_EXECUTE) > 0 
                ? SectionTypes.CODE_SECTION : SectionTypes.DATA_SECTION; 

            // TODO : handle more things ? 
        }

        /// <summary>
        /// Create a section entry from memory
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="mainHeader"></param>
        public SectionEntry(List<Entry> entries, Header mainHeader)
        {
            this.sectionId = id++;

            this.header = new SectionHeader(entries, mainHeader);

            this.isInMemory = true;

            // TODO : handle more things ? 
        }

        /// <summary>
        /// Set the section's name
        /// NOTE : simple proxy
        /// </summary>
        /// <param name="name"></param>
        public void setSectionName(string name)
        {
            this.header.setName(name);
        }

        /// <summary>
        /// Get the section data
        /// </summary>
        /// <returns>Array of byte corresponding to the section's data</returns>
        public byte[] getSectionBuffer()
        {
            byte[] result = this.rawData;

            // at the time I don't store section's data
            //if (!isInMemory && result == null)
            //{
            //    return this.reader.readBytes(this.header.pointerToRawData.getValue(), this.header.sizeOfRawData.getValue());
            //}

            return result;
        }
    }
}
