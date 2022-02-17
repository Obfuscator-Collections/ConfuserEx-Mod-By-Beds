using System;
using System.Linq;
using KoiVM.VMIL;

namespace KoiVM.VM
{
	public class OpCodeDescriptor
	{
		private readonly byte[] opCodeOrder = (from x in Enumerable.Range(0, 256)
			select (byte)x).ToArray();

		public byte this[ILOpCode opCode] => opCodeOrder[(int)opCode];

		public OpCodeDescriptor(Random random)
		{
			random.Shuffle(opCodeOrder);
		}
	}
}
