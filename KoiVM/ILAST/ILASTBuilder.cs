#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;
using KoiVM.CFG;

namespace KoiVM.ILAST
{
	public class ILASTBuilder
	{
		private struct BlockState
		{
			public ILASTVariable[] BeginStack;

			public ILASTTree ASTTree;
		}

		private readonly IList<BasicBlock<CILInstrList>> basicBlocks;

		private readonly Dictionary<Instruction, BasicBlock<CILInstrList>> blockHeaders;

		private readonly Dictionary<BasicBlock<CILInstrList>, BlockState> blockStates;

		private readonly CilBody body;

		private readonly List<ILASTExpression> instrReferences;

		private readonly MethodDef method;

		private readonly ScopeBlock scope;

		private ILASTBuilder(MethodDef method, CilBody body, ScopeBlock scope)
		{
			this.method = method;
			this.body = body;
			this.scope = scope;
			basicBlocks = scope.GetBasicBlocks().Cast<BasicBlock<CILInstrList>>().ToList();
			blockHeaders = basicBlocks.ToDictionary((BasicBlock<CILInstrList> block) => block.Content[0], (BasicBlock<CILInstrList> block) => block);
			blockStates = new Dictionary<BasicBlock<CILInstrList>, BlockState>();
			instrReferences = new List<ILASTExpression>();
			Debug.Assert(basicBlocks.Count > 0);
		}

		public static void BuildAST(MethodDef method, CilBody body, ScopeBlock scope)
		{
			ILASTBuilder builder = new ILASTBuilder(method, body, scope);
			List<BasicBlock<CILInstrList>> basicBlocks = scope.GetBasicBlocks().Cast<BasicBlock<CILInstrList>>().ToList();
			builder.BuildAST();
		}

		private void BuildAST()
		{
			BuildASTInternal();
			BuildPhiNodes();
			Dictionary<BasicBlock<CILInstrList>, BasicBlock<ILASTTree>> blockMap = scope.UpdateBasicBlocks((BasicBlock<CILInstrList> block) => blockStates[block].ASTTree);
			Dictionary<Instruction, BasicBlock<ILASTTree>> newBlockMap = blockHeaders.ToDictionary((KeyValuePair<Instruction, BasicBlock<CILInstrList>> pair) => pair.Key, (KeyValuePair<Instruction, BasicBlock<CILInstrList>> pair) => blockMap[pair.Value]);
			foreach (ILASTExpression expr in instrReferences)
			{
				if (expr.Operand is Instruction)
				{
					expr.Operand = newBlockMap[(Instruction)expr.Operand];
					continue;
				}
				expr.Operand = ((IEnumerable<Instruction>)(Instruction[])expr.Operand).Select((Func<Instruction, IBasicBlock>)((Instruction instr) => newBlockMap[instr])).ToArray();
			}
		}

		private void BuildASTInternal()
		{
			Stack<BasicBlock<CILInstrList>> workList = new Stack<BasicBlock<CILInstrList>>();
			PopulateBeginStates(workList);
			HashSet<BasicBlock<CILInstrList>> visited = new HashSet<BasicBlock<CILInstrList>>();
			while (workList.Count > 0)
			{
				BasicBlock<CILInstrList> block = workList.Pop();
				if (visited.Contains(block))
				{
					continue;
				}
				visited.Add(block);
				Debug.Assert(blockStates.ContainsKey(block));
				BlockState state = blockStates[block];
				Debug.Assert(state.ASTTree == null);
				ILASTTree tree = BuildAST(block.Content, state.BeginStack);
				ILASTVariable[] remains = tree.StackRemains;
				state.ASTTree = tree;
				blockStates[block] = state;
				foreach (BasicBlock<CILInstrList> successor in block.Targets)
				{
					if (!blockStates.TryGetValue(successor, out var successorState))
					{
						ILASTVariable[] blockVars = new ILASTVariable[remains.Length];
						for (int i = 0; i < blockVars.Length; i++)
						{
							blockVars[i] = new ILASTVariable
							{
								Name = $"ph_{successor.Id:x2}_{i:x2}",
								Type = remains[i].Type,
								VariableType = ILASTVariableType.PhiVar
							};
						}
						BlockState blockState = default(BlockState);
						blockState.BeginStack = blockVars;
						successorState = blockState;
						blockStates[successor] = successorState;
					}
					else if (successorState.BeginStack.Length != remains.Length)
					{
						throw new InvalidProgramException("Inconsistent stack depth.");
					}
					workList.Push(successor);
				}
			}
		}

