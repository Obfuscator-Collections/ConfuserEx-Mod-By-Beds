using System;
using dnlib.DotNet;
using dnlib.DotNet.Writer;

namespace Confuser.Core
{
	/// <summary>
	///     The listener of module writer event.
	/// </summary>
	// Token: 0x02000045 RID: 69
	public class ModuleWriterListener : IModuleWriterListener
	{
		/// <inheritdoc />
		// Token: 0x060001A5 RID: 421 RVA: 0x0000D5F2 File Offset: 0x0000B7F2
		void IModuleWriterListener.OnWriterEvent(ModuleWriterBase writer, ModuleWriterEvent evt)
		{
			if (evt == ModuleWriterEvent.PESectionsCreated)
			{
				NativeEraser.Erase(writer as NativeModuleWriter, writer.Module as ModuleDefMD);
			}
			if (this.OnWriterEvent != null)
			{
				this.OnWriterEvent(writer, new ModuleWriterListenerEventArgs(evt));
			}
		}

		/// <summary>
		///     Occurs when a module writer event is triggered.
		/// </summary>
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060001A6 RID: 422 RVA: 0x0000D628 File Offset: 0x0000B828
		// (remove) Token: 0x060001A7 RID: 423 RVA: 0x0000D660 File Offset: 0x0000B860
		public event EventHandler<ModuleWriterListenerEventArgs> OnWriterEvent;
	}
}
