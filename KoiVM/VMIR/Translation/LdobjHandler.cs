#define DEBUG
using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class LdobjHandler : ITranslationHandler
	{
		public Code ILCode => Code.Ldobj;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 1);
			IIROperand addr = tr.Translate(expr.Arguments[0]);
			IRVariable retVar = tr.Context.AllocateVRegister(expr.Type.Value);
			tr.Instructions.Add(new IRInstruction(IROpCode.__LDOBJ, addr, retVar)
			{
				Annotation = new PointerInfo("LDOBJ", (ITypeDefOrRef)expr.Operand)
			});
			return retVar;
		}
	}
}
