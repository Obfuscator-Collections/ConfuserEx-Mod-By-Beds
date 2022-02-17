#define DEBUG
using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class StsfldHandler : ITranslationHandler
	{
		public Code ILCode => Code.Stsfld;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 1);
			IIROperand val = tr.Translate(expr.Arguments[0]);
			int fieldId = (int)tr.VM.Data.GetId((IField)expr.Operand);
			int ecallId = tr.VM.Runtime.VMCall.STFLD;
			tr.Instructions.Add(new IRInstruction(IROpCode.PUSH, IRConstant.Null()));
			tr.Instructions.Add(new IRInstruction(IROpCode.PUSH, val));
			tr.Instructions.Add(new IRInstruction(IROpCode.VCALL, IRConstant.FromI4(ecallId), IRConstant.FromI4(fieldId)));
			return null;
		}
	}
}
