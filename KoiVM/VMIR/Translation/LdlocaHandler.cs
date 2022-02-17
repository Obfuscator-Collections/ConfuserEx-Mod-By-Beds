using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class LdlocaHandler : ITranslationHandler
	{
		public Code ILCode => Code.Ldloca;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			IRVariable local = tr.Context.ResolveLocal((Local)expr.Operand);
			IRVariable ret = tr.Context.AllocateVRegister(ASTType.ByRef);
			tr.Instructions.Add(new IRInstruction(IROpCode.__LEA, ret, local));
			return ret;
		}
	}
}
