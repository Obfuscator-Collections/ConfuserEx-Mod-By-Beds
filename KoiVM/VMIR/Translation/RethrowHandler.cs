#define DEBUG
using System;
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;
using KoiVM.CFG;

namespace KoiVM.VMIR.Translation
{
	public class RethrowHandler : ITranslationHandler
	{
		public Code ILCode => Code.Rethrow;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 0);
			ScopeBlock[] parentScopes = tr.RootScope.SearchBlock(tr.Block);
			ScopeBlock catchScope = parentScopes[parentScopes.Length - 1];
			if (catchScope.Type != ScopeType.Handler || catchScope.ExceptionHandler.HandlerType != 0)
			{
				throw new InvalidProgramException();
			}
			IRVariable exVar = tr.Context.ResolveExceptionVar(catchScope.ExceptionHandler);
			Debug.Assert(exVar != null);
			int ecallId = tr.VM.Runtime.VMCall.THROW;
			tr.Instructions.Add(new IRInstruction(IROpCode.PUSH, exVar));
			tr.Instructions.Add(new IRInstruction(IROpCode.VCALL)
			{
				Operand1 = IRConstant.FromI4(ecallId),
				Operand2 = IRConstant.FromI4(1)
			});
			return null;
		}
	}
}
