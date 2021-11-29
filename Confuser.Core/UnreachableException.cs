using System;

namespace Confuser.Core
{
	/// <summary>
	///     The exception that is thrown when supposedly unreachable code is executed.
	/// </summary>
	// Token: 0x0200008B RID: 139
	public class UnreachableException : SystemException
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.UnreachableException" /> class.
		/// </summary>
		// Token: 0x06000344 RID: 836 RVA: 0x00013E84 File Offset: 0x00012084
		public UnreachableException() : base("Unreachable code reached.")
		{
		}
	}
}
