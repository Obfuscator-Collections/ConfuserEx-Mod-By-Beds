#define DEBUG
using System;
using System.Diagnostics;
using KoiVM.AST;
using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class FConvHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.FCONV;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.PushOperand(instr.Operand2);
			switch (instr.Operand2.Type)
			{
			case ASTType.R4:
				Debug.Assert(instr.Operand1.Type == ASTType.R8);
				tr.Instructions.Add(new ILInstruction(ILOpCode.FCONV_R32_R64));
				break;
			case ASTType.R8:
				Debug.Assert(instr.Operand1.Type == ASTType.R4);
				tr.Instructions.Add(new ILInstruction(ILOpCode.FCONV_R64_R32));
				break;
			default:
				Debug.Assert(instr.Operand2.Type == ASTType.I8);
				switch (instr.Operand1.Type)
				{
				case ASTType.R4:
					tr.Instructions.Add(new ILInstruction(ILOpCode.FCONV_R32));
					break;
				case ASTType.R8:
					tr.Instructions.Add(new ILInstruction(ILOpCode.FCONV_R64));
					break;
				default:
					throw new NotSupportedException();
				}
				break;
			}
			tr.PopOperand(instr.Operand1);
		}
	}
}
