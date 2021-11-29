using System;

namespace Confuser.Core
{
	/// <summary>
	///     Targets of protection.
	/// </summary>
	// Token: 0x02000077 RID: 119
	[Flags]
	public enum ProtectionTargets
	{
		/// <summary> Type definitions. </summary>
		// Token: 0x040001B1 RID: 433
		Types = 1,
		/// <summary> Method definitions. </summary>
		// Token: 0x040001B2 RID: 434
		Methods = 2,
		/// <summary> Field definitions. </summary>
		// Token: 0x040001B3 RID: 435
		Fields = 4,
		/// <summary> Event definitions. </summary>
		// Token: 0x040001B4 RID: 436
		Events = 8,
		/// <summary> Property definitions. </summary>
		// Token: 0x040001B5 RID: 437
		Properties = 16,
		/// <summary> All member definitions (i.e. type, methods, fields, events and properties). </summary>
		// Token: 0x040001B6 RID: 438
		AllMembers = 31,
		/// <summary> Module definitions. </summary>
		// Token: 0x040001B7 RID: 439
		Modules = 32,
		/// <summary> All definitions (i.e. All member definitions and modules). </summary>
		// Token: 0x040001B8 RID: 440
		AllDefinitions = 63
	}
}
