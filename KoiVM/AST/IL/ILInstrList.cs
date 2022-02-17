using System;
using System.Collections.Generic;

namespace KoiVM.AST.IL
{
	public class ILInstrList : List<ILInstruction>
	{
		public override string ToString()
		{
			return string.Join(Environment.NewLine, this);
		}

		public void VisitInstrs<T>(VisitFunc<ILInstrList, ILInstruction, T> visitFunc, T arg)
		{
			for (int i = 0; i < base.Count; i++)
			{
				visitFunc(this, base[i], ref i, arg);
			}
		}
	}
}
