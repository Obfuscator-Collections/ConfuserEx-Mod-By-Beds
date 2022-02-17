#define DEBUG
using System.Diagnostics;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.IL;
using KoiVM.VM;
using KoiVM.VMIL;

namespace KoiVM.RT
{
	internal class BasicBlockChunk : IKoiChunk
	{
		private readonly MethodDef method;

		private readonly VMRuntime rt;

		public ILBlock Block { get; set; }

		public uint Length { get; set; }

		public BasicBlockChunk(VMRuntime rt, MethodDef method, ILBlock block)
		{
			this.rt = rt;
			this.method = method;
			Block = block;
			Length = rt.serializer.ComputeLength(block);
		}

		public void OnOffsetComputed(uint offset)
		{
			uint len = rt.serializer.ComputeOffset(Block, offset);
			Debug.Assert(len - offset == Length);
		}

		public byte[] GetData()
		{
			MemoryStream stream = new MemoryStream();
			rt.serializer.WriteData(Block, new BinaryWriter(stream));
			return Encrypt(stream.ToArray());
		}

		private byte[] Encrypt(byte[] data)
		{
			VMBlockKey blockKey = rt.Descriptor.Data.LookupInfo(method).BlockKeys[Block];
			byte currentKey = blockKey.EntryKey;
			ILInstruction firstInstr = Block.Content[0];
			ILInstruction lastInstr = Block.Content[Block.Content.Count - 1];
			foreach (ILInstruction instr in Block.Content)
			{
				uint instrStart = instr.Offset - firstInstr.Offset;
				uint instrEnd = instrStart + rt.serializer.ComputeLength(instr);
				byte b2 = data[instrStart];
				data[instrStart] ^= currentKey;
				currentKey = (byte)(currentKey * 7 + b2);
				byte? fixupTarget = null;
				if (instr.Annotation == InstrAnnotation.JUMP || instr == lastInstr)
				{
					fixupTarget = blockKey.ExitKey;
				}
				else if (instr.OpCode == ILOpCode.LEAVE)
				{
					ExceptionHandler eh = ((EHInfo)instr.Annotation).ExceptionHandler;
					if (eh.HandlerType == ExceptionHandlerType.Finally)
					{
						fixupTarget = blockKey.ExitKey;
					}
				}
				else if (instr.OpCode == ILOpCode.CALL)
				{
					InstrCallInfo callInfo2 = (InstrCallInfo)instr.Annotation;
					VMMethodInfo info2 = rt.Descriptor.Data.LookupInfo((MethodDef)callInfo2.Method);
					fixupTarget = info2.EntryKey;
				}
				if (fixupTarget.HasValue)
				{
					byte fixup = (data[instrStart + 1] = CalculateFixupByte(fixupTarget.Value, data, currentKey, instrStart + 1, instrEnd));
				}
				for (uint i = instrStart + 1; i < instrEnd; i++)
				{
					byte b = data[i];
					data[i] ^= currentKey;
					currentKey = (byte)(currentKey * 7 + b);
				}
				if (fixupTarget.HasValue)
				{
					Debug.Assert(currentKey == fixupTarget.Value);
				}
				if (instr.OpCode == ILOpCode.CALL)
				{
					InstrCallInfo callInfo = (InstrCallInfo)instr.Annotation;
					VMMethodInfo info = rt.Descriptor.Data.LookupInfo((MethodDef)callInfo.Method);
					currentKey = info.ExitKey;
				}
			}
			return data;
		}

		private static byte CalculateFixupByte(byte target, byte[] data, uint currentKey, uint rangeStart, uint rangeEnd)
		{
			byte fixupByte = target;
			for (uint i = rangeEnd - 1; i > rangeStart; i--)
			{
				fixupByte = (byte)((fixupByte - data[i]) * 183);
			}
			return (byte)(fixupByte - (byte)(currentKey * 7));
		}
	}
}
