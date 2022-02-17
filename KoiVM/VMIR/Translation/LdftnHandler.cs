using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class LdftnHandler : ITranslationHandler
	{
		public Code ILCode => Code.Ldftn;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			IRVariable retVar = tr.Context.AllocateVRegister(expr.Type.Value);
			MethodDef method = ((IMethod)expr.Operand).ResolveMethodDef();
			bool intraLinking = method != null && tr.VM.Settings.IsVirtualized(method);
			int ecallId = tr.VM.Runtime.VMCall.LDFTN;
			if (intraLinking)
			{
				int sigId = (int)tr.VM.Data.GetId(method.DeclaringType, method.MethodSig);
				uint entryKey = tr.VM.Data.LookupInfo(method).EntryKey;
				entryKey = ((uint)tr.VM.Random.Next() & 0xFFFFFF00u) | entryKey;
				tr.Instructions.Add(new IRInstruction(IROpCode.PUSH, IRConstant.FromI4((int)entryKey)));
				tr.Instructions.Add(new IRInstruction(IROpCode.PUSH, IRConstant.FromI4(sigId)));
				tr.Instructions.Add(new IRInstruction(IROpCode.PUSH, new IRMetaTarget(method)
				{
					LateResolve = true
				}));
				tr.Instructions.Add(new IRInstruction(IROpCode.VCALL, IRConstant.FromI4(ecallId)));
			}
			else
			{
				int methodId = (int)tr.VM.Data.GetId((IMethod)expr.Operand);
				tr.Instructions.Add(new IRInstruction(IROpCode.PUSH, IRConstant.FromI4(0)));
				tr.Instructions.Add(new IRInstruction(IROpCode.VCALL, IRConstant.FromI4(ecallId), IRConstant.FromI4(methodId)));
			}
			tr.Instructions.Add(new IRInstruction(IROpCode.POP, retVar));
			return retVar;
		}
	}
}
