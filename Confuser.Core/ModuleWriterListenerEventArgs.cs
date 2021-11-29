using System;
using dnlib.DotNet.Writer;

namespace Confuser.Core
{
	/// <summary>
	///     Indicates the triggered writer event.
	/// </summary>
	// Token: 0x02000046 RID: 70
	public class ModuleWriterListenerEventArgs : EventArgs
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.ModuleWriterListenerEventArgs" /> class.
		/// </summary>
		/// <param name="evt">The triggered writer event.</param>
		// Token: 0x060001A9 RID: 425 RVA: 0x0000D69D File Offset: 0x0000B89D
		public ModuleWriterListenerEventArgs(ModuleWriterEvent evt)
		{
			this.WriterEvent = evt;
		}

		/// <summary>
		///     Gets the triggered writer event.
		/// </summary>
		/// <value>The triggered writer event.</value>
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060001AA RID: 426 RVA: 0x0000D6AC File Offset: 0x0000B8AC
		// (set) Token: 0x060001AB RID: 427 RVA: 0x0000D6B4 File Offset: 0x0000B8B4
		public ModuleWriterEvent WriterEvent
		{
			get;
			private set;
		}
	}
}
