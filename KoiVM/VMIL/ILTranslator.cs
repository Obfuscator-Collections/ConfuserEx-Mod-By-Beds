using System;
using System.Collections.Generic;
using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.CFG;
using KoiVM.RT;
using KoiVM.VM;
using KoiVM.VMIR;

namespace KoiVM.VMIL
{
	public class ILTranslator
	{
		private static readonly Dictionary<IROpCode, ITranslationHandler> handlers;

		public VMRuntime Runtime { get; }

		public VMDescriptor VM => Runtime.Descriptor;

		internal ILInstrList Instructions { get; private set; }

		static ILTranslator()
		{
			handlers = new Dictionary<IROpCode, ITranslationHandler>();
			Type[] exportedTypes = typeof(ILTranslator).Assembly.GetExportedTypes();
			foreach (Type type in exportedTypes)
			{
				if (typeof(ITranslationHandler).IsAssignableFrom(type) && !type.IsAbstract)
				{
					ITranslationHandler handler = (ITranslationHandler)Activator.CreateInstance(type);
					handlers.Add(handler.IRCode, handler);
				}
			}
		}

		public ILTranslator(VMRuntime runtime)
		{
			Runtime = runtime;
		}

		public ILInstrList Translate(IRInstrList instrs)
		{
			Instructions = new ILInstrList();
			int i = 0;
			foreach (IRInstruction instr in instrs)
			{
				if (!handlers.TryGetValue(instr.OpCode, out var handler))
				{
					throw new NotSupportedException(instr.OpCode.ToString());
				}
				try
				{
					handler.Translate(instr, this);
				}
				catch (Exception ex)
				{
					throw new Exception($"Failed to translate ir {instr.ILAST}.", ex);
				}
				for (; i < Instructions.Count; i++)
				{
					Instructions[i].IR = instr;
				}
			}
			ILInstrList ret = Instructions;
			Instructions = null;
			return ret;
		}

		public void Translate(ScopeBlock rootScope)
		{
			Dictionary<BasicBlock<IRInstrList>, BasicBlock<ILInstrList>> blockMap = rootScope.UpdateBasicBlocks((BasicBlock<IRInstrList> block) => Translate(block.Content), (int id, ILInstrList content) => new ILBlock(id, content));
			rootScope.ProcessBasicBlocks(delegate(BasicBlock<ILInstrList> block)
			{
				foreach (ILInstruction current in block.Content)
				{
					if (current.Operand is ILBlockTarget)
					{
						ILBlockTarget iLBlockTarget = (ILBlockTarget)current.Operand;
						iLBlockTarget.Target = blockMap[(BasicBlock<IRInstrList>)iLBlockTarget.Target];
					}
					else if (current.Operand is ILJumpTable)
					{
						ILJumpTable iLJumpTable = (ILJumpTable)current.Operand;
						for (int i = 0; i < iLJumpTable.Targets.Length; i++)
						{
							iLJumpTable.Targets[i] = blockMap[(BasicBlock<IRInstrList>)iLJumpTable.Targets[i]];
						}
					}
				}
			});
		}
	}
}
