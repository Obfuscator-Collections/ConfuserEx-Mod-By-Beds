using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace Confuser.Core
{
    /// <summary>
    ///     Provides a set of utility methods
    /// </summary>
    public static class Utils
    {
        static readonly char[] hexCharset = "0123456789abcdef".ToCharArray();

        /// <summary>
        ///     Gets the value associated with the specified key, or default value if the key does not exists.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="defValue">The default value.</param>
        /// <returns>The value associated with the specified key, or the default value if the key does not exists</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defValue = default(TValue))
        {
            TValue ret;
            if (dictionary.TryGetValue(key, out ret))
                return ret;
            return defValue;
        }

        /// <summary>
        ///     Gets the value associated with the specified key, or default value if the key does not exists.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="defValueFactory">The default value factory function.</param>
        /// <returns>The value associated with the specified key, or the default value if the key does not exists</returns>
        public static TValue GetValueOrDefaultLazy<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TKey, TValue> defValueFactory)
        {
            TValue ret;
            if (dictionary.TryGetValue(key, out ret))
                return ret;
            return defValueFactory(key);
        }
        public static Random rnd = new Random();
        /// <summary>
        /// This method generates a random sizeof value, which will get replaced with an sizeof instruction
        /// </summary>
        /// <returns>random sizeof value</returns>
        public static int GetSizeofValue()
        {
            switch (rnd.Next(0, 4))
            {
                case 0: return 2;
                case 1: return 4;
                case 2: return 8;
                case 3: return 12;
                default:
                    return 16;
            }

        }
        /// <summary>
        /// Checks if numeric value can be replaced with an sizeof instruction
        /// </summary>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static bool IsSizeof(int operand)
        {
            if (operand == 2 || operand == 4 || operand == 8 || operand == 12 || operand == 16)
                return true;

            return false;
        }

        /// <summary>
        /// This function decides if a ldci4 instruction can be obfuscated
        /// </summary>
        /// <param name="instructions">instructions of target method</param>
        /// <param name="i">index of ldci4 instruction</param>
        /// <returns></returns>
        public static bool CanObfuscateLDCI4(IList<Instruction> instructions, int i)
        {
            if (instructions[i + 1].GetOperand() != null)
                if (instructions[i + 1].Operand.ToString().Contains("bool"))
                    return false;

            if (instructions[i + 1].OpCode == OpCodes.Newobj)
                return false;

            if (instructions[i].GetLdcI4Value() == 0 || instructions[i].GetLdcI4Value() == 1)
                return false;


            return true;
        }

        /// <summary>
        /// This is used for SecondReplaceStageProcessor class 
        /// </summary>
        /// <param name="size">size of target instruction</param>
        /// <param name="importer"></param>
        /// <returns>TypeDef with fully added size</returns>
        public static TypeDef CreateStruct(Importer importer, int size)
        {
            ITypeDefOrRef valueTypeRef = importer.Import(typeof(System.ValueType));
            TypeDef structDef = new TypeDefUser(Guid.NewGuid().ToString().Replace("-", ""), valueTypeRef);
            structDef.ClassLayout = new ClassLayoutUser(1, 0);
            structDef.Attributes |= TypeAttributes.Sealed | TypeAttributes.SequentialLayout | TypeAttributes.Public;
            List<Type> typeList = CreateTypeList(size);

            typeList.ForEach(x => structDef.Fields.Add(new FieldDefUser(Guid.NewGuid().ToString(), new FieldSig(importer.Import(x).ToTypeSig()), FieldAttributes.Public)));
            /*
            while (operand != 0)
            {
                if (16 <= operand)
                {
                    structDef.Fields.Add(new FieldDefUser(Guid.NewGuid().ToString(), new FieldSig(importer.Import(GetType(16)).ToTypeSig()), FieldAttributes.Public));
                    operand -= 16;
                }
                else if (12 <= operand)
                {
                    structDef.Fields.Add(new FieldDefUser(Guid.NewGuid().ToString(), new FieldSig(importer.Import(GetType(12)).ToTypeSig()), FieldAttributes.Public));
                    operand -= 12;
                }
                else if (8 <= operand)
                {
                    structDef.Fields.Add(new FieldDefUser(Guid.NewGuid().ToString(), new FieldSig(importer.Import(GetType(8)).ToTypeSig()), FieldAttributes.Public));
                    operand -= 8;
                }
                else if (4 <= operand)
                {
                    structDef.Fields.Add(new FieldDefUser(Guid.NewGuid().ToString(), new FieldSig(importer.Import(GetType(4)).ToTypeSig()), FieldAttributes.Public));
                    operand -= 4;
                }
                else if (2 <= operand)
                {
                    structDef.Fields.Add(new FieldDefUser(Guid.NewGuid().ToString(), new FieldSig(importer.Import(GetType(2)).ToTypeSig()), FieldAttributes.Public));
                    operand -= 2;
                }
                else if (1 <= operand)
                {
                    structDef.Fields.Add(new FieldDefUser(Guid.NewGuid().ToString(), new FieldSig(importer.Import(GetType(1)).ToTypeSig()), FieldAttributes.Public));
                    operand -= 1;
                }
            }
            */

            return structDef;

        }
        /// <summary>
        /// Creates a list of types with the same size as a specific size
        /// </summary>
        /// <param name="size">target size to reach</param>
        /// <returns>List of types which have the size of the target size</returns>
        public static List<Type> CreateTypeList(int size)
        {
            List<Type> retList = new List<Type>();
            while (size != 0)
            {
                if (16 <= size)
                {
                    size -= 16;
                    retList.Add(GetType(16));
                }
                else if (12 <= size)
                {
                    size -= 12;
                    retList.Add(GetType(12));
                }
                else if (8 <= size)
                {
                    size -= 8;
                    retList.Add(GetType(8));
                }
                else if (4 <= size)
                {
                    size -= 4;
                    retList.Add(GetType(4));
                }
                else if (2 <= size)
                {
                    size -= 2;
                    retList.Add(GetType(2));
                }
                else if (1 <= size)
                {
                    size -= 1;
                    retList.Add(GetType(1));
                }
            }


            return retList;
        }
        /// <summary>
        /// Create sizeof instruction for target operand
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static Instruction GetSizeofInstruction(Importer importer, int operand)
        {
            return OpCodes.Sizeof.ToInstruction(importer.Import(GetType(operand)));
        }

        private static Type GetType(int operand)
        {
            switch (operand)
            {
                case 1:
                    switch (rnd.Next(0, 3))
                    {
                        case 0: return typeof(Boolean);
                        case 1: return typeof(SByte);
                        case 2: return typeof(Byte);
                    }
                    break;
                case 2:
                    switch (rnd.Next(0, 3))
                    {
                        case 0: return typeof(Int16);
                        case 1: return typeof(UInt16);
                        case 2: return typeof(Char);
                    }
                    break;
                case 4:
                    switch (rnd.Next(0, 3))
                    {
                        case 0: return typeof(Int32);
                        case 1: return typeof(Single);
                        case 2: return typeof(UInt32);
                    }
                    break;
                case 8:
                    switch (rnd.Next(0, 5))
                    {
                        case 0: return typeof(DateTime);
                        case 1: return typeof(TimeSpan);
                        case 2: return typeof(Int64);
                        case 3: return typeof(Double);
                        case 4: return typeof(UInt64);
                    }
                    break;

                case 12: return typeof(ConsoleKeyInfo);

                case 16:
                    switch (rnd.Next(0, 2))
                    {
                        case 0: return typeof(Guid);
                        case 1: return typeof(Decimal);
                    }
                    break;
            }

            return null;
        }


        /// <summary>
        ///     Adds the specified key and value to the multi dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of key.</typeparam>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="self">The dictionary to add to.</param>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <exception cref="System.ArgumentNullException">key is <c>null</c>.</exception>
        public static void AddListEntry<TKey, TValue>(this IDictionary<TKey, List<TValue>> self, TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            List<TValue> list;
            if (!self.TryGetValue(key, out list))
                list = self[key] = new List<TValue>();
            list.Add(value);
        }

        /// <summary>
        ///     Obtains the relative path from the specified base path.
        /// </summary>
        /// <param name="filespec">The file path.</param>
        /// <param name="folder">The base path.</param>
        /// <returns>The path of <paramref name="filespec" /> relative to <paramref name="folder" />.</returns>
        public static string GetRelativePath(string filespec, string folder)
        {
            //http://stackoverflow.com/a/703292/462805

            var pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }
            var folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        /// <summary>
        ///     If the input string is empty, return null; otherwise, return the original input string.
        /// </summary>
        /// <param name="val">The input string.</param>
        /// <returns><c>null</c> if the input string is empty; otherwise, the original input string.</returns>
        public static string NullIfEmpty(this string val)
        {
            if (string.IsNullOrEmpty(val))
                return null;
            return val;
        }

        /// <summary>
        ///     Compute the SHA1 hash of the input buffer.
        /// </summary>
        /// <param name="buffer">The input buffer.</param>
        /// <returns>The SHA1 hash of the input buffer.</returns>
        public static byte[] SHA1(byte[] buffer)
        {
            var sha = new SHA1Managed();
            return sha.ComputeHash(buffer);
        }

        /// <summary>
        ///     Xor the values in the two buffer together.
        /// </summary>
        /// <param name="buffer1">The input buffer 1.</param>
        /// <param name="buffer2">The input buffer 2.</param>
        /// <returns>The result buffer.</returns>
        /// <exception cref="System.ArgumentException">Length of the two buffers are not equal.</exception>
        public static byte[] Xor(byte[] buffer1, byte[] buffer2)
        {
            if (buffer1.Length != buffer2.Length)
                throw new ArgumentException("Length mismatched.");
            var ret = new byte[buffer1.Length];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = (byte)(buffer1[i] ^ buffer2[i]);
            return ret;
        }

        /// <summary>
        ///     Compute the SHA256 hash of the input buffer.
        /// </summary>
        /// <param name="buffer">The input buffer.</param>
        /// <returns>The SHA256 hash of the input buffer.</returns>
        public static byte[] SHA256(byte[] buffer)
        {
            var sha = new SHA256Managed();
            return sha.ComputeHash(buffer);
        }

        /// <summary>
        ///     Encoding the buffer to a string using specified charset.
        /// </summary>
        /// <param name="buff">The input buffer.</param>
        /// <param name="charset">The charset.</param>
        /// <returns>The encoded string.</returns>
        public static string EncodeString(byte[] buff, char[] charset)
        {
            int current = buff[0];
            var ret = new StringBuilder();
            for (int i = 1; i < buff.Length; i++)
            {
                current = (current << 8) + buff[i];
                while (current >= charset.Length)
                {
                    ret.Append(charset[current % charset.Length]);
                    current /= charset.Length;
                }
            }
            if (current != 0)
                ret.Append(charset[current % charset.Length]);
            return ret.ToString();
        }

        /// <summary>
        ///     Returns a new string in which all occurrences of a specified string in
        ///     <paramref name="str" /><paramref name="str" /> are replaced with another specified string.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> equivalent to <paramref name="str" /> but with all instances of
        ///     <paramref name="oldValue" />
        ///     replaced with <paramref name="newValue" />.
        /// </returns>
        /// <param name="str">A string to do the replace in. </param>
        /// <param name="oldValue">A string to be replaced. </param>
        /// <param name="newValue">A string to replace all occurrences of <paramref name="oldValue" />. </param>
        /// <param name="comparison">One of the <see cref="StringComparison" /> values. </param>
        /// <remarks>Adopted from http://stackoverflow.com/a/244933 </remarks>
        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;
                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }


        /// <summary>
        ///     Encode the buffer to a hexadecimal string.
        /// </summary>
        /// <param name="buff">The input buffer.</param>
        /// <returns>A hexadecimal representation of input buffer.</returns>
        public static string ToHexString(byte[] buff)
        {
            var ret = new char[buff.Length * 2];
            int i = 0;
            foreach (byte val in buff)
            {
                ret[i++] = hexCharset[val >> 4];
                ret[i++] = hexCharset[val & 0xf];
            }
            return new string(ret);
        }

        /// <summary>
        ///     Removes all elements that match the conditions defined by the specified predicate from a the list.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="self" />.</typeparam>
        /// <param name="self">The list to remove from.</param>
        /// <param name="match">The predicate that defines the conditions of the elements to remove.</param>
        /// <returns><paramref name="self" /> for method chaining.</returns>
        public static IList<T> RemoveWhere<T>(this IList<T> self, Predicate<T> match)
        {
            for (int i = self.Count - 1; i >= 0; i--)
            {
                if (match(self[i]))
                    self.RemoveAt(i);
            }
            return self;
        }

        /// <summary>
        ///     Returns a <see cref="IEnumerable{T}" /> that log the progress of iterating the specified list.
        /// </summary>
        /// <typeparam name="T">The type of list element</typeparam>
        /// <param name="enumerable">The list.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A wrapper of the list.</returns>
        public static IEnumerable<T> WithProgress<T>(this IEnumerable<T> enumerable, ILogger logger)
        {
            var list = new List<T>(enumerable);
            int i;
            for (i = 0; i < list.Count; i++)
            {
                logger.Progress(i, list.Count);
                yield return list[i];
            }
            logger.Progress(i, list.Count);
            logger.EndProgress();
        }
    }
}