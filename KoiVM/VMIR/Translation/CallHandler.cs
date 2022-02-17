using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class CallHandler : ITranslationHandler
	{
		public Code ILCode => Code.Call;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			InstrCallInfo callInfo = new InstrCallInfo("CALL")
			{
				Method = (IMethod)expr.Operand
			};
			tr.Instructions.Add(new IRInstruction(IROpCode.__BEGINCALL)
			{
				Annotation = callInfo
			});
			IIROperand[] args = new IIROperand[expr.Arguments.Length];
			for (int i = 0; i < args.Length; i++)
			{
				args[i] = tr.Translate(expr.Arguments[i]);
				tr.Instructions.Add(new IRInstruction(IROpCode.PUSH)
				{
					Operand1 = args[i],
					Annotation = callInfo
				});
			}
			callInfo.Arguments = args;
			IIROperand retVal = null;
			if (expr.Type.HasValue)
			{
				retVal = tr.Context.AllocateVRegister(expr.Type.Value);
				tr.Instructions.Add(new IRInstruction(IROpCode.__CALL)
				{
					Operand1 = new IRMetaTarget(callInfo.Method),
					Operand2 = retVal,
					Annotation = callInfo
				});
			}
			else
			{
				tr.Instructions.Add(new IRInstruction(IROpCode.__CALL)
				{
					Operand1 = new IRMetaTarget(callInfo.Method),
					Annotation = callInfo
				});
			}
			callInfo.ReturnValue = retVal;
			tr.Instructions.Add(new IRInstruction(IROpCode.__ENDCALL)
			{
				Annotation = callInfo
			});
			return retVal;
		}
	}
}
