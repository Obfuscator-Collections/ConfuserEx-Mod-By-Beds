using System;
using System.Collections.Generic;

namespace Confuser.Core
{
	/// <summary>
	///     Protection settings for a certain component
	/// </summary>
	// Token: 0x02000076 RID: 118
	public class ProtectionSettings : Dictionary<ConfuserComponent, Dictionary<string, string>>
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.ProtectionSettings" /> class.
		/// </summary>
		// Token: 0x060002C4 RID: 708 RVA: 0x00012312 File Offset: 0x00010512
		public ProtectionSettings()
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.ProtectionSettings" /> class
		///     from an existing <see cref="T:Confuser.Core.ProtectionSettings" />.
		/// </summary>
		/// <param name="settings">The settings to copy from.</param>
		// Token: 0x060002C5 RID: 709 RVA: 0x0001231C File Offset: 0x0001051C
		public ProtectionSettings(ProtectionSettings settings)
		{
			foreach (KeyValuePair<ConfuserComponent, Dictionary<string, string>> i in settings)
			{
				base.Add(i.Key, new Dictionary<string, string>(i.Value));
			}
		}

		/// <summary>
		///     Determines whether the settings is empty.
		/// </summary>
		/// <returns><c>true</c> if the settings is empty; otherwise, <c>false</c>.</returns>
		// Token: 0x060002C6 RID: 710 RVA: 0x00012384 File Offset: 0x00010584
		public bool IsEmpty()
		{
			return base.Count == 0;
		}
	}
}
