using System;
using dnlib.DotNet;
using KoiVM.AST;
using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.CFG;
using KoiVM.RT;

namespace KoiVM.VMIL
{
	public static class TranslationHelpers
	{
		public static ILOpCode GetLIND(ASTType type, TypeSig rawType)
		{
			if (rawType != null)
			{
				switch (rawType.ElementType)
				{
				case ElementType.Boolean:
				case ElementType.I1:
				case ElementType.U1:
					return ILOpCode.LIND_BYTE;
				case ElementType.Char:
				case ElementType.I2:
				case ElementType.U2:
					return ILOpCode.LIND_WORD;
				case ElementType.I4:
				case ElementType.U4:
				case ElementType.R4:
					return ILOpCode.LIND_DWORD;
				case ElementType.I8:
				case ElementType.U8:
				case ElementType.R8:
					return ILOpCode.LIND_QWORD;
				case ElementType.Ptr:
				case ElementType.I:
				case ElementType.U:
					return ILOpCode.LIND_PTR;
				default:
					return ILOpCode.LIND_OBJECT;
				}
			}
			switch (type)
			{
			case ASTType.I4:
			case ASTType.R4:
				return ILOpCode.LIND_DWORD;
			case ASTType.I8:
			case ASTType.R8:
				return ILOpCode.LIND_QWORD;
			case ASTType.Ptr:
				return ILOpCode.LIND_PTR;
			default:
				return ILOpCode.LIND_OBJECT;
			}
		}

		public static ILOpCode GetLIND(this IRRegister reg)
		{
			return GetLIND(reg.Type, (reg.SourceVariable == null) ? null : reg.SourceVariable.RawType);
		}

		public static ILOpCode GetLIND(this IRPointer ptr)
		{
			return GetLIND(ptr.Type, (ptr.SourceVariable == null) ? null : ptr.SourceVariable.RawType);
		}

		public static ILOpCode GetSIND(ASTType type, TypeSig rawType)
		{
			if (rawType != null)
			{
				switch (rawType.ElementType)
				{
				case ElementType.Boolean:
				case ElementType.I1:
				case ElementType.U1:
					return ILOpCode.SIND_BYTE;
				case ElementType.Char:
				case ElementType.I2:
				case ElementType.U2:
					return ILOpCode.SIND_WORD;
				case ElementType.I4:
				case ElementType.U4:
				case ElementType.R4:
					return ILOpCode.SIND_DWORD;
				case ElementType.I8:
				case ElementType.U8:
				case ElementType.R8:
					return ILOpCode.SIND_QWORD;
				case ElementType.Ptr:
				case ElementType.I:
				case ElementType.U:
					return ILOpCode.SIND_PTR;
				default:
					return ILOpCode.SIND_OBJECT;
				}
			}
			switch (type)
			{
			case ASTType.I4:
			case ASTType.R4:
				return ILOpCode.SIND_DWORD;
			case ASTType.I8:
			case ASTType.R8:
				return ILOpCode.SIND_QWORD;
			case ASTType.Ptr:
				return ILOpCode.SIND_PTR;
			default:
				return ILOpCode.SIND_OBJECT;
			}
		}

		public static ILOpCode GetSIND(this IRRegister reg)
		{
			return GetSIND(reg.Type, (reg.SourceVariable == null) ? null : reg.SourceVariable.RawType);
		}

		public static ILOpCode GetSIND(this IRPointer ptr)
		{
			return GetSIND(ptr.Type, (ptr.SourceVariable == null) ? null : ptr.SourceVariable.RawType);
		}

		public static ILOpCode GetPUSHR(ASTType type, TypeSig rawType)
		{
			if (rawType != null)
			{
				switch (rawType.ElementType)
				{
				case ElementType.Boolean:
				case ElementType.I1:
				case ElementType.U1:
					return ILOpCode.PUSHR_BYTE;
				case ElementType.Char:
				case ElementType.I2:
				case ElementType.U2:
					return ILOpCode.PUSHR_WORD;
				case ElementType.I4:
				case ElementType.U4:
				case ElementType.R4:
					return ILOpCode.PUSHR_DWORD;
				case ElementType.I8:
				case ElementType.U8:
				case ElementType.R8:
				case ElementType.Ptr:
					return ILOpCode.PUSHR_QWORD;
				default:
					return ILOpCode.PUSHR_OBJECT;
				}
			}
			switch (type)
			{
			case ASTType.I4:
			case ASTType.R4:
				return ILOpCode.PUSHR_DWORD;
			case ASTType.I8:
			case ASTType.R8:
			case ASTType.Ptr:
				return ILOpCode.PUSHR_QWORD;
			default:
				return ILOpCode.PUSHR_OBJECT;
			}
		}

