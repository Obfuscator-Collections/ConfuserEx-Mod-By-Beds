using System;
using System.IO;
using dnlib.DotNet.Pdb;
using KoiVM.AST.IL;
using KoiVM.AST.ILAST;

namespace KoiVM.RT
{
	internal class BasicBlockSerializer
	{
		private readonly VMRuntime rt;

		public BasicBlockSerializer(VMRuntime rt)
		{
			this.rt = rt;
		}

		public uint ComputeLength(ILBlock block)
		{
			uint len = 0u;
			foreach (ILInstruction instr in block.Content)
			{
				len += ComputeLength(instr);
			}
			return len;
		}

		public uint ComputeLength(ILInstruction instr)
		{
			uint len = 2u;
			if (instr.Operand != null)
			{
				if (instr.Operand is ILRegister)
				{
					len++;
				}
				else if (instr.Operand is ILImmediate)
				{
					object value = ((ILImmediate)instr.Operand).Value;
					if (value is uint || value is int || value is float)
					{
						len += 4;
					}
					else
					{
						if (!(value is ulong) && !(value is long) && !(value is double))
						{
							throw new NotSupportedException();
						}
						len += 8;
					}
				}
				else
				{
					if (!(instr.Operand is ILRelReference))
					{
						throw new NotSupportedException();
					}
					len += 4;
				}
			}
			return len;
		}

		public uint ComputeOffset(ILBlock block, uint offset)
		{
			foreach (ILInstruction instr in block.Content)
			{
				instr.Offset = offset;
				offset += 2;
				if (instr.Operand == null)
				{
					continue;
				}
				if (instr.Operand is ILRegister)
				{
					offset++;
				}
				else if (instr.Operand is ILImmediate)
				{
					object value = ((ILImmediate)instr.Operand).Value;
					if (value is uint || value is int || value is float)
					{
						offset += 4;
						continue;
					}
					if (!(value is ulong) && !(value is long) && !(value is double))
					{
						throw new NotSupportedException();
					}
					offset += 8;
				}
				else
				{
					if (!(instr.Operand is ILRelReference))
					{
						throw new NotSupportedException();
					}
					offset += 4;
				}
			}
			return offset;
		}

		private static bool Equals(SequencePoint a, SequencePoint b)
		{
			return a.Document.Url == b.Document.Url && a.StartLine == b.StartLine;
		}

		public void WriteData(ILBlock block, BinaryWriter writer)
		{
			uint offset = 0u;
			SequencePoint prevSeq = null;
			uint prevOffset = 0u;
			foreach (ILInstruction instr in block.Content)
			{
				if (rt.dbgWriter != null && instr.IR.ILAST is ILASTExpression)
				{
					ILASTExpression expr = (ILASTExpression)instr.IR.ILAST;
					SequencePoint seq = ((expr.CILInstr == null) ? null : expr.CILInstr.SequencePoint);
					if (seq != null && seq.StartLine != 16707566 && (prevSeq == null || !Equals(seq, prevSeq)))
					{
						if (prevSeq != null)
						{
							uint len2 = offset - prevOffset;
							uint line2 = (uint)prevSeq.StartLine;
							string doc2 = prevSeq.Document.Url;
							rt.dbgWriter.AddSequencePoint(block, prevOffset, len2, doc2, line2);
						}
						prevSeq = seq;
						prevOffset = offset;
					}
				}
				writer.Write(rt.Descriptor.Architecture.OpCodes[instr.OpCode]);
				writer.Write((byte)rt.Descriptor.Random.Next());
				offset += 2;
				if (instr.Operand == null)
				{
					continue;
				}
				if (instr.Operand is ILRegister)
				{
					writer.Write(rt.Descriptor.Architecture.Registers[((ILRegister)instr.Operand).Register]);
					offset++;
					continue;
				}
				if (instr.Operand is ILImmediate)
				{
					object value = ((ILImmediate)instr.Operand).Value;
					if (value is int)
					{
						writer.Write((int)value);
						offset += 4;
					}
					else if (value is uint)
					{
						writer.Write((uint)value);
						offset += 4;
					}
					else if (value is long)
					{
						writer.Write((long)value);
						offset += 8;
					}
					else if (value is ulong)
					{
						writer.Write((ulong)value);
						offset += 8;
					}
					else if (value is float)
					{
						writer.Write((float)value);
						offset += 4;
					}
					else if (value is double)
					{
						writer.Write((double)value);
						offset += 8;
					}
					continue;
				}
				throw new NotSupportedException();
			}
			if (prevSeq != null)
			{
				uint len = offset - prevOffset;
				uint line = (uint)prevSeq.StartLine;
				string doc = prevSeq.Document.Url;
				rt.dbgWriter.AddSequencePoint(block, prevOffset, len, doc, line);
			}
		}
	}
}
