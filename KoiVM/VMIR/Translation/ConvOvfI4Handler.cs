#define DEBUG
using System;
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class ConvOvfI4Handler : ITranslationHandler
	{
		public Code ILCode => Code.Conv_Ovf_I4;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 1);
			IIROperand value = tr.Translate(expr.Arguments[0]);
			ASTType valueType = value.Type;
			if (valueType == ASTType.I4)
			{
				return value;
			}
			IRVariable retVar = tr.Context.AllocateVRegister(ASTType.I4);
			int rangechk = tr.VM.Runtime.VMCall.RANGECHK;
			int ckovf = tr.VM.Runtime.VMCall.CKOVERFLOW;
			switch (valueType)
			{
			case ASTType.I8:
			case ASTType.Ptr:
				tr.Instructions.Add(new IRInstruction(IROpCode.PUSH, IRConstant.FromI8(-2147483648L)));
				tr.Instructions.Add(new IRInstruction(IROpCode.PUSH, IRConstant.FromI8(2147483647L)));
				tr.Instructions.Add(new IRInstruction(IROpCode.VCALL, IRConstant.FromI4(rangechk), value));
				tr.Instructions.Add(new IRInstruction(IROpCode.VCALL, IRConstant.FromI4(ckovf)));
				tr.Instructions.Add(new IRInstruction(IROpCode.MOV, retVar, value));
				return retVar;
			case ASTType.R4:
			case ASTType.R8:
			{
				IRVariable tmpVar = tr.Context.AllocateVRegister(ASTType.I8);
				IRVariable fl = tr.Context.AllocateVRegister(ASTType.I4);
				tr.Instructions.Add(new IRInstruction(IROpCode.ICONV, tmpVar, value));
				tr.Instructions.Add(new IRInstruction(IROpCode.__GETF)
				{
					Operand1 = fl,
					Operand2 = IRConstant.FromI4(1 << tr.Arch.Flags.OVERFLOW)
				});
				tr.Instructions.Add(new IRInstruction(IROpCode.VCALL, IRConstant.FromI4(ckovf), fl));
				value = tmpVar;
				goto case ASTType.I8;
			}
			default:
				throw new NotSupportedException();
			}
		}
	}
}
