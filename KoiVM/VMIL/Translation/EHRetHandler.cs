using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class EHRetHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.__EHRET;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			if (instr.Operand1 != null)
			{
				tr.PushOperand(instr.Operand1);
				tr.Instructions.Add(new ILInstruction(ILOpCode.POP, ILRegister.R0));
			}
			tr.Instructions.Add(new ILInstruction(ILOpCode.RET));
		}
	}
}
