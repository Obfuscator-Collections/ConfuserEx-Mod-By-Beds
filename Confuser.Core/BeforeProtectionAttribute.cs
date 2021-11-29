using System;

namespace Confuser.Core
{
	/// <summary>
	///     Indicates the <see cref="T:Confuser.Core.Protection" /> must initialize before the specified protections.
	/// </summary>
	// Token: 0x02000073 RID: 115
	[AttributeUsage(AttributeTargets.Class)]
	public class BeforeProtectionAttribute : Attribute
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.BeforeProtectionAttribute" /> class.
		/// </summary>
		/// <param name="ids">The full IDs of the specified protections.</param>
		// Token: 0x060002B7 RID: 695 RVA: 0x000122AF File Offset: 0x000104AF
		public BeforeProtectionAttribute(params string[] ids)
		{
			this.Ids = ids;
		}

		/// <summary>
		///     Gets the full IDs of the specified protections.
		/// </summary>
		/// <value>The IDs of protections.</value>
		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060002B8 RID: 696 RVA: 0x000122BE File Offset: 0x000104BE
		// (set) Token: 0x060002B9 RID: 697 RVA: 0x000122C6 File Offset: 0x000104C6
		public string[] Ids
		{
			get;
			private set;
		}
	}
}
