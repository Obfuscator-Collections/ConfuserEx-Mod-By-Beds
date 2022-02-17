using System;
using System.Collections.Generic;
using System.Text;

namespace KoiVM.AST.ILAST
{
	public class ILASTTree : List<IILASTStatement>
	{
		public ILASTVariable[] StackRemains { get; set; }

		public override string ToString()
		{
			StringBuilder ret = new StringBuilder();
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IILASTStatement st = enumerator.Current;
					ret.AppendLine(st.ToString());
				}
			}
			ret.AppendLine();
			ret.Append("[");
			for (int i = 0; i < StackRemains.Length; i++)
			{
				if (i != 0)
				{
					ret.Append(", ");
				}
				ret.Append(StackRemains[i]);
			}
			ret.AppendLine("]");
			return ret.ToString();
		}

		public void TraverseTree<T>(Action<ILASTExpression, T> visitFunc, T state)
		{
			Enumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				IILASTStatement st = enumerator.Current;
				if (st is ILASTExpression)
				{
					TraverseTreeInternal((ILASTExpression)st, visitFunc, state);
				}
				else if (st is ILASTAssignment)
				{
					TraverseTreeInternal(((ILASTAssignment)st).Value, visitFunc, state);
				}
			}
		}

		private void TraverseTreeInternal<T>(ILASTExpression expr, Action<ILASTExpression, T> visitFunc, T state)
		{
			IILASTNode[] arguments = expr.Arguments;
			foreach (IILASTNode arg in arguments)
			{
				if (arg is ILASTExpression)
				{
					TraverseTreeInternal((ILASTExpression)arg, visitFunc, state);
				}
			}
			visitFunc(expr, state);
		}
	}
}
