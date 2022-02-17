using System.Reflection;

namespace KoiVM.VM
{
	[Obfuscation(Exclude = false, ApplyToMembers = false, Feature = "+rename(forceRen=true);")]
	public enum VMCalls
	{
		EXIT,
		BREAK,
		ECALL,
		CAST,
		CKFINITE,
		CKOVERFLOW,
		RANGECHK,
		INITOBJ,
		LDFLD,
		LDFTN,
		TOKEN,
		THROW,
		SIZEOF,
		STFLD,
		BOX,
		UNBOX,
		LOCALLOC,
		Max
	}
}
