using System.Reflection;

namespace KoiVM.VM
{
	[Obfuscation(Exclude = false, ApplyToMembers = false, Feature = "+rename(forceRen=true);")]
	public enum VMRegisters
	{
		R0,
		R1,
		R2,
		R3,
		R4,
		R5,
		R6,
		R7,
		BP,
		SP,
		IP,
		FL,
		K1,
		K2,
		M1,
		M2,
		Max
	}
}
