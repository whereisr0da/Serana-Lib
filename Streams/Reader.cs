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
using System.IO;
using System.Text;

using Serana.Engine.Exceptions;

namespace Serana.Engine.Streams
{
    public class Reader
    {
        private string filePath;

        private FileStream handle;

        private BinaryReader buffer;

        public Reader(string filePath)
        {
            try
            {
                this.filePath = filePath;

                this.handle = new FileStream(filePath, FileMode.Open);
                this.buffer = new BinaryReader(this.handle);
            }
            catch (IOException e)
            {
                throw new FailToOpenException();
            }
        }

        public string fileName()
        {
            return new FileInfo(this.filePath).Name;
        }

        public long size()
        {
            return this.buffer.BaseStream.Length;
        }

        public byte readByte(int offset)
        {
            this.buffer.BaseStream.Position = offset;

            return this.buffer.ReadByte();
        }

        public byte[] readBytes(int offset, int count)
        {
            byte[] result = new byte[count];

            this.buffer.BaseStream.Position = offset;

            for (int i = 0; i < count; i++)
            {
                result[i] = this.buffer.ReadByte();
            }

            return result;
        }

        public string readString(int offset, int count)
        {
            return Encoding.UTF8.GetString(readBytes(offset, count));
        }

        public Int32 readInt32(int offset)
        {
            this.buffer.BaseStream.Position = offset;

            int a = this.buffer.ReadByte();
            int b = this.buffer.ReadByte();
            int c = this.buffer.ReadByte();
            int d = this.buffer.ReadByte();

            return ((d << 24) | (c << 16) | (b << 8) | a);
        }

        public long readInt64(int offset)
        {
            this.buffer.BaseStream.Position = offset;

            // long long
            return this.buffer.ReadInt64();
        }

        public Int16 readInt16(int offset)
        {
            this.buffer.BaseStream.Position = offset;

            int a = this.buffer.ReadByte();
            int b = this.buffer.ReadByte();

            return (short)((b << 8) | a);
        }

        public void Dispose()
        {
            buffer.Close();
        }
    }
}
