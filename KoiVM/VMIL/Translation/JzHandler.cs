using KoiVM.AST;
using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class JzHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.JZ;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.PushOperand(instr.Operand2);
			tr.PushOperand(instr.Operand1);
			tr.Instructions.Add(new ILInstruction(ILOpCode.JZ)
			{
				Annotation = InstrAnnotation.JUMP
			});
		}
	}
}
