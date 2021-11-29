using System;

namespace Confuser.Core
{
	/// <summary>
	///     Base class of Confuser protections.
	/// </summary>
	/// <remarks>
	///     A parameterless constructor must exists in derived classes to enable plugin discovery.
	/// </remarks>
	// Token: 0x02000072 RID: 114
	public abstract class Protection : ConfuserComponent
	{
		/// <summary>
		///     Gets the preset this protection is in.
		/// </summary>
		/// <value>The protection's preset.</value>
		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060002B5 RID: 693
		public abstract ProtectionPreset Preset
		{
			get;
		}
	}
}
