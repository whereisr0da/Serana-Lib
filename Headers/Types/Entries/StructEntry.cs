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

using Serana.Engine.Streams;
using System.Collections.Generic;

namespace Serana.Engine.Headers.Types
{
    public class StructEntry<T> : Entry
    {
        public T value;
        public int offset;
        public string name;
        public EntrySize size;
        public EntrySize size64;

        public bool changeFor64 = false;

        private bool is32bit;

        public StructEntry(string name, int offset, EntrySize size)
        {
            this.name = name;
            this.offset = offset;
            this.size = size;
        }

        public StructEntry(List<Entry> list, bool is32bit, string name, int offset, EntrySize size)
        {
            this.name = name;

            this.is32bit = is32bit;

            if (list.Count < 1)
                this.offset = offset;
            else
            {
                Entry lastEntry = list[list.Count - 1];
                this.offset = lastEntry.getOffset() + ((int)lastEntry.getSize() / 8);
            }

            this.size = size;

            list.Add(this);
        }

        public string getName()
        {
            return this.name;
        }

        public StructEntry(List<Entry> list, bool is32bit, string name, int offset, EntrySize size32, EntrySize size64)
        {
            this.name = name;

            this.is32bit = is32bit;

            if (list.Count < 1)
                this.offset = offset;
            else
            {
                Entry lastEntry = list[list.Count - 1];
                this.offset = lastEntry.getOffset() + ((int)lastEntry.getSize() / 8);
            }

            this.size = size32;
            this.size64 = size64;

            list.Add(this);

            this.changeFor64 = true;
        }

        public int getOffset()
        {
            return this.offset;
        }

        public int getSize()
        {

            // TODO : dynamic

            // x64 mais meme size
            if (!this.changeFor64 && !is32bit)
                return (int)this.size;

            return is32bit ? (int)this.size : (int)this.size64;
        }

        public EntrySize getEntrySize()
        {
            // x64 mais meme size
            if (!this.changeFor64 && !is32bit)
                return this.size;

            return is32bit ? this.size : this.size64;
        }

        public void setMachineType(bool is32bit)
        {
            this.is32bit = is32bit;
        }

        public virtual byte[] export()
        {
            // TODO

            List<byte> result = new List<byte>();

            return result.ToArray();
        }

        public virtual void readValue(Reader reader)
        {

        }
    }
}
