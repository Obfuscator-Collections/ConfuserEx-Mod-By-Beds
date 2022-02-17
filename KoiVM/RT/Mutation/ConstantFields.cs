using System.Reflection;

namespace KoiVM.RT.Mutation
{
	[Obfuscation(Exclude = false, ApplyToMembers = false, Feature = "+rename(forceRen=true);")]
	internal enum ConstantFields
	{
		E_CALL,
		E_CALLVIRT,
		E_NEWOBJ,
		E_CALLVIRT_CONSTRAINED,
		INIT,
		INSTANCE,
		CATCH,
		FILTER,
		FAULT,
		FINALLY
	}
}
