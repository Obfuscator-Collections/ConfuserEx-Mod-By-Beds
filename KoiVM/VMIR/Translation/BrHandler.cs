using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;
using KoiVM.CFG;

namespace KoiVM.VMIR.Translation
{
	public class BrHandler : ITranslationHandler
	{
		public Code ILCode => Code.Br;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			tr.Instructions.Add(new IRInstruction(IROpCode.JMP)
			{
				Operand1 = new IRBlockTarget((IBasicBlock)expr.Operand)
			});
			return null;
		}
	}
}
