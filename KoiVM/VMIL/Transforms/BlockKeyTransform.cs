using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet.Emit;
using KoiVM.AST.IL;
using KoiVM.CFG;
using KoiVM.RT;
using KoiVM.VM;

namespace KoiVM.VMIL.Transforms
{
	public class BlockKeyTransform : IPostTransform
	{
		private struct BlockKey
		{
			public uint Entry;

			public uint Exit;
		}

		private class FinallyInfo
		{
			public readonly HashSet<ILBlock> FinallyEnds = new HashSet<ILBlock>();

			public readonly HashSet<ILBlock> TryEndNexts = new HashSet<ILBlock>();
		}

		private class EHMap
		{
			public readonly Dictionary<ExceptionHandler, FinallyInfo> Finally = new Dictionary<ExceptionHandler, FinallyInfo>();

			public readonly HashSet<ILBlock> Starts = new HashSet<ILBlock>();
		}

		private Dictionary<ILBlock, BlockKey> Keys;

		private VMMethodInfo methodInfo;

		private VMRuntime runtime;

		public void Initialize(ILPostTransformer tr)
		{
			runtime = tr.Runtime;
			methodInfo = tr.Runtime.Descriptor.Data.LookupInfo(tr.Method);
			ComputeBlockKeys(tr.RootScope);
		}

		public void Transform(ILPostTransformer tr)
		{
			BlockKey key = Keys[tr.Block];
			methodInfo.BlockKeys[tr.Block] = new VMBlockKey
			{
				EntryKey = (byte)key.Entry,
				ExitKey = (byte)key.Exit
			};
		}

		private void ComputeBlockKeys(ScopeBlock rootScope)
		{
			List<ILBlock> blocks = rootScope.GetBasicBlocks().OfType<ILBlock>().ToList();
			uint id = 1u;
			Keys = blocks.ToDictionary((ILBlock block) => block, delegate
			{
				BlockKey result = default(BlockKey);
				result.Entry = id++;
				result.Exit = id++;
				return result;
			});
			EHMap ehMap = MapEHs(rootScope);
			bool updated;
			do
			{
				updated = false;
				BlockKey key = Keys[blocks[0]];
				key.Entry = 4294967294u;
				Keys[blocks[0]] = key;
				key = Keys[blocks[blocks.Count - 1]];
				key.Exit = 4294967293u;
				Keys[blocks[blocks.Count - 1]] = key;
				foreach (ILBlock block3 in blocks)
				{
					key = Keys[block3];
					if (block3.Sources.Count > 0)
					{
						uint newEntry = block3.Sources.Select((BasicBlock<ILInstrList> b) => Keys[(ILBlock)b].Exit).Max();
						if (key.Entry != newEntry)
						{
							key.Entry = newEntry;
							updated = true;
						}
					}
					if (block3.Targets.Count > 0)
					{
						uint newExit = block3.Targets.Select((BasicBlock<ILInstrList> b) => Keys[(ILBlock)b].Entry).Max();
						if (key.Exit != newExit)
						{
							key.Exit = newExit;
							updated = true;
						}
					}
					Keys[block3] = key;
				}
				MatchHandlers(ehMap, ref updated);
			}
			while (updated);
			Dictionary<uint, uint> idMap = new Dictionary<uint, uint>();
			idMap[uint.MaxValue] = 0u;
			idMap[4294967294u] = methodInfo.EntryKey;
			idMap[4294967293u] = methodInfo.ExitKey;
			foreach (ILBlock block2 in blocks)
			{
				BlockKey key2 = Keys[block2];
				uint entryId = key2.Entry;
				if (!idMap.TryGetValue(entryId, out key2.Entry))
				{
					uint num2 = (key2.Entry = (idMap[entryId] = (byte)runtime.Descriptor.Random.Next()));
				}
				uint exitId = key2.Exit;
				if (!idMap.TryGetValue(exitId, out key2.Exit))
				{
					uint num2 = (key2.Exit = (idMap[exitId] = (byte)runtime.Descriptor.Random.Next()));
				}
				Keys[block2] = key2;
			}
		}

		private EHMap MapEHs(ScopeBlock rootScope)
		{
			EHMap map = new EHMap();
			MapEHsInternal(rootScope, map);
			return map;
		}

		private void MapEHsInternal(ScopeBlock scope, EHMap map)
		{
			if (scope.Type == ScopeType.Filter)
			{
				map.Starts.Add((ILBlock)scope.GetBasicBlocks().First());
			}
			else if (scope.Type != 0)
			{
				if (scope.ExceptionHandler.HandlerType == ExceptionHandlerType.Finally)
				{
					if (!map.Finally.TryGetValue(scope.ExceptionHandler, out var info))
					{
						info = (map.Finally[scope.ExceptionHandler] = new FinallyInfo());
					}
					if (scope.Type == ScopeType.Try)
					{
						HashSet<IBasicBlock> scopeBlocks = new HashSet<IBasicBlock>(scope.GetBasicBlocks());
						foreach (ILBlock block2 in scopeBlocks)
						{
							if ((block2.Flags & BlockFlags.ExitEHLeave) == 0 || (block2.Targets.Count != 0 && !block2.Targets.Any((BasicBlock<ILInstrList> target) => !scopeBlocks.Contains(target))))
							{
								continue;
							}
							foreach (BasicBlock<ILInstrList> target2 in block2.Targets)
							{
								info.TryEndNexts.Add((ILBlock)target2);
							}
						}
					}
					else if (scope.Type == ScopeType.Handler)
					{
						IEnumerable<IBasicBlock> candidates = ((scope.Children.Count <= 0) ? scope.Content : scope.Children.Where((ScopeBlock s) => s.Type == ScopeType.None).SelectMany((ScopeBlock s) => s.GetBasicBlocks()));
						foreach (ILBlock block in candidates)
						{
							if ((block.Flags & BlockFlags.ExitEHReturn) != 0 && block.Targets.Count == 0)
							{
								info.FinallyEnds.Add(block);
							}
						}
					}
				}
				if (scope.Type == ScopeType.Handler)
				{
					map.Starts.Add((ILBlock)scope.GetBasicBlocks().First());
				}
			}
			foreach (ScopeBlock child in scope.Children)
			{
				MapEHsInternal(child, map);
			}
		}

		private void MatchHandlers(EHMap map, ref bool updated)
		{
			foreach (ILBlock start in map.Starts)
			{
				BlockKey key = Keys[start];
				if (key.Entry != uint.MaxValue)
				{
					key.Entry = uint.MaxValue;
					Keys[start] = key;
					updated = true;
				}
			}
			foreach (FinallyInfo info in map.Finally.Values)
			{
				uint maxEnd = info.FinallyEnds.Max((ILBlock block) => Keys[block].Exit);
				uint maxEntry = info.TryEndNexts.Max((ILBlock block) => Keys[block].Entry);
				uint maxId = Math.Max(maxEnd, maxEntry);
				foreach (ILBlock block3 in info.FinallyEnds)
				{
					BlockKey key3 = Keys[block3];
					if (key3.Exit != maxId)
					{
						key3.Exit = maxId;
						Keys[block3] = key3;
						updated = true;
					}
				}
				foreach (ILBlock block2 in info.TryEndNexts)
				{
					BlockKey key2 = Keys[block2];
					if (key2.Entry != maxId)
					{
						key2.Entry = maxId;
						Keys[block2] = key2;
						updated = true;
					}
				}
			}
		}
	}
}
