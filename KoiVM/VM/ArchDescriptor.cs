using System;

namespace KoiVM.VM
{
	public class ArchDescriptor
	{
		public OpCodeDescriptor OpCodes { get; }

		public FlagDescriptor Flags { get; }

		public RegisterDescriptor Registers { get; }

		public ArchDescriptor(Random random)
		{
			OpCodes = new OpCodeDescriptor(random);
			Flags = new FlagDescriptor(random);
			Registers = new RegisterDescriptor(random);
		}
	}
}
