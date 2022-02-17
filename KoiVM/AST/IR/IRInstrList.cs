using System;
using System.Collections.Generic;

namespace KoiVM.AST.IR
{
	public class IRInstrList : List<IRInstruction>
	{
		public override string ToString()
		{
			return string.Join(Environment.NewLine, this);
		}

		public void VisitInstrs<T>(VisitFunc<IRInstrList, IRInstruction, T> visitFunc, T arg)
		{
			for (int i = 0; i < base.Count; i++)
			{
				visitFunc(this, base[i], ref i, arg);
			}
		}
	}
}
