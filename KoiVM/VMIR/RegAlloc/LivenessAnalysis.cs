#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KoiVM.AST.IR;
using KoiVM.CFG;

namespace KoiVM.VMIR.RegAlloc
{
	public class LivenessAnalysis
	{
		private enum LiveFlags
		{
			GEN1 = 1,
			GEN2 = 2,
			KILL1 = 4,
			KILL2 = 8
		}

		private static readonly Dictionary<IROpCode, LiveFlags> opCodeLiveness = new Dictionary<IROpCode, LiveFlags>
		{
			{
				IROpCode.MOV,
				(LiveFlags)6
			},
			{
				IROpCode.POP,
				LiveFlags.KILL1
			},
			{
				IROpCode.PUSH,
				LiveFlags.GEN1
			},
			{
				IROpCode.CALL,
				(LiveFlags)9
			},
			{
				IROpCode.NOR,
				(LiveFlags)7
			},
			{
				IROpCode.CMP,
				(LiveFlags)3
			},
			{
				IROpCode.JZ,
				LiveFlags.GEN2
			},
			{
				IROpCode.JNZ,
				LiveFlags.GEN2
			},
			{
				IROpCode.SWT,
				LiveFlags.GEN2
			},
			{
				IROpCode.ADD,
				(LiveFlags)7
			},
			{
				IROpCode.SUB,
				(LiveFlags)7
			},
			{
				IROpCode.MUL,
				(LiveFlags)7
			},
			{
				IROpCode.DIV,
				(LiveFlags)7
			},
			{
				IROpCode.REM,
				(LiveFlags)7
			},
			{
				IROpCode.SHR,
				(LiveFlags)7
			},
			{
				IROpCode.SHL,
				(LiveFlags)7
			},
			{
				IROpCode.FCONV,
				(LiveFlags)6
			},
			{
				IROpCode.ICONV,
				(LiveFlags)6
			},
			{
				IROpCode.SX,
				(LiveFlags)6
			},
			{
				IROpCode.VCALL,
				LiveFlags.GEN1
			},
			{
				IROpCode.TRY,
				(LiveFlags)3
			},
			{
				IROpCode.LEAVE,
				LiveFlags.GEN1
			},
			{
				IROpCode.__EHRET,
				LiveFlags.GEN1
			},
			{
				IROpCode.__LEA,
				(LiveFlags)6
			},
			{
				IROpCode.__LDOBJ,
				(LiveFlags)9
			},
			{
				IROpCode.__STOBJ,
				(LiveFlags)3
			},
			{
				IROpCode.__GEN,
				LiveFlags.GEN1
			},
			{
				IROpCode.__KILL,
				LiveFlags.KILL1
			}
		};

		public static Dictionary<BasicBlock<IRInstrList>, BlockLiveness> ComputeLiveness(IList<BasicBlock<IRInstrList>> blocks)
		{
			Dictionary<BasicBlock<IRInstrList>, BlockLiveness> liveness = new Dictionary<BasicBlock<IRInstrList>, BlockLiveness>();
			List<BasicBlock<IRInstrList>> entryBlocks = blocks.Where((BasicBlock<IRInstrList> block) => block.Sources.Count == 0).ToList();
			List<BasicBlock<IRInstrList>> order = new List<BasicBlock<IRInstrList>>();
			HashSet<BasicBlock<IRInstrList>> visited = new HashSet<BasicBlock<IRInstrList>>();
			foreach (BasicBlock<IRInstrList> entry in entryBlocks)
			{
				PostorderTraversal(entry, visited, delegate(BasicBlock<IRInstrList> block)
				{
					order.Add(block);
				});
			}
			bool worked = false;
			do
			{
				foreach (BasicBlock<IRInstrList> currentBlock in order)
				{
					BlockLiveness blockLiveness = BlockLiveness.Empty();
					foreach (BasicBlock<IRInstrList> successor in currentBlock.Targets)
					{
						if (liveness.TryGetValue(successor, out var successorLiveness))
						{
							blockLiveness.OutLive.UnionWith(successorLiveness.InLive);
						}
					}
					HashSet<IRVariable> live = new HashSet<IRVariable>(blockLiveness.OutLive);
					for (int i = currentBlock.Content.Count - 1; i >= 0; i--)
					{
						IRInstruction instr = currentBlock.Content[i];
						ComputeInstrLiveness(instr, live);
					}
					blockLiveness.InLive.UnionWith(live);
					if (!worked && liveness.TryGetValue(currentBlock, out var prevLiveness))
					{
						worked = !prevLiveness.InLive.SetEquals(blockLiveness.InLive) || !prevLiveness.OutLive.SetEquals(blockLiveness.OutLive);
					}
					liveness[currentBlock] = blockLiveness;
				}
			}
			while (worked);
			return liveness;
		}

		public static Dictionary<IRInstruction, HashSet<IRVariable>> ComputeLiveness(BasicBlock<IRInstrList> block, BlockLiveness liveness)
		{
			Dictionary<IRInstruction, HashSet<IRVariable>> ret = new Dictionary<IRInstruction, HashSet<IRVariable>>();
			HashSet<IRVariable> live = new HashSet<IRVariable>(liveness.OutLive);
			for (int i = block.Content.Count - 1; i >= 0; i--)
			{
				IRInstruction instr = block.Content[i];
				ComputeInstrLiveness(instr, live);
				ret[instr] = new HashSet<IRVariable>(live);
			}
			Debug.Assert(live.SetEquals(liveness.InLive));
			return ret;
		}

		private static void PostorderTraversal(BasicBlock<IRInstrList> block, HashSet<BasicBlock<IRInstrList>> visited, Action<BasicBlock<IRInstrList>> visitFunc)
		{
			visited.Add(block);
			foreach (BasicBlock<IRInstrList> successor in block.Targets)
			{
				if (!visited.Contains(successor))
				{
					PostorderTraversal(successor, visited, visitFunc);
				}
			}
			visitFunc(block);
		}

		private static void ComputeInstrLiveness(IRInstruction instr, HashSet<IRVariable> live)
		{
			if (!opCodeLiveness.TryGetValue(instr.OpCode, out var flags))
			{
				flags = (LiveFlags)0;
			}
			IRVariable op1 = instr.Operand1 as IRVariable;
			IRVariable op2 = instr.Operand2 as IRVariable;
			if ((flags & LiveFlags.KILL1) != 0 && op1 != null)
			{
				live.Remove(op1);
			}
			if ((flags & LiveFlags.KILL2) != 0 && op2 != null)
			{
				live.Remove(op2);
			}
			if ((flags & LiveFlags.GEN1) != 0 && op1 != null)
			{
				live.Add(op1);
			}
			if ((flags & LiveFlags.GEN2) != 0 && op2 != null)
			{
				live.Add(op2);
			}
		}
	}
}
