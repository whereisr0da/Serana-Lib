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

namespace Serana.Engine.Headers
{
    public class DataDirectory
    {
        public DataEntry entry;

        public DataDirectory(List<DataDirectory> list, DataEntry entry)
        {
            this.entry = entry;

            this.entry.values32 = new Int32[2];

            list.Add(this);
        }

        public int getVirtualAddress()
        {
            return this.entry.values32[0];
        }

        public int getSize()
        {
            return this.entry.values32[1];
        }

        public void setVirtualAddress(Int32 value)
        {
            this.entry.values32[0] = value;
        }

        public void setSize(Int32 value)
        {
            this.entry.values32[1] = value;
        }
    }
}