		public static ILOpCode GetPUSHR(this IRRegister reg)
		{
			return GetPUSHR(reg.Type, (reg.SourceVariable == null) ? null : reg.SourceVariable.RawType);
		}

		public static ILOpCode GetPUSHR(this IRPointer ptr)
		{
			return GetPUSHR(ptr.Type, (ptr.SourceVariable == null) ? null : ptr.SourceVariable.RawType);
		}

		public static ILOpCode GetPUSHI(this ASTType type)
		{
			switch (type)
			{
			case ASTType.I4:
			case ASTType.R4:
				return ILOpCode.PUSHI_DWORD;
			case ASTType.I8:
			case ASTType.R8:
			case ASTType.Ptr:
				return ILOpCode.PUSHI_QWORD;
			default:
				throw new NotSupportedException();
			}
		}

		public static void PushOperand(this ILTranslator tr, IIROperand operand)
		{
			if (operand is IRRegister)
			{
				ILRegister reg = ILRegister.LookupRegister(((IRRegister)operand).Register);
				tr.Instructions.Add(new ILInstruction(((IRRegister)operand).GetPUSHR(), reg));
			}
			else if (operand is IRPointer)
			{
				IRPointer pointer = (IRPointer)operand;
				ILRegister reg2 = ILRegister.LookupRegister(pointer.Register.Register);
				tr.Instructions.Add(new ILInstruction(pointer.Register.GetPUSHR(), reg2));
				if (pointer.Offset != 0)
				{
					tr.Instructions.Add(new ILInstruction(ILOpCode.PUSHI_DWORD, ILImmediate.Create(pointer.Offset, ASTType.I4)));
					if (pointer.Register.Type == ASTType.I4)
					{
						tr.Instructions.Add(new ILInstruction(ILOpCode.ADD_DWORD));
					}
					else
					{
						tr.Instructions.Add(new ILInstruction(ILOpCode.ADD_QWORD));
					}
				}
				tr.Instructions.Add(new ILInstruction(pointer.GetLIND()));
			}
			else if (operand is IRConstant)
			{
				IRConstant constant = (IRConstant)operand;
				if (constant.Value == null)
				{
					tr.Instructions.Add(new ILInstruction(ILOpCode.PUSHI_DWORD, ILImmediate.Create(0, ASTType.O)));
				}
				else
				{
					tr.Instructions.Add(new ILInstruction(constant.Type.Value.GetPUSHI(), ILImmediate.Create(constant.Value, constant.Type.Value)));
				}
			}
			else if (operand is IRMetaTarget)
			{
				MethodDef method = (MethodDef)((IRMetaTarget)operand).MetadataItem;
				tr.Instructions.Add(new ILInstruction(ILOpCode.PUSHI_DWORD, new ILMethodTarget(method)));
			}
			else if (operand is IRBlockTarget)
			{
				IBasicBlock target2 = ((IRBlockTarget)operand).Target;
				tr.Instructions.Add(new ILInstruction(ILOpCode.PUSHI_DWORD, new ILBlockTarget(target2)));
			}
			else if (operand is IRJumpTable)
			{
				IBasicBlock[] targets = ((IRJumpTable)operand).Targets;
				tr.Instructions.Add(new ILInstruction(ILOpCode.PUSHI_DWORD, new ILJumpTable(targets)));
			}
			else
			{
				if (!(operand is IRDataTarget))
				{
					throw new NotSupportedException();
				}
				BinaryChunk target = ((IRDataTarget)operand).Target;
				tr.Instructions.Add(new ILInstruction(ILOpCode.PUSHI_DWORD, new ILDataTarget(target)));
			}
		}

		public static void PopOperand(this ILTranslator tr, IIROperand operand)
		{
			if (operand is IRRegister)
			{
				ILRegister reg2 = ILRegister.LookupRegister(((IRRegister)operand).Register);
				tr.Instructions.Add(new ILInstruction(ILOpCode.POP, reg2));
				return;
			}
			if (operand is IRPointer)
			{
				IRPointer pointer = (IRPointer)operand;
				ILRegister reg = ILRegister.LookupRegister(pointer.Register.Register);
				tr.Instructions.Add(new ILInstruction(pointer.Register.GetPUSHR(), reg));
				if (pointer.Offset != 0)
				{
					tr.Instructions.Add(new ILInstruction(ILOpCode.PUSHI_DWORD, ILImmediate.Create(pointer.Offset, ASTType.I4)));
					if (pointer.Register.Type == ASTType.I4)
					{
						tr.Instructions.Add(new ILInstruction(ILOpCode.ADD_DWORD));
					}
					else
					{
						tr.Instructions.Add(new ILInstruction(ILOpCode.ADD_QWORD));
					}
				}
				tr.Instructions.Add(new ILInstruction(pointer.GetSIND()));
				return;
			}
			throw new NotSupportedException();
		}
	}
}
