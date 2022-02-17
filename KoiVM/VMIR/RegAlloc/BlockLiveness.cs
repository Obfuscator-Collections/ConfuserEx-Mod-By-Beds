using System.Collections.Generic;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.RegAlloc
{
	public class BlockLiveness
	{
		public HashSet<IRVariable> InLive { get; }

		public HashSet<IRVariable> OutLive { get; }

		private BlockLiveness(HashSet<IRVariable> inLive, HashSet<IRVariable> outLive)
		{
			InLive = inLive;
			OutLive = outLive;
		}

		internal static BlockLiveness Empty()
		{
			return new BlockLiveness(new HashSet<IRVariable>(), new HashSet<IRVariable>());
		}

		internal BlockLiveness Clone()
		{
			return new BlockLiveness(new HashSet<IRVariable>(InLive), new HashSet<IRVariable>(OutLive));
		}

		public override string ToString()
		{
			return string.Format("In=[{0}], Out=[{1}]", string.Join(", ", InLive), string.Join(", ", OutLive));
		}
	}
}
