#define DEBUG
using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class LdvirtftnHandler : ITranslationHandler
	{
		public Code ILCode => Code.Ldvirtftn;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 1);
			IRVariable retVar = tr.Context.AllocateVRegister(expr.Type.Value);
			IIROperand obj = tr.Translate(expr.Arguments[0]);
			IMethod method = (IMethod)expr.Operand;
			int methodId = (int)tr.VM.Data.GetId(method);
			int ecallId = tr.VM.Runtime.VMCall.LDFTN;
			tr.Instructions.Add(new IRInstruction(IROpCode.PUSH, obj));
			tr.Instructions.Add(new IRInstruction(IROpCode.VCALL, IRConstant.FromI4(ecallId), IRConstant.FromI4(methodId)));
			tr.Instructions.Add(new IRInstruction(IROpCode.POP, retVar));
			return retVar;
		}
	}
}
