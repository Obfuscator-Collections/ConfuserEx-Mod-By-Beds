using System;

namespace Confuser.Core
{
	/// <summary>
	///     Indicates the <see cref="T:Confuser.Core.Protection" /> must initialize after the specified protections.
	/// </summary>
	// Token: 0x02000074 RID: 116
	[AttributeUsage(AttributeTargets.Class)]
	public class AfterProtectionAttribute : Attribute
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.BeforeProtectionAttribute" /> class.
		/// </summary>
		/// <param name="ids">The full IDs of the specified protections.</param>
		// Token: 0x060002BA RID: 698 RVA: 0x000122CF File Offset: 0x000104CF
		public AfterProtectionAttribute(params string[] ids)
		{
			this.Ids = ids;
		}

		/// <summary>
		///     Gets the full IDs of the specified protections.
		/// </summary>
		/// <value>The IDs of protections.</value>
		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060002BB RID: 699 RVA: 0x000122DE File Offset: 0x000104DE
		// (set) Token: 0x060002BC RID: 700 RVA: 0x000122E6 File Offset: 0x000104E6
		public string[] Ids
		{
			get;
			private set;
		}
	}
}
