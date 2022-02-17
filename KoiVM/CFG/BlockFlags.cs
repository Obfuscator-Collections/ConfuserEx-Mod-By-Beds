using System;

namespace KoiVM.CFG
{
	[Flags]
	public enum BlockFlags
	{
		Normal = 0x0,
		ExitEHLeave = 0x1,
		ExitEHReturn = 0x2
	}
}
