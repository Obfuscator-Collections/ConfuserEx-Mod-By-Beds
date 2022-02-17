using System.Reflection;

namespace KoiVM.VM
{
	[Obfuscation(Exclude = false, ApplyToMembers = false, Feature = "+rename(forceRen=true);")]
	public enum VMFlags
	{
		OVERFLOW,
		CARRY,
		ZERO,
		SIGN,
		UNSIGNED,
		BEHAV1,
		BEHAV2,
		BEHAV3,
		Max
	}
}
