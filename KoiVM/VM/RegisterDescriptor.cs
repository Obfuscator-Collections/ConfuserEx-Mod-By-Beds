using System;
using System.Linq;

namespace KoiVM.VM
{
	public class RegisterDescriptor
	{
		private readonly byte[] regOrder = (from x in Enumerable.Range(0, 16)
			select (byte)x).ToArray();

		public byte this[VMRegisters reg] => regOrder[(int)reg];

		public RegisterDescriptor(Random random)
		{
			random.Shuffle(regOrder);
		}
	}
}
