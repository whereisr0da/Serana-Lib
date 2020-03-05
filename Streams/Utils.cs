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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Serana.Engine.Streams
{
    public class Utils
    {
        /// <summary>
        /// Add an array to a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="array"></param>
        public static void addArrayToList<T>(List<T> list, T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                list.Add(array[i]);
            }
        }

        /// <summary>
        /// Convert a int value intro a byte buffer representation on 16 bits (2 bytes)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToInt16(int value)
        {
            return new byte[2] { (byte)value, (byte)(value >> 8) };
        }

        /// <summary>
        /// Convert a int value intro a byte buffer representation on 32 bits (4 bytes)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToInt32(int value)
        {
            return new byte[4] { (byte)value, (byte)(value >> 8), (byte)(value >> 16), (byte)(value >> 24) };
        }

        /// <summary>
        /// Convert a int value intro a byte buffer representation on 64 bits (8 bytes)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToInt64(Int64 value)
        {
            return new byte[8] {
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24),
                (byte)(value >> 32),
                (byte)(value >> 40),
                (byte)(value >> 48),
                (byte)(value >> 56)
            };
        }

        /// <summary>
        /// Compare two byte arrays
        /// </summary>
        /// <param name="ar1"></param>
        /// <param name="ar2"></param>
        /// <returns></returns>
        public static bool bytesCompare(byte[] ar1, byte[] ar2)
        {
            if (ar1.Length != ar2.Length)
                return false;

            for (int i = 0; i < ar1.Length; i++)
            {
                if (ar1[i] != ar2[i])
                    return false;
            }

            return true;
        }
    }
}
