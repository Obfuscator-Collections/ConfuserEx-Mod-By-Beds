#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using KoiVM.AST.IL;
using KoiVM.VM;

namespace KoiVM.RT
{
	internal class HeaderChunk : IKoiChunk
	{
		private byte[] data;

		public uint Length { get; set; }

		public HeaderChunk(VMRuntime rt)
		{
			Length = ComputeLength(rt);
		}

		public void OnOffsetComputed(uint offset)
		{
		}

		public byte[] GetData()
		{
			return data;
		}

		private uint GetCodedLen(MDToken token)
		{
			switch (token.Table)
			{
			case Table.TypeRef:
			case Table.TypeDef:
			case Table.Field:
			case Table.Method:
			case Table.MemberRef:
			case Table.TypeSpec:
			case Table.MethodSpec:
				return Utils.GetCompressedUIntLength(token.Rid << 3);
			default:
				throw new NotSupportedException();
			}
		}

		private uint GetCodedToken(MDToken token)
		{
			Table table = token.Table;
			if (table <= Table.MemberRef)
			{
				switch (table)
				{
					case Table.TypeRef:
						return (token.Rid << 3) | 2U;
					case Table.TypeDef:
						return (token.Rid << 3) | 1U;
					case Table.FieldPtr:
					case Table.MethodPtr:
						break;
					case Table.Field:
						return (token.Rid << 3) | 6U;
					case Table.Method:
						return (token.Rid << 3) | 5U;
					default:
						if (table == Table.MemberRef)
						{
							return (token.Rid << 3) | 4U;
						}
						break;
				}
			}
			else
			{
				if (table == Table.TypeSpec)
				{
					return (token.Rid << 3) | 3U;
				}
				if (table == Table.MethodSpec)
				{
					return (token.Rid << 3) | 7U;
				}
			}
			throw new NotSupportedException();
		}

		private uint ComputeLength(VMRuntime rt)
		{
			uint len = 16u;
			foreach (KeyValuePair<IMemberRef, uint> reference in rt.Descriptor.Data.refMap)
			{
				len += Utils.GetCompressedUIntLength(reference.Value) + GetCodedLen(reference.Key.MDToken);
			}
			foreach (KeyValuePair<string, uint> str in rt.Descriptor.Data.strMap)
			{
				len += Utils.GetCompressedUIntLength(str.Value);
				len += Utils.GetCompressedUIntLength((uint)str.Key.Length);
				len += (uint)(str.Key.Length * 2);
			}
			foreach (DataDescriptor.FuncSigDesc sig in rt.Descriptor.Data.sigs)
			{
				len += Utils.GetCompressedUIntLength(sig.Id);
				len += 4;
				if (sig.Method != null)
				{
					len += 4;
				}
				uint paramCount = (uint)sig.FuncSig.ParamSigs.Length;
				len += 1 + Utils.GetCompressedUIntLength(paramCount);
				ITypeDefOrRef[] paramSigs = sig.FuncSig.ParamSigs;
				foreach (ITypeDefOrRef param in paramSigs)
				{
					len += GetCodedLen(param.MDToken);
				}
				len += GetCodedLen(sig.FuncSig.RetType.MDToken);
			}
			return len;
		}

		internal void WriteData(VMRuntime rt)
		{
			MemoryStream stream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(1752394086u);
			writer.Write(rt.Descriptor.Data.refMap.Count);
			writer.Write(rt.Descriptor.Data.strMap.Count);
			writer.Write(rt.Descriptor.Data.sigs.Count);
			foreach (KeyValuePair<IMemberRef, uint> refer in rt.Descriptor.Data.refMap)
			{
				writer.WriteCompressedUInt(refer.Value);
				writer.WriteCompressedUInt(GetCodedToken(refer.Key.MDToken));
			}
			foreach (KeyValuePair<string, uint> str in rt.Descriptor.Data.strMap)
			{
				writer.WriteCompressedUInt(str.Value);
				writer.WriteCompressedUInt((uint)str.Key.Length);
				string key2 = str.Key;
				foreach (char chr in key2)
				{
					writer.Write((ushort)chr);
				}
			}
			foreach (DataDescriptor.FuncSigDesc sig in rt.Descriptor.Data.sigs)
			{
				writer.WriteCompressedUInt(sig.Id);
				if (sig.Method != null)
				{
					ILBlock entry = rt.methodMap[sig.Method].Item2;
					uint entryOffset = entry.Content[0].Offset;
					Debug.Assert(entryOffset != 0);
					writer.Write(entryOffset);
					uint key = (uint)rt.Descriptor.Random.Next();
					key = (key << 8) | rt.Descriptor.Data.LookupInfo(sig.Method).EntryKey;
					writer.Write(key);
				}
				else
				{
					writer.Write(0u);
				}
				writer.Write(sig.FuncSig.Flags);
				writer.WriteCompressedUInt((uint)sig.FuncSig.ParamSigs.Length);
				ITypeDefOrRef[] paramSigs = sig.FuncSig.ParamSigs;
				foreach (ITypeDefOrRef paramType in paramSigs)
				{
					writer.WriteCompressedUInt(GetCodedToken(paramType.MDToken));
				}
				writer.WriteCompressedUInt(GetCodedToken(sig.FuncSig.RetType.MDToken));
			}
			data = stream.ToArray();
			Debug.Assert(data.Length == Length);
		}
	}
}
