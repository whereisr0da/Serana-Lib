

using System;
using System.Collections.Generic;
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
namespace Serana.Engine.Headers.Types.Entries
{
    public class NumericUnsignedEntry : NumericEntry
    {
        private uint value;
        private UInt64 value64;

        public NumericUnsignedEntry(List<Entry> list, bool is32bit, string name, int offset, EntrySize size)
            : base(list, is32bit, name, offset, size)
        {

        }

        public object getValueUnsigned()
        {
            return (!base.is32bit && base.changeFor64) ? this.value64 : this.value;
        }
    }
}
