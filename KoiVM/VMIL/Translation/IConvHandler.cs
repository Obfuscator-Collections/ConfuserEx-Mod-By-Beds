#define DEBUG
using System;
using System.Diagnostics;
using KoiVM.AST;
using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class IConvHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.ICONV;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.PushOperand(instr.Operand2);
			Debug.Assert(instr.Operand1.Type == ASTType.I8);
			switch (instr.Operand2.Type)
			{
			case ASTType.R4:
				tr.Instructions.Add(new ILInstruction(ILOpCode.FCONV_R32_R64));
				tr.Instructions.Add(new ILInstruction(ILOpCode.ICONV_R64));
				break;
			case ASTType.R8:
				tr.Instructions.Add(new ILInstruction(ILOpCode.ICONV_R64));
				break;
			case ASTType.Ptr:
				tr.Instructions.Add(new ILInstruction(ILOpCode.ICONV_PTR));
				break;
			default:
				throw new NotSupportedException();
			}
			tr.PopOperand(instr.Operand1);
		}
	}
}
