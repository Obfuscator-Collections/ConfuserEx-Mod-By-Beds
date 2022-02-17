#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;

namespace KoiVM.ILAST.Transformation
{
	public class VariableInlining : ITransformationHandler
	{
		public void Initialize(ILASTTransformer tr)
		{
		}

		public void Transform(ILASTTransformer tr)
		{
			Dictionary<ILASTVariable, int> varUsage = new Dictionary<ILASTVariable, int>();
			for (int i = 0; i < tr.Tree.Count; i++)
			{
				IILASTStatement st = tr.Tree[i];
				ILASTExpression expr = GetExpression(st);
				if (expr == null)
				{
					continue;
				}
				if (st is ILASTExpression && expr.ILCode == Code.Nop)
				{
					tr.Tree.RemoveAt(i);
					i--;
					continue;
				}
				if (st is ILASTAssignment)
				{
					ILASTAssignment assignment = (ILASTAssignment)st;
					if (Array.IndexOf(tr.Tree.StackRemains, assignment.Variable) != -1)
					{
						continue;
					}
					Debug.Assert(assignment.Variable.VariableType == ILASTVariableType.StackVar);
				}
				IILASTNode[] arguments = expr.Arguments;
				foreach (IILASTNode arg in arguments)
				{
					Debug.Assert(arg is ILASTVariable);
					ILASTVariable argVar = (ILASTVariable)arg;
					if (argVar.VariableType == ILASTVariableType.StackVar)
					{
						varUsage.Increment(argVar);
					}
				}
			}
			ILASTVariable[] stackRemains = tr.Tree.StackRemains;
			foreach (ILASTVariable remain in stackRemains)
			{
				varUsage.Remove(remain);
			}
			HashSet<ILASTVariable> simpleVars = new HashSet<ILASTVariable>(from usage in varUsage
				where usage.Value == 1
				select usage into pair
				select pair.Key);
			bool modified;
			do
			{
				modified = false;
				for (int j = 0; j < tr.Tree.Count - 1; j++)
				{
					ILASTAssignment assignment2 = tr.Tree[j] as ILASTAssignment;
					if (assignment2 == null || !simpleVars.Contains(assignment2.Variable))
					{
						continue;
					}
					ILASTExpression expr2 = GetExpression(tr.Tree[j + 1]);
					if (expr2?.ILCode.ToOpCode().Name.StartsWith("stelem") ?? true)
					{
						continue;
					}
					for (int argIndex = 0; argIndex < expr2.Arguments.Length; argIndex++)
					{
						ILASTVariable argVar2 = expr2.Arguments[argIndex] as ILASTVariable;
						if (argVar2 == null)
						{
							break;
						}
						if (argVar2 == assignment2.Variable)
						{
							expr2.Arguments[argIndex] = assignment2.Value;
							tr.Tree.RemoveAt(j);
							j--;
							modified = true;
							break;
						}
					}
					if (modified)
					{
						break;
					}
				}
			}
			while (modified);
		}

		public static ILASTExpression GetExpression(IILASTStatement node)
		{
			if (node is ILASTExpression)
			{
				ILASTExpression expr = (ILASTExpression)node;
				if (expr.ILCode == Code.Pop && expr.Arguments[0] is ILASTExpression)
				{
					expr = (ILASTExpression)expr.Arguments[0];
				}
				return expr;
			}
			if (node is ILASTAssignment)
			{
				return ((ILASTAssignment)node).Value;
			}
			return null;
		}
	}
}
