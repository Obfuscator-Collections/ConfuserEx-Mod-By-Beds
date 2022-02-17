using dnlib.DotNet;

namespace KoiVM.VM
{
	public class FuncSig
	{
		public byte Flags;

		public ITypeDefOrRef[] ParamSigs;

		public ITypeDefOrRef RetType;

		public override int GetHashCode()
		{
			SigComparer comparer = default(SigComparer);
			int hashCode = Flags;
			ITypeDefOrRef[] paramSigs = ParamSigs;
			foreach (ITypeDefOrRef param in paramSigs)
			{
				hashCode = hashCode * 7 + comparer.GetHashCode(param);
			}
			return hashCode * 7 + comparer.GetHashCode(RetType);
		}

		public override bool Equals(object obj)
		{
			FuncSig other = obj as FuncSig;
			if (other == null || other.Flags != Flags)
			{
				return false;
			}
			if (other.ParamSigs.Length != ParamSigs.Length)
			{
				return false;
			}
			SigComparer comparer = default(SigComparer);
			for (int i = 0; i < ParamSigs.Length; i++)
			{
				if (!comparer.Equals(ParamSigs[i], other.ParamSigs[i]))
				{
					return false;
				}
			}
			if (!comparer.Equals(RetType, other.RetType))
			{
				return false;
			}
			return true;
		}
	}
}
