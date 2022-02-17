#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.IR;
using KoiVM.CFG;

namespace KoiVM.VMIR.Transforms
{
	public class EHTransform : ITransform
	{
		private ScopeBlock[] thisScopes;

		public void Initialize(IRTransformer tr)
		{
		}

		public void Transform(IRTransformer tr)
		{
			thisScopes = tr.RootScope.SearchBlock(tr.Block);
			AddTryStart(tr);
			if (thisScopes[thisScopes.Length - 1].Type == ScopeType.Handler)
			{
				ScopeBlock tryScope = SearchForTry(tr.RootScope, thisScopes[thisScopes.Length - 1].ExceptionHandler);
				ScopeBlock[] scopes = tr.RootScope.SearchBlock(tryScope.GetBasicBlocks().First());
				thisScopes = scopes.TakeWhile((ScopeBlock s) => s != tryScope).ToArray();
			}
			tr.Instructions.VisitInstrs(VisitInstr, tr);
		}

		private void SearchForHandlers(ScopeBlock scope, ExceptionHandler eh, ref IBasicBlock handler, ref IBasicBlock filter)
		{
			if (scope.ExceptionHandler == eh)
			{
				if (scope.Type == ScopeType.Handler)
				{
					handler = scope.GetBasicBlocks().First();
				}
				else if (scope.Type == ScopeType.Filter)
				{
					filter = scope.GetBasicBlocks().First();
				}
			}
			foreach (ScopeBlock child in scope.Children)
			{
				SearchForHandlers(child, eh, ref handler, ref filter);
			}
		}

		private void AddTryStart(IRTransformer tr)
		{
			List<IRInstruction> tryStartInstrs = new List<IRInstruction>();
			for (int i = 0; i < thisScopes.Length; i++)
			{
				ScopeBlock scope = thisScopes[i];
				if (scope.Type != ScopeType.Try || scope.GetBasicBlocks().First() != tr.Block)
				{
					continue;
				}
				IBasicBlock handler = null;
				IBasicBlock filter = null;
				SearchForHandlers(tr.RootScope, scope.ExceptionHandler, ref handler, ref filter);
				Debug.Assert(handler != null && (scope.ExceptionHandler.HandlerType != ExceptionHandlerType.Filter || filter != null));
				tryStartInstrs.Add(new IRInstruction(IROpCode.PUSH, new IRBlockTarget(handler)));
				IIROperand tryOperand = null;
				int ehType;
				if (scope.ExceptionHandler.HandlerType == ExceptionHandlerType.Catch)
				{
					tryOperand = IRConstant.FromI4((int)tr.VM.Data.GetId(scope.ExceptionHandler.CatchType));
					ehType = tr.VM.Runtime.RTFlags.EH_CATCH;
				}
				else if (scope.ExceptionHandler.HandlerType == ExceptionHandlerType.Filter)
				{
					tryOperand = new IRBlockTarget(filter);
					ehType = tr.VM.Runtime.RTFlags.EH_FILTER;
				}
				else if (scope.ExceptionHandler.HandlerType == ExceptionHandlerType.Fault)
				{
					ehType = tr.VM.Runtime.RTFlags.EH_FAULT;
				}
				else
				{
					if (scope.ExceptionHandler.HandlerType != ExceptionHandlerType.Finally)
					{
						throw new InvalidProgramException();
					}
					ehType = tr.VM.Runtime.RTFlags.EH_FINALLY;
				}
				tryStartInstrs.Add(new IRInstruction(IROpCode.TRY, IRConstant.FromI4(ehType), tryOperand)
				{
					Annotation = new EHInfo(scope.ExceptionHandler)
				});
			}
			tr.Instructions.InsertRange(0, tryStartInstrs);
		}

		private ScopeBlock SearchForTry(ScopeBlock scope, ExceptionHandler eh)
		{
			if (scope.ExceptionHandler == eh && scope.Type == ScopeType.Try)
			{
				return scope;
			}
			foreach (ScopeBlock child in scope.Children)
			{
				ScopeBlock s = SearchForTry(child, eh);
				if (s != null)
				{
					return s;
				}
			}
			return null;
		}

		private static ScopeBlock FindCommonAncestor(ScopeBlock[] a, ScopeBlock[] b)
		{
			ScopeBlock ret = null;
			for (int i = 0; i < a.Length && i < b.Length && a[i] == b[i]; i++)
			{
				ret = a[i];
			}
			return ret;
		}

		private void VisitInstr(IRInstrList instrs, IRInstruction instr, ref int index, IRTransformer tr)
		{
			if (instr.OpCode != IROpCode.__LEAVE)
			{
				return;
			}
			ScopeBlock[] targetScopes = tr.RootScope.SearchBlock(((IRBlockTarget)instr.Operand1).Target);
			ScopeBlock escapeTarget = FindCommonAncestor(thisScopes, targetScopes);
			List<IRInstruction> leaveInstrs = new List<IRInstruction>();
			int i = thisScopes.Length - 1;
			while (i >= 0 && thisScopes[i] != escapeTarget)
			{
				if (thisScopes[i].Type == ScopeType.Try)
				{
					IBasicBlock handler = null;
					IBasicBlock filter = null;
					SearchForHandlers(tr.RootScope, thisScopes[i].ExceptionHandler, ref handler, ref filter);
					if (handler == null)
					{
						throw new InvalidProgramException();
					}
					leaveInstrs.Add(new IRInstruction(IROpCode.LEAVE, new IRBlockTarget(handler))
					{
						Annotation = new EHInfo(thisScopes[i].ExceptionHandler)
					});
				}
				i--;
			}
			instr.OpCode = IROpCode.JMP;
			leaveInstrs.Add(instr);
			instrs.Replace(index, leaveInstrs);
		}
	}
}
