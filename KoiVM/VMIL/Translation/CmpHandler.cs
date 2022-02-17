using KoiVM.AST;
using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class CmpHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.CMP;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.PushOperand(instr.Operand1);
			tr.PushOperand(instr.Operand2);
			if (instr.Operand1.Type == ASTType.O || instr.Operand2.Type == ASTType.O)
			{
				tr.Instructions.Add(new ILInstruction(ILOpCode.CMP));
			}
			else if (instr.Operand1.Type == ASTType.I8 || instr.Operand2.Type == ASTType.I8 || instr.Operand1.Type == ASTType.Ptr || instr.Operand2.Type == ASTType.Ptr)
			{
				tr.Instructions.Add(new ILInstruction(ILOpCode.CMP_QWORD));
			}
			else if (instr.Operand1.Type == ASTType.R8 || instr.Operand2.Type == ASTType.R8)
			{
				tr.Instructions.Add(new ILInstruction(ILOpCode.CMP_R64));
			}
			else if (instr.Operand1.Type == ASTType.R4 || instr.Operand2.Type == ASTType.R4)
			{
				tr.Instructions.Add(new ILInstruction(ILOpCode.CMP_R32));
			}
			else
			{
				tr.Instructions.Add(new ILInstruction(ILOpCode.CMP_DWORD));
			}
		}
	}
}
