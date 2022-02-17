#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Pdb;

namespace KoiVM.CFG
{
	public class BlockParser
	{
		public static ScopeBlock Parse(MethodDef method, CilBody body)
		{
			body.SimplifyMacros(method.Parameters);
			ExpandSequencePoints(body);
			FindHeaders(body, out var headers, out var entries);
			List<BasicBlock<CILInstrList>> blocks = SplitBlocks(body, headers, entries);
			LinkBlocks(blocks);
			return AssignScopes(body, blocks);
		}

		private static void ExpandSequencePoints(CilBody body)
		{
			SequencePoint current = null;
			foreach (Instruction instr in body.Instructions)
			{
				if (instr.SequencePoint != null)
				{
					current = instr.SequencePoint;
				}
				else
				{
					instr.SequencePoint = current;
				}
			}
		}

		private static void FindHeaders(CilBody body, out HashSet<Instruction> headers, out HashSet<Instruction> entries)
		{
			headers = new HashSet<Instruction>();
			entries = new HashSet<Instruction>();
			foreach (ExceptionHandler eh in body.ExceptionHandlers)
			{
				headers.Add(eh.TryStart);
				if (eh.TryEnd != null)
				{
					headers.Add(eh.TryEnd);
				}
				headers.Add(eh.HandlerStart);
				entries.Add(eh.HandlerStart);
				if (eh.HandlerEnd != null)
				{
					headers.Add(eh.HandlerEnd);
				}
				if (eh.FilterStart != null)
				{
					headers.Add(eh.FilterStart);
					entries.Add(eh.FilterStart);
				}
			}
			IList<Instruction> instrs = body.Instructions;
			for (int i = 0; i < instrs.Count; i++)
			{
				Instruction instr = instrs[i];
				if (instr.Operand is Instruction)
				{
					headers.Add((Instruction)instr.Operand);
					if (i + 1 < body.Instructions.Count)
					{
						headers.Add(body.Instructions[i + 1]);
					}
				}
				else if (instr.Operand is Instruction[])
				{
					Instruction[] array = (Instruction[])instr.Operand;
					foreach (Instruction target in array)
					{
						headers.Add(target);
					}
					if (i + 1 < body.Instructions.Count)
					{
						headers.Add(body.Instructions[i + 1]);
					}
				}
				else if ((instr.OpCode.FlowControl == FlowControl.Throw || instr.OpCode.FlowControl == FlowControl.Return) && i + 1 < body.Instructions.Count)
				{
					headers.Add(body.Instructions[i + 1]);
				}
			}
			if (instrs.Count > 0)
			{
				headers.Add(instrs[0]);
				entries.Add(instrs[0]);
			}
		}

		private static List<BasicBlock<CILInstrList>> SplitBlocks(CilBody body, HashSet<Instruction> headers, HashSet<Instruction> entries)
		{
			int nextBlockId = 0;
			int currentBlockId = -1;
			Instruction currentBlockHdr = null;
			List<BasicBlock<CILInstrList>> blocks = new List<BasicBlock<CILInstrList>>();
			CILInstrList instrList = new CILInstrList();
			for (int i = 0; i < body.Instructions.Count; i++)
			{
				Instruction instr = body.Instructions[i];
				if (headers.Contains(instr))
				{
					if (currentBlockHdr != null)
					{
						Instruction footer = body.Instructions[i - 1];
						Debug.Assert(instrList.Count > 0);
						blocks.Add(new BasicBlock<CILInstrList>(currentBlockId, instrList));
						instrList = new CILInstrList();
					}
					currentBlockId = nextBlockId++;
					currentBlockHdr = instr;
				}
				instrList.Add(instr);
			}
			if (blocks.Count == 0 || blocks[blocks.Count - 1].Id != currentBlockId)
			{
				Instruction footer2 = body.Instructions[body.Instructions.Count - 1];
				Debug.Assert(instrList.Count > 0);
				blocks.Add(new BasicBlock<CILInstrList>(currentBlockId, instrList));
			}
			return blocks;
		}

		private static void LinkBlocks(List<BasicBlock<CILInstrList>> blocks)
		{
			Dictionary<Instruction, BasicBlock<CILInstrList>> instrMap = blocks.SelectMany((BasicBlock<CILInstrList> block) => block.Content.Select((Instruction instr) => new
			{
				Instr = instr,
				Block = block
			})).ToDictionary(instr => instr.Instr, instr => instr.Block);
			foreach (BasicBlock<CILInstrList> block2 in blocks)
			{
				foreach (Instruction instr2 in block2.Content)
				{
					if (instr2.Operand is Instruction)
					{
						BasicBlock<CILInstrList> dstBlock2 = instrMap[(Instruction)instr2.Operand];
						dstBlock2.Sources.Add(block2);
						block2.Targets.Add(dstBlock2);
					}
					else if (instr2.Operand is Instruction[])
					{
						Instruction[] array = (Instruction[])instr2.Operand;
						foreach (Instruction target in array)
						{
							BasicBlock<CILInstrList> dstBlock = instrMap[target];
							dstBlock.Sources.Add(block2);
							block2.Targets.Add(dstBlock);
						}
					}
				}
			}
			for (int i = 0; i < blocks.Count; i++)
			{
				Instruction footer = blocks[i].Content.Last();
				if (footer.OpCode.FlowControl != 0 && footer.OpCode.FlowControl != FlowControl.Return && footer.OpCode.FlowControl != FlowControl.Throw && i + 1 < blocks.Count)
				{
					BasicBlock<CILInstrList> src = blocks[i];
					BasicBlock<CILInstrList> dst = blocks[i + 1];
					if (!src.Targets.Contains(dst))
					{
						src.Targets.Add(dst);
						dst.Sources.Add(src);
						src.Content.Add(Instruction.Create(OpCodes.Br, dst.Content[0]));
					}
				}
			}
		}

