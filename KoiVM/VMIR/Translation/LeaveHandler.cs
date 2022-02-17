using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;
using KoiVM.CFG;

namespace KoiVM.VMIR.Translation
{
	public class LeaveHandler : ITranslationHandler
	{
		public Code ILCode => Code.Leave;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			tr.Instructions.Add(new IRInstruction(IROpCode.__LEAVE)
			{
				Operand1 = new IRBlockTarget((IBasicBlock)expr.Operand)
			});
			tr.Block.Flags |= BlockFlags.ExitEHLeave;
			return null;
		}
	}
}
