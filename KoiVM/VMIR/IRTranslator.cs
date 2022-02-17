#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;
using KoiVM.CFG;
using KoiVM.RT;
using KoiVM.VM;

namespace KoiVM.VMIR
{
	public class IRTranslator
	{
		private static readonly Dictionary<Code, ITranslationHandler> handlers;

		public ScopeBlock RootScope { get; private set; }

		public IRContext Context { get; }

		public VMRuntime Runtime { get; }

		public VMDescriptor VM => Runtime.Descriptor;

		public ArchDescriptor Arch => VM.Architecture;

		internal BasicBlock<ILASTTree> Block { get; private set; }

		internal IRInstrList Instructions { get; private set; }

		static IRTranslator()
		{
			handlers = new Dictionary<Code, ITranslationHandler>();
			Type[] exportedTypes = typeof(IRTranslator).Assembly.GetExportedTypes();
			foreach (Type type in exportedTypes)
			{
				if (typeof(ITranslationHandler).IsAssignableFrom(type) && !type.IsAbstract)
				{
					ITranslationHandler handler = (ITranslationHandler)Activator.CreateInstance(type);
					handlers.Add(handler.ILCode, handler);
				}
			}
		}

		public IRTranslator(IRContext ctx, VMRuntime runtime)
		{
			Context = ctx;
			Runtime = runtime;
		}

		internal IIROperand Translate(IILASTNode node)
		{
			if (node is ILASTExpression)
			{
				ILASTExpression expr = (ILASTExpression)node;
				try
				{
					if (!handlers.TryGetValue(expr.ILCode, out var handler))
					{
						throw new NotSupportedException(expr.ILCode.ToString());
					}
					int i = Instructions.Count;
					IIROperand operand = handler.Translate(expr, this);
					for (; i < Instructions.Count; i++)
					{
						Instructions[i].ILAST = expr;
					}
					return operand;
				}
				catch (Exception ex)
				{
					throw new Exception($"Failed to translate expr {expr.CILInstr} @ {expr.CILInstr.GetOffset():x4}.", ex);
				}
			}
			if (node is ILASTVariable)
			{
				return Context.ResolveVRegister((ILASTVariable)node);
			}
			throw new NotSupportedException();
		}

		private IRInstrList Translate(BasicBlock<ILASTTree> block)
		{
			Block = block;
			Instructions = new IRInstrList();
			bool seenJump = false;
			foreach (IILASTStatement st in block.Content)
			{
				if (st is ILASTPhi)
				{
					ILASTVariable variable = ((ILASTPhi)st).Variable;
					Instructions.Add(new IRInstruction(IROpCode.POP)
					{
						Operand1 = Context.ResolveVRegister(variable),
						ILAST = st
					});
					continue;
				}
				if (st is ILASTAssignment)
				{
					ILASTAssignment assignment = (ILASTAssignment)st;
					IIROperand valueVar = Translate(assignment.Value);
					Instructions.Add(new IRInstruction(IROpCode.MOV)
					{
						Operand1 = Context.ResolveVRegister(assignment.Variable),
						Operand2 = valueVar,
						ILAST = st
					});
					continue;
				}
				if (st is ILASTExpression)
				{
					ILASTExpression expr = (ILASTExpression)st;
					OpCode opCode = expr.ILCode.ToOpCode();
					if (!seenJump && (opCode.FlowControl == FlowControl.Cond_Branch || opCode.FlowControl == FlowControl.Branch || opCode.FlowControl == FlowControl.Return || opCode.FlowControl == FlowControl.Throw))
					{
						ILASTVariable[] stackRemains = block.Content.StackRemains;
						foreach (ILASTVariable remain in stackRemains)
						{
							Instructions.Add(new IRInstruction(IROpCode.PUSH)
							{
								Operand1 = Context.ResolveVRegister(remain),
								ILAST = st
							});
						}
						seenJump = true;
					}
					Translate((ILASTExpression)st);
					continue;
				}
				throw new NotSupportedException();
			}
			Debug.Assert(seenJump);
			IRInstrList ret = Instructions;
			Instructions = null;
			return ret;
		}

		public void Translate(ScopeBlock rootScope)
		{
			RootScope = rootScope;
			Dictionary<BasicBlock<ILASTTree>, BasicBlock<IRInstrList>> blockMap = rootScope.UpdateBasicBlocks((BasicBlock<ILASTTree> block) => Translate(block));
			rootScope.ProcessBasicBlocks(delegate(BasicBlock<IRInstrList> block)
			{
				foreach (IRInstruction current in block.Content)
				{
					if (current.Operand1 is IRBlockTarget)
					{
						IRBlockTarget iRBlockTarget = (IRBlockTarget)current.Operand1;
						iRBlockTarget.Target = blockMap[(BasicBlock<ILASTTree>)iRBlockTarget.Target];
					}
					else if (current.Operand1 is IRJumpTable)
					{
						IRJumpTable iRJumpTable = (IRJumpTable)current.Operand1;
						for (int i = 0; i < iRJumpTable.Targets.Length; i++)
						{
							iRJumpTable.Targets[i] = blockMap[(BasicBlock<ILASTTree>)iRJumpTable.Targets[i]];
						}
					}
					if (current.Operand2 is IRBlockTarget)
					{
						IRBlockTarget iRBlockTarget2 = (IRBlockTarget)current.Operand2;
						iRBlockTarget2.Target = blockMap[(BasicBlock<ILASTTree>)iRBlockTarget2.Target];
					}
					else if (current.Operand2 is IRJumpTable)
					{
						IRJumpTable iRJumpTable2 = (IRJumpTable)current.Operand2;
						for (int j = 0; j < iRJumpTable2.Targets.Length; j++)
						{
							iRJumpTable2.Targets[j] = blockMap[(BasicBlock<ILASTTree>)iRJumpTable2.Targets[j]];
						}
					}
				}
			});
		}
	}
}