		private void PopulateBeginStates(Stack<BasicBlock<CILInstrList>> workList)
		{
			BlockState value;
			for (int i = 0; i < body.ExceptionHandlers.Count; i++)
			{
				ExceptionHandler eh = body.ExceptionHandlers[i];
				value = (blockStates[blockHeaders[eh.TryStart]] = new BlockState
				{
					BeginStack = new ILASTVariable[0]
				});
				BasicBlock<CILInstrList> handlerBlock = blockHeaders[eh.HandlerStart];
				workList.Push(handlerBlock);
				if (eh.HandlerType == ExceptionHandlerType.Fault || eh.HandlerType == ExceptionHandlerType.Finally)
				{
					Dictionary<BasicBlock<CILInstrList>, BlockState> dictionary = blockStates;
					value = new BlockState
					{
						BeginStack = new ILASTVariable[0]
					};
					dictionary[handlerBlock] = value;
					continue;
				}
				ASTType type = TypeInference.ToASTType(eh.CatchType.ToTypeSig());
				if (!blockStates.ContainsKey(handlerBlock))
				{
					ILASTVariable exVar = new ILASTVariable
					{
						Name = $"ex_{i:x2}",
						Type = type,
						VariableType = ILASTVariableType.ExceptionVar,
						Annotation = eh
					};
					Dictionary<BasicBlock<CILInstrList>, BlockState> dictionary2 = blockStates;
					value = new BlockState
					{
						BeginStack = new ILASTVariable[1] { exVar }
					};
					dictionary2[handlerBlock] = value;
				}
				else
				{
					Debug.Assert(blockStates[handlerBlock].BeginStack.Length == 1);
				}
				if (eh.FilterStart != null)
				{
					ILASTVariable filterVar = new ILASTVariable
					{
						Name = $"ef_{i:x2}",
						Type = type,
						VariableType = ILASTVariableType.FilterVar,
						Annotation = eh
					};
					BasicBlock<CILInstrList> filterBlock = blockHeaders[eh.FilterStart];
					workList.Push(filterBlock);
					Dictionary<BasicBlock<CILInstrList>, BlockState> dictionary3 = blockStates;
					value = new BlockState
					{
						BeginStack = new ILASTVariable[1] { filterVar }
					};
					dictionary3[filterBlock] = value;
				}
			}
			value = (blockStates[basicBlocks[0]] = new BlockState
			{
				BeginStack = new ILASTVariable[0]
			});
			workList.Push(basicBlocks[0]);
			foreach (BasicBlock<CILInstrList> block in basicBlocks)
			{
				if (block.Sources.Count <= 0 && !workList.Contains(block))
				{
					Dictionary<BasicBlock<CILInstrList>, BlockState> dictionary4 = blockStates;
					value = new BlockState
					{
						BeginStack = new ILASTVariable[0]
					};
					dictionary4[block] = value;
					workList.Push(block);
				}
			}
		}

		private void BuildPhiNodes()
		{
			foreach (KeyValuePair<BasicBlock<CILInstrList>, BlockState> statePair in blockStates)
			{
				BasicBlock<CILInstrList> block = statePair.Key;
				BlockState state = statePair.Value;
				if (block.Sources.Count == 0 && state.BeginStack.Length != 0)
				{
					Debug.Assert(state.BeginStack.Length == 1);
					ILASTPhi iLASTPhi = new ILASTPhi();
					iLASTPhi.Variable = state.BeginStack[0];
					iLASTPhi.SourceVariables = new ILASTVariable[1] { state.BeginStack[0] };
					ILASTPhi phi2 = iLASTPhi;
					state.ASTTree.Insert(0, phi2);
				}
				else
				{
					if (state.BeginStack.Length == 0)
					{
						continue;
					}
					for (int varIndex = 0; varIndex < state.BeginStack.Length; varIndex++)
					{
						ILASTPhi phi = new ILASTPhi
						{
							Variable = state.BeginStack[varIndex]
						};
						phi.SourceVariables = new ILASTVariable[block.Sources.Count];
						for (int i = 0; i < phi.SourceVariables.Length; i++)
						{
							phi.SourceVariables[i] = blockStates[block.Sources[i]].ASTTree.StackRemains[varIndex];
						}
						state.ASTTree.Insert(0, phi);
					}
				}
			}
		}

		private ILASTTree BuildAST(CILInstrList instrs, ILASTVariable[] beginStack)
		{
			ILASTTree tree = new ILASTTree();
			Stack<ILASTVariable> evalStack = new Stack<ILASTVariable>(beginStack);
			Func<int, IILASTNode[]> popArgs = delegate(int numArgs)
			{
				IILASTNode[] array = new IILASTNode[numArgs];
				for (int num = numArgs - 1; num >= 0; num--)
				{
					array[num] = evalStack.Pop();
				}
				return array;
			};
			List<Instruction> prefixes = new List<Instruction>();
			foreach (Instruction instr in instrs)
			{
				if (instr.OpCode.OpCodeType == OpCodeType.Prefix)
				{
					prefixes.Add(instr);
					continue;
				}
				int pushes;
				int pops;
				ILASTExpression expr;
				if (instr.OpCode.Code == Code.Dup)
				{
					pushes = (pops = 1);
					ILASTVariable arg = evalStack.Peek();
					ILASTExpression iLASTExpression = new ILASTExpression();
					iLASTExpression.ILCode = Code.Dup;
					iLASTExpression.Operand = null;
					iLASTExpression.Arguments = new IILASTNode[1] { arg };
					expr = iLASTExpression;
				}
				else
				{
					instr.CalculateStackUsage(method.ReturnType.ElementType != ElementType.Void, out pushes, out pops);
					Debug.Assert(pushes == 0 || pushes == 1);
					if (pops == -1)
					{
						evalStack.Clear();
						pops = 0;
					}
					expr = new ILASTExpression
					{
						ILCode = instr.OpCode.Code,
						Operand = instr.Operand,
						Arguments = popArgs(pops)
					};
					if (expr.Operand is Instruction || expr.Operand is Instruction[])
					{
						instrReferences.Add(expr);
					}
				}
				expr.CILInstr = instr;
				if (prefixes.Count > 0)
				{
					expr.Prefixes = prefixes.ToArray();
					prefixes.Clear();
				}
				if (pushes == 1)
				{
					ILASTVariable variable = new ILASTVariable
					{
						Name = $"s_{instr.Offset:x4}",
						VariableType = ILASTVariableType.StackVar
					};
					evalStack.Push(variable);
					tree.Add(new ILASTAssignment
					{
						Variable = variable,
						Value = expr
					});
				}
				else
				{
					tree.Add(expr);
				}
			}
			tree.StackRemains = evalStack.Reverse().ToArray();
			return tree;
		}
	}
}
