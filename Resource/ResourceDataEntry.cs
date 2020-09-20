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

using System;
using System.Collections.Generic;
using System.Text;

namespace Serana.Engine.Resource
{
    public class ResourceDataEntry
    {
        private Reader reader;

        private Resources resources;

        public NumericEntry OffsetToData;
        public NumericEntry DataSize;
        public NumericEntry CodePage;
        public NumericEntry Reserved;

        public ResourceDataEntry(Resources resources, Reader reader, List<Entry> entries, ref int offset)
        {
            this.reader = reader;
            this.resources = resources;

            setupStruct(ref offset);

            this.OffsetToData.readValue(this.reader);
            this.DataSize.readValue(this.reader);
            this.CodePage.readValue(this.reader);
            this.Reserved.readValue(this.reader);
        }

        private void setupStruct(ref int offset)
        {
            // TODO : clear the code

            this.OffsetToData = new NumericEntry(null, true, "OffsetToData", offset, EntrySize._32Bits);
            offset += (int)this.OffsetToData.getRawSize();

            this.DataSize = new NumericEntry(null, true, "DataSize", offset, EntrySize._32Bits);
            offset += (int)this.DataSize.getRawSize();

            this.CodePage = new NumericEntry(null, true, "CodePage", offset, EntrySize._32Bits);
            offset += (int)this.CodePage.getRawSize();

            this.Reserved = new NumericEntry(null, true, "Reserved", offset, EntrySize._32Bits);
            offset += (int)this.Reserved.getRawSize();
        }

        public byte[] getData()
        {
            int offsetToRaw = this.OffsetToData.getValue() - resources.resourceSection.header.virtualAddress.getValue() + resources.resourceSection.header.pointerToRawData.getValue();

            return this.reader.readBytes(offsetToRaw, this.DataSize.getValue());
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(String.Format("0x{0} OffsetToData 0x{1}{2}", OffsetToData.getOffset().ToString("X"), OffsetToData.getValue().ToString("X"), Environment.NewLine));
            stringBuilder.Append(String.Format("0x{0} DataSize 0x{1}{2}", DataSize.getOffset().ToString("X"), DataSize.getValue().ToString("X"), Environment.NewLine));
            stringBuilder.Append(String.Format("0x{0} CodePage 0x{1}{2}", CodePage.getOffset().ToString("X"), CodePage.getValue().ToString("X"), Environment.NewLine));
            stringBuilder.Append(String.Format("0x{0} Reserved 0x{1}{2}", Reserved.getOffset().ToString("X"), Reserved.getValue().ToString("X"), ""));

            return stringBuilder.ToString();
        }
    }
}