		private static ScopeBlock AssignScopes(CilBody body, List<BasicBlock<CILInstrList>> blocks)
		{
			Dictionary<ExceptionHandler, Tuple<ScopeBlock, ScopeBlock, ScopeBlock>> ehScopes = new Dictionary<ExceptionHandler, Tuple<ScopeBlock, ScopeBlock, ScopeBlock>>();
			foreach (ExceptionHandler eh4 in body.ExceptionHandlers)
			{
				ScopeBlock tryBlock = new ScopeBlock(ScopeType.Try, eh4);
				ScopeBlock handlerBlock = new ScopeBlock(ScopeType.Handler, eh4);
				if (eh4.FilterStart != null)
				{
					ScopeBlock filterBlock = new ScopeBlock(ScopeType.Filter, eh4);
					ehScopes[eh4] = Tuple.Create(tryBlock, handlerBlock, filterBlock);
				}
				else
				{
					ehScopes[eh4] = Tuple.Create<ScopeBlock, ScopeBlock, ScopeBlock>(tryBlock, handlerBlock, null);
				}
			}
			ScopeBlock root = new ScopeBlock();
			Stack<ScopeBlock> scopeStack = new Stack<ScopeBlock>();
			scopeStack.Push(root);
			foreach (BasicBlock<CILInstrList> block in blocks)
			{
				Instruction header = block.Content[0];
				foreach (ExceptionHandler eh3 in body.ExceptionHandlers)
				{
					Tuple<ScopeBlock, ScopeBlock, ScopeBlock> ehScope2 = ehScopes[eh3];
					if (header == eh3.TryEnd)
					{
						ScopeBlock pop5 = scopeStack.Pop();
						Debug.Assert(pop5 == ehScope2.Item1);
					}
					if (header == eh3.HandlerEnd)
					{
						ScopeBlock pop4 = scopeStack.Pop();
						Debug.Assert(pop4 == ehScope2.Item2);
					}
					if (eh3.FilterStart != null && header == eh3.HandlerStart)
					{
						Debug.Assert(scopeStack.Peek().Type == ScopeType.Filter);
						ScopeBlock pop3 = scopeStack.Pop();
						Debug.Assert(pop3 == ehScope2.Item3);
					}
				}
				foreach (ExceptionHandler eh2 in body.ExceptionHandlers.Reverse())
				{
					Tuple<ScopeBlock, ScopeBlock, ScopeBlock> ehScope = ehScopes[eh2];
					ScopeBlock parent = ((scopeStack.Count > 0) ? scopeStack.Peek() : null);
					if (header == eh2.TryStart)
					{
						if (parent != null)
						{
							AddScopeBlock(parent, ehScope.Item1);
						}
						scopeStack.Push(ehScope.Item1);
					}
					if (header == eh2.HandlerStart)
					{
						if (parent != null)
						{
							AddScopeBlock(parent, ehScope.Item2);
						}
						scopeStack.Push(ehScope.Item2);
					}
					if (header == eh2.FilterStart)
					{
						if (parent != null)
						{
							AddScopeBlock(parent, ehScope.Item3);
						}
						scopeStack.Push(ehScope.Item3);
					}
				}
				ScopeBlock scope = scopeStack.Peek();
				AddBasicBlock(scope, block);
			}
			foreach (ExceptionHandler eh in body.ExceptionHandlers)
			{
				if (eh.TryEnd == null)
				{
					ScopeBlock pop2 = scopeStack.Pop();
					Debug.Assert(pop2 == ehScopes[eh].Item1);
				}
				if (eh.HandlerEnd == null)
				{
					ScopeBlock pop = scopeStack.Pop();
					Debug.Assert(pop == ehScopes[eh].Item2);
				}
			}
			Debug.Assert(scopeStack.Count == 1);
			Validate(root);
			return root;
		}

		private static void Validate(ScopeBlock scope)
		{
			scope.Validate();
			foreach (ScopeBlock child in scope.Children)
			{
				Validate(child);
			}
		}

		private static void AddScopeBlock(ScopeBlock block, ScopeBlock child)
		{
			if (block.Content.Count > 0)
			{
				ScopeBlock newScope = new ScopeBlock();
				foreach (IBasicBlock instrBlock in block.Content)
				{
					newScope.Content.Add(instrBlock);
				}
				block.Content.Clear();
				block.Children.Add(newScope);
			}
			block.Children.Add(child);
		}

		private static void AddBasicBlock(ScopeBlock block, BasicBlock<CILInstrList> child)
		{
			if (block.Children.Count > 0)
			{
				ScopeBlock last = block.Children.Last();
				if (last.Type != 0)
				{
					last = new ScopeBlock();
					block.Children.Add(last);
				}
				block = last;
			}
			Debug.Assert(block.Children.Count == 0);
			block.Content.Add(child);
		}
	}
}
