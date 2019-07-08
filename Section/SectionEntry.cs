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

namespace Serana.Engine.Section
{
    public class SectionEntry
    {
        public readonly int sectionId;

        // searching the best way to hold section data
        //public DataEntry data;

        public readonly DataEntry name;
        public readonly NumericEntry virtualSize;
        public readonly NumericEntry virtualAddress;
        public readonly NumericEntry sizeOfRawData;
        public readonly NumericEntry pointerToRawData;
        public readonly NumericEntry pointerToRelocations;
        public readonly NumericEntry pointerToLineNumbers;
        public readonly NumericEntry numberOfRelocations;
        public readonly NumericEntry numberOfLinenumbers;
        public readonly NumericEntry characteristics;

        public List<Entry> entries;

        private static int id = 0;

        private Reader reader;

        public SectionEntry(Reader reader, DataEntry name, NumericEntry virtualSize, NumericEntry virtualAddress, NumericEntry sizeOfRawData, NumericEntry pointerToRawData, NumericEntry pointerToRelocations,
                NumericEntry pointerToLineNumbers, NumericEntry numberOfRelocations, NumericEntry numberOfLinenumbers, NumericEntry characteristics)
        {
            this.sectionId = id++;

            this.reader = reader;

            this.name = name;
            this.virtualSize = virtualSize;
            this.virtualAddress = virtualAddress;
            this.sizeOfRawData = sizeOfRawData;
            this.pointerToRawData = pointerToRawData;
            this.pointerToRelocations = pointerToRelocations;
            this.pointerToLineNumbers = pointerToLineNumbers;
            this.numberOfRelocations = numberOfRelocations;
            this.numberOfLinenumbers = numberOfLinenumbers;
            this.characteristics = characteristics;

            this.entries = new List<Entry>();

            this.entries.Add(this.name);
            this.entries.Add(this.pointerToRawData);
            this.entries.Add(this.sizeOfRawData);
            this.entries.Add(this.virtualSize);
            this.entries.Add(this.virtualAddress);
            this.entries.Add(this.pointerToRelocations);
            this.entries.Add(this.pointerToLineNumbers);
            this.entries.Add(this.numberOfRelocations);
            this.entries.Add(this.numberOfLinenumbers);
            this.entries.Add(this.characteristics);
        }

        public byte[] getSectionBuffer()
        {
            return this.reader.readBytes(this.pointerToRawData.getValue(), this.sizeOfRawData.getValue());
        }
    }
}
