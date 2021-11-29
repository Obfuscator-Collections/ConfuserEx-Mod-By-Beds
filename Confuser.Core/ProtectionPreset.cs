using System;

namespace Confuser.Core
{
	/// <summary>
	///     Various presets of protections.
	/// </summary>
	// Token: 0x0200007C RID: 124
	public enum ProtectionPreset
	{
		/// <summary> The protection does not belong to any preset. </summary>
		// Token: 0x040001E9 RID: 489
		None,
		/// <summary> The protection provides basic security. </summary>
		// Token: 0x040001EA RID: 490
		Minimum,
		/// <summary> The protection provides normal security for public release. </summary>
		// Token: 0x040001EB RID: 491
		Normal,
		/// <summary> The protection provides better security with observable performance impact. </summary>
		// Token: 0x040001EC RID: 492
		Aggressive,
		/// <summary> The protection provides strongest security with possible incompatibility. </summary>
		// Token: 0x040001ED RID: 493
		Maximum
	}
}
