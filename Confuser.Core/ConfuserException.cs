using System;

namespace Confuser.Core
{
	/// <summary>
	///     The exception that is thrown when a handled error occurred during the protection process.
	/// </summary>
	// Token: 0x02000036 RID: 54
	public class ConfuserException : Exception
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.ConfuserException" /> class.
		/// </summary>
		/// <param name="innerException">The inner exception, or null if no exception is associated with the error.</param>
		// Token: 0x0600011B RID: 283 RVA: 0x0000A634 File Offset: 0x00008834
		public ConfuserException(Exception innerException) : base("Exception occurred during the protection process.", innerException)
		{
		}
	}
}
