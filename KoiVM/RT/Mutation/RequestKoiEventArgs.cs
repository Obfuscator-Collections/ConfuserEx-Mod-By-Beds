using System;

namespace KoiVM.RT.Mutation
{
	internal class RequestKoiEventArgs : EventArgs
	{
		public KoiHeap Heap { get; set; }
	}
}
