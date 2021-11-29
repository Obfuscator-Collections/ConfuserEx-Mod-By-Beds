using System;
using dnlib.DotNet;

namespace Confuser.Core
{
	/// <summary>
	///     An <see cref="T:Confuser.Core.ILogger" /> implementation that doesn't actually do any logging.
	/// </summary>
	// Token: 0x0200004F RID: 79
	internal class NullLogger : ILogger
	{
		/// <summary>
		///     Prevents a default instance of the <see cref="T:Confuser.Core.NullLogger" /> class from being created.
		/// </summary>
		// Token: 0x060001D6 RID: 470 RVA: 0x0000F878 File Offset: 0x0000DA78
		private NullLogger()
		{
		}

		/// <inheritdoc />
		// Token: 0x060001D7 RID: 471 RVA: 0x0000F880 File Offset: 0x0000DA80
		public void Debug(string msg)
		{
		}

		/// <inheritdoc />
		// Token: 0x060001D8 RID: 472 RVA: 0x0000F882 File Offset: 0x0000DA82
		public void DebugFormat(string format, params object[] args)
		{
		}

		/// <inheritdoc />
		// Token: 0x060001D9 RID: 473 RVA: 0x0000F884 File Offset: 0x0000DA84
		public void Info(string msg)
		{
		}

		/// <inheritdoc />
		// Token: 0x060001DA RID: 474 RVA: 0x0000F886 File Offset: 0x0000DA86
		public void InfoFormat(string format, params object[] args)
		{
		}

		/// <inheritdoc />
		// Token: 0x060001DB RID: 475 RVA: 0x0000F888 File Offset: 0x0000DA88
		public void Warn(string msg)
		{
		}

		/// <inheritdoc />
		// Token: 0x060001DC RID: 476 RVA: 0x0000F88A File Offset: 0x0000DA8A
		public void WarnFormat(string format, params object[] args)
		{
		}

		/// <inheritdoc />
		// Token: 0x060001DD RID: 477 RVA: 0x0000F88C File Offset: 0x0000DA8C
		public void WarnException(string msg, Exception ex)
		{
		}

		/// <inheritdoc />
		// Token: 0x060001DE RID: 478 RVA: 0x0000F88E File Offset: 0x0000DA8E
		public void Error(string msg)
		{
		}

		/// <inheritdoc />
		// Token: 0x060001DF RID: 479 RVA: 0x0000F890 File Offset: 0x0000DA90
		public void ErrorFormat(string format, params object[] args)
		{
		}

		/// <inheritdoc />
		// Token: 0x060001E0 RID: 480 RVA: 0x0000F892 File Offset: 0x0000DA92
		public void ErrorException(string msg, Exception ex)
		{
		}

		/// <inheritdoc />
		// Token: 0x060001E1 RID: 481 RVA: 0x0000F894 File Offset: 0x0000DA94
		public void Progress(int overall, int progress)
		{
		}

		/// <inheritdoc />
		// Token: 0x060001E2 RID: 482 RVA: 0x0000F896 File Offset: 0x0000DA96
		public void EndProgress()
		{
		}

		/// <inheritdoc />
		// Token: 0x060001E3 RID: 483 RVA: 0x0000F898 File Offset: 0x0000DA98
		public void Finish(bool successful)
		{
		}

		/// <inheritdoc />
		// Token: 0x060001E4 RID: 484 RVA: 0x0000F89A File Offset: 0x0000DA9A
		public void BeginModule(ModuleDef module)
		{
		}

		/// <inheritdoc />
		// Token: 0x060001E5 RID: 485 RVA: 0x0000F89C File Offset: 0x0000DA9C
		public void EndModule(ModuleDef module)
		{
		}

		/// <summary>
		///     The singleton instance of <see cref="T:Confuser.Core.NullLogger" />.
		/// </summary>
		// Token: 0x04000160 RID: 352
		public static readonly NullLogger Instance = new NullLogger();
	}
}
