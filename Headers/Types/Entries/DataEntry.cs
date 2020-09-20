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

using System;
using System.Collections.Generic;
using System.Linq;
using Serana.Engine.Streams;

namespace Serana.Engine.Headers.Types
{
    public class DataEntry : NumericEntry
    {
        // an object array is too complex to handle (cast problems)
        // so I store values in each array of its value size
        public byte[] values8;
        public Int16[] values16;
        public Int32[] values32;
        public long[] values64;
        public int count;

        public DataEntry(List<Entry> list,
            bool is32bit,
            string name,
            int offset,
            int count,
            EntrySize size) : base(list, is32bit, name, offset, size)
        {
            this.count = count;
        }

        public override int getSize()
        {
            // x64 mais meme size
            if (!this.changeFor64 && !this.is32bit)
                return ((int)this.size / 8) * this.count;

            return this.is32bit ? ((int)this.size / 8) * this.count : ((int)this.size64 / 8) * this.count;
        }

        public override void readValue(Reader reader)
        {
            EntrySize currentSize = getEntrySize();
            int currentOffset = getOffset();

            // init the right buffer
            switch (currentSize)
            {
                case EntrySize._8Bits:
                    this.values8 = new byte[this.count];
                    break;
                case EntrySize._16Bits:
                    this.values16 = new Int16[this.count];
                    break;
                case EntrySize._32Bits:
                    this.values32 = new Int32[this.count];
                    break;
                case EntrySize._64Bits:
                    this.values64 = new long[this.count];
                    break;
                default:
                    break;
            }

            for (int i = 0; i < this.count; i++)
            {
                switch (currentSize)
                {
                    case EntrySize._8Bits:
                        this.values8[i] = reader.readByte(currentOffset + i);
                        break;
                    case EntrySize._16Bits:
                        this.values16[i] = reader.readInt16(currentOffset + (i * ((int)currentSize / 8)));
                        break;
                    case EntrySize._32Bits:
                        this.values32[i] = reader.readInt32(currentOffset + (i * ((int)currentSize / 8)));
                        break;
                    case EntrySize._64Bits:
                        this.values64[i] = reader.readInt64(currentOffset + (i * ((int)currentSize / 8)));
                        break;
                    default:
                        break;
                }
            }
        }

        public string getName()
        {
            return this.name;
        }

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < this.count; i++)
            {
                switch (getEntrySize())
                {
                    case EntrySize._8Bits:
                        result += (char)this.values8[i];
                        break;
                    case EntrySize._16Bits:
                        result += (char)this.values16[i];
                        break;
                    case EntrySize._32Bits:
                        result += (char)this.values32[i];
                        break;
                    case EntrySize._64Bits:
                        result += (char)this.values64[i];
                        break;
                    default:
                        break;
                }
            }

            return result.Replace("\x00", "");
        }

        public void setValue(byte[] value)
        {
            this.values8 = new byte[this.count];

            // loop only throw allowed length
            for (int i = 0; i < this.count; i++)
            {
                this.values8[i] = (byte)value[i];
            }
        }

        public void setValue(Int16[] value)
        {
            this.values16 = new Int16[this.count];

            // loop only throw allowed length
            for (int i = 0; i < this.count; i++)
            {
                this.values16[i] = (Int16)value[i];
            }
        }

        public void setValue(Int32[] value)
        {
            this.values32 = new Int32[this.count];

            // loop only throw allowed length
            for (int i = 0; i < this.count; i++)
            {
                this.values32[i] = (Int32)value[i];
            }
        }

        public void setValue(long[] value)
        {
            this.values64 = new long[this.count];

            // loop only throw allowed length
            for (int i = 0; i < this.count; i++)
            {
                this.values64[i] = (long)value[i];
            }
        }

        /*
         * too complex to handle with object array
         * 
        public void setValue(object[] value)
        {
            EntrySize currentSize = getEntrySize();

            // init the right buffer
            switch (currentSize)
            {
                case EntrySize._8Bits:
                    this.values8 = new byte[this.count];
                    break;
                case EntrySize._16Bits:
                    this.values16 = new Int16[this.count];
                    break;
                case EntrySize._32Bits:
                    this.values32 = new Int32[this.count];
                    break;
                case EntrySize._64Bits:
                    this.values64 = new long[this.count];
                    break;
                default:
                    break;
            }

            // loop only throw allowed length
            for (int i = 0; i < this.count; i++)
            {
                switch (currentSize)
                {
                    case EntrySize._8Bits:
                        if (value[i] != null)
                            this.values8[i] = (byte)value[i];
                        else
                            this.values8[i] = 0x00;
                        break;
                    case EntrySize._16Bits:
                        if (value[i] != null)
                            this.values16[i] = (Int16)value[i];
                        else
                            this.values16[i] = 0x00;
                        break;
                    case EntrySize._32Bits:
                        if (value[i] != null)
                            this.values32[i] = (Int32)value[i];
                        else
                            this.values32[i] = 0x00;
                        break;
                    case EntrySize._64Bits:
                        if (value[i] != null)
                            this.values64[i] = (long)value[i];
                        else
                            this.values64[i] = 0x00;
                        break;
                    default:
                        break;
                }
            }
        }
        */

        public override byte[] export()
        {
            List<byte> result = new List<byte>();

            for (int i = 0; i < this.count; i++)
            {
                switch (getEntrySize())
                {
                    case EntrySize._8Bits:
                        result.Add(this.values8[i]);
                        break;
                    case EntrySize._16Bits:
                        Utils.addArrayToList<byte>(result, Utils.ToInt16(this.values16[i]));
                        break;
                    case EntrySize._32Bits:
                        Utils.addArrayToList<byte>(result, Utils.ToInt32(this.values32[i]));
                        break;
                    case EntrySize._64Bits:
                        Utils.addArrayToList<byte>(result, Utils.ToInt64(this.values64[i]));
                        break;
                    default:
                        break;
                }
            }

            return result.ToArray();
        }
    }
}
