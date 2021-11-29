using System;
using System.Collections.Generic;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;
using dnlib.IO;
using dnlib.PE;

namespace Confuser.Core
{
	// Token: 0x02000047 RID: 71
	internal class NativeEraser
	{
		// Token: 0x060001AC RID: 428 RVA: 0x0000D6BD File Offset: 0x0000B8BD
		private static void Erase(Tuple<uint, uint, byte[]> section, uint offset, uint len)
		{
			Array.Clear(section.Item3, (int)(offset - section.Item1), (int)len);
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000D6D4 File Offset: 0x0000B8D4
		private static void Erase(List<Tuple<uint, uint, byte[]>> sections, uint beginOffset, uint size)
		{
			foreach (Tuple<uint, uint, byte[]> sect in sections)
			{
				if (beginOffset >= sect.Item1 && beginOffset + size < sect.Item2)
				{
					NativeEraser.Erase(sect, beginOffset, size);
					break;
				}
			}
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000D73C File Offset: 0x0000B93C
		private static void Erase(List<Tuple<uint, uint, byte[]>> sections, IFileSection s)
		{
			foreach (Tuple<uint, uint, byte[]> sect in sections)
			{
				if ((uint)s.StartOffset >= sect.Item1 && (uint)s.EndOffset < sect.Item2)
				{
					NativeEraser.Erase(sect, (uint)s.StartOffset, (uint)(s.EndOffset - s.StartOffset));
					break;
				}
			}
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000D7C0 File Offset: 0x0000B9C0
		private static void Erase(List<Tuple<uint, uint, byte[]>> sections, uint methodOffset)
		{
			foreach (Tuple<uint, uint, byte[]> sect in sections)
			{
				if (methodOffset >= sect.Item1)
				{
					uint f = (uint)sect.Item3[(int)((UIntPtr)(methodOffset - sect.Item1))];
					uint size;
					switch (f & 7u)
					{
					case 2u:
					case 6u:
						size = (f >> 2) + 1u;
						break;
					case 3u:
					{
						f |= (uint)((uint)sect.Item3[(int)((UIntPtr)(methodOffset - sect.Item1 + 1u))] << 8);
						size = (f >> 12) * 4u;
						uint codeSize = BitConverter.ToUInt32(sect.Item3, (int)(methodOffset - sect.Item1 + 4u));
						size += codeSize;
						break;
					}
					case 4u:
					case 5u:
						goto IL_98;
					default:
						goto IL_98;
					}
					NativeEraser.Erase(sect, methodOffset, size);
					continue;
					IL_98:
					break;
				}
			}
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000D89C File Offset: 0x0000BA9C
		public static void Erase(NativeModuleWriter writer, ModuleDefMD module)
		{
			if (writer == null || module == null)
			{
				return;
			}
			List<Tuple<uint, uint, byte[]>> sections = new List<Tuple<uint, uint, byte[]>>();
			MemoryStream s = new MemoryStream();
			foreach (NativeModuleWriter.OrigSection origSect in writer.OrigSections)
			{
				BinaryReaderChunk oldChunk = origSect.Chunk;
				ImageSectionHeader sectHdr = origSect.PESection;
				s.SetLength(0L);
				oldChunk.WriteTo(new BinaryWriter(s));
				byte[] buf = s.ToArray();
				BinaryReaderChunk newChunk = new BinaryReaderChunk(MemoryImageStream.Create(buf), oldChunk.GetVirtualSize());
				newChunk.SetOffset(oldChunk.FileOffset, oldChunk.RVA);
				origSect.Chunk = newChunk;
				sections.Add(Tuple.Create<uint, uint, byte[]>(sectHdr.PointerToRawData, sectHdr.PointerToRawData + sectHdr.SizeOfRawData, buf));
			}
			IMetaData md = module.MetaData;
			uint row = md.TablesStream.MethodTable.Rows;
			for (uint i = 1u; i <= row; i += 1u)
			{
				RawMethodRow method = md.TablesStream.ReadMethodRow(i);
				if ((method.ImplFlags & 3) == 0)
				{
					NativeEraser.Erase(sections, (uint)md.PEImage.ToFileOffset((RVA)method.RVA));
				}
			}
			ImageDataDirectory res = md.ImageCor20Header.Resources;
			if (res.Size > 0u)
			{
				NativeEraser.Erase(sections, (uint)res.StartOffset, res.Size);
			}
			NativeEraser.Erase(sections, md.ImageCor20Header);
			NativeEraser.Erase(sections, md.MetaDataHeader);
			foreach (DotNetStream stream in md.AllStreams)
			{
				NativeEraser.Erase(sections, stream);
			}
		}
	}
}
