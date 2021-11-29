using System;

namespace Confuser.Core
{
	/// <summary>
	///     The exception that is thrown when there exists circular dependency between protections.
	/// </summary>
	// Token: 0x0200003C RID: 60
	internal class CircularDependencyException : Exception
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.CircularDependencyException" /> class.
		/// </summary>
		/// <param name="a">The first protection.</param>
		/// <param name="b">The second protection.</param>
		// Token: 0x06000144 RID: 324 RVA: 0x0000AD73 File Offset: 0x00008F73
		internal CircularDependencyException(Protection a, Protection b) : base(string.Format("The protections '{0}' and '{1}' has a circular dependency between them.", a, b))
		{
			this.ProtectionA = a;
			this.ProtectionB = b;
		}

		/// <summary>
		///     First protection that involved in circular dependency.
		/// </summary>
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000145 RID: 325 RVA: 0x0000AD95 File Offset: 0x00008F95
		// (set) Token: 0x06000146 RID: 326 RVA: 0x0000AD9D File Offset: 0x00008F9D
		public Protection ProtectionA
		{
			get;
			private set;
		}

		/// <summary>
		///     Second protection that involved in circular dependency.
		/// </summary>
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000147 RID: 327 RVA: 0x0000ADA6 File Offset: 0x00008FA6
		// (set) Token: 0x06000148 RID: 328 RVA: 0x0000ADAE File Offset: 0x00008FAE
		public Protection ProtectionB
		{
			get;
			private set;
		}
	}
}
