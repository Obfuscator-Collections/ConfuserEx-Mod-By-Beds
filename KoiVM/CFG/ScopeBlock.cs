using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet.Emit;

namespace KoiVM.CFG
{
	public class ScopeBlock
	{
		public ScopeType Type { get; }

		public ExceptionHandler ExceptionHandler { get; }

		public IList<ScopeBlock> Children { get; }

		public IList<IBasicBlock> Content { get; }

		public ScopeBlock()
		{
			Type = ScopeType.None;
			ExceptionHandler = null;
			Children = new List<ScopeBlock>();
			Content = new List<IBasicBlock>();
		}

		public ScopeBlock(ScopeType type, ExceptionHandler eh)
		{
			if (type == ScopeType.None)
			{
				throw new ArgumentException("type");
			}
			Type = type;
			ExceptionHandler = eh;
			Children = new List<ScopeBlock>();
			Content = new List<IBasicBlock>();
		}

		public IEnumerable<IBasicBlock> GetBasicBlocks()
		{
			Validate();
			if (Content.Count > 0)
			{
				return Content;
			}
			return Children.SelectMany((ScopeBlock child) => child.GetBasicBlocks());
		}

		public Dictionary<BasicBlock<TOld>, BasicBlock<TNew>> UpdateBasicBlocks<TOld, TNew>(Func<BasicBlock<TOld>, TNew> updateFunc)
		{
			return UpdateBasicBlocks(updateFunc, (int id, TNew content) => new BasicBlock<TNew>(id, content));
		}

		public Dictionary<BasicBlock<TOld>, BasicBlock<TNew>> UpdateBasicBlocks<TOld, TNew>(Func<BasicBlock<TOld>, TNew> updateFunc, Func<int, TNew, BasicBlock<TNew>> factoryFunc)
		{
			Dictionary<BasicBlock<TOld>, BasicBlock<TNew>> blockMap = new Dictionary<BasicBlock<TOld>, BasicBlock<TNew>>();
			UpdateBasicBlocksInternal(updateFunc, blockMap, factoryFunc);
			foreach (KeyValuePair<BasicBlock<TOld>, BasicBlock<TNew>> blockPair in blockMap)
			{
				foreach (BasicBlock<TOld> src in blockPair.Key.Sources)
				{
					blockPair.Value.Sources.Add(blockMap[src]);
				}
				foreach (BasicBlock<TOld> dst in blockPair.Key.Targets)
				{
					blockPair.Value.Targets.Add(blockMap[dst]);
				}
			}
			return blockMap;
		}

		private void UpdateBasicBlocksInternal<TOld, TNew>(Func<BasicBlock<TOld>, TNew> updateFunc, Dictionary<BasicBlock<TOld>, BasicBlock<TNew>> blockMap, Func<int, TNew, BasicBlock<TNew>> factoryFunc)
		{
			Validate();
			if (Content.Count > 0)
			{
				for (int i = 0; i < Content.Count; i++)
				{
					BasicBlock<TOld> oldBlock = (BasicBlock<TOld>)Content[i];
					TNew newContent = updateFunc(oldBlock);
					BasicBlock<TNew> newBlock = factoryFunc(oldBlock.Id, newContent);
					newBlock.Flags = oldBlock.Flags;
					Content[i] = newBlock;
					blockMap[oldBlock] = newBlock;
				}
				return;
			}
			foreach (ScopeBlock child in Children)
			{
				child.UpdateBasicBlocksInternal(updateFunc, blockMap, factoryFunc);
			}
		}

		public void ProcessBasicBlocks<T>(Action<BasicBlock<T>> processFunc)
		{
			Validate();
			if (Content.Count > 0)
			{
				foreach (IBasicBlock child2 in Content)
				{
					processFunc((BasicBlock<T>)child2);
				}
				return;
			}
			foreach (ScopeBlock child in Children)
			{
				child.ProcessBasicBlocks(processFunc);
			}
		}

		public void Validate()
		{
			if (Children.Count != 0 && Content.Count != 0)
			{
				throw new InvalidOperationException("Children and Content cannot be set at the same time.");
			}
		}

		public ScopeBlock[] SearchBlock(IBasicBlock target)
		{
			Stack<ScopeBlock> scopeStack = new Stack<ScopeBlock>();
			SearchBlockInternal(this, target, scopeStack);
			return scopeStack.Reverse().ToArray();
		}

		private static bool SearchBlockInternal(ScopeBlock scope, IBasicBlock target, Stack<ScopeBlock> scopeStack)
		{
			if (scope.Content.Count > 0)
			{
				if (scope.Content.Contains(target))
				{
					scopeStack.Push(scope);
					return true;
				}
				return false;
			}
			scopeStack.Push(scope);
			foreach (ScopeBlock child in scope.Children)
			{
				if (SearchBlockInternal(child, target, scopeStack))
				{
					return true;
				}
			}
			scopeStack.Pop();
			return false;
		}

		private static string ToString(ExceptionHandler eh)
		{
			return $"{eh.GetHashCode():x8}:{eh.HandlerType}";
		}

		public override string ToString()
		{
			StringBuilder ret = new StringBuilder();
			if (Type == ScopeType.Try)
			{
				ret.AppendLine("try @ " + ToString(ExceptionHandler) + " {");
			}
			else if (Type == ScopeType.Handler)
			{
				ret.AppendLine("handler @ " + ToString(ExceptionHandler) + " {");
			}
			else if (Type == ScopeType.Filter)
			{
				ret.AppendLine("filter @ " + ToString(ExceptionHandler) + " {");
			}
			else
			{
				ret.AppendLine("{");
			}
			if (Children.Count > 0)
			{
				foreach (ScopeBlock child2 in Children)
				{
					ret.AppendLine(child2.ToString());
				}
			}
			else
			{
				foreach (IBasicBlock child in Content)
				{
					ret.AppendLine(child.ToString());
				}
			}
			ret.AppendLine("}");
			return ret.ToString();
		}
	}
}
