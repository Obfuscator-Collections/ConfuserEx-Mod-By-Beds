using System;

namespace Confuser.Core
{
	// Token: 0x02000051 RID: 81
	internal class PackerLogger : ILogger
	{
		// Token: 0x060001EA RID: 490 RVA: 0x0000FB6C File Offset: 0x0000DD6C
		public PackerLogger(ILogger baseLogger)
		{
			this.baseLogger = baseLogger;
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000FB7B File Offset: 0x0000DD7B
		public void Debug(string msg)
		{
			this.baseLogger.Debug(msg);
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000FB89 File Offset: 0x0000DD89
		public void DebugFormat(string format, params object[] args)
		{
			this.baseLogger.DebugFormat(format, args);
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000FB98 File Offset: 0x0000DD98
		public void Info(string msg)
		{
			this.baseLogger.Info(msg);
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000FBA6 File Offset: 0x0000DDA6
		public void InfoFormat(string format, params object[] args)
		{
			this.baseLogger.InfoFormat(format, args);
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000FBB5 File Offset: 0x0000DDB5
		public void Warn(string msg)
		{
			this.baseLogger.Warn(msg);
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000FBC3 File Offset: 0x0000DDC3
		public void WarnFormat(string format, params object[] args)
		{
			this.baseLogger.WarnFormat(format, args);
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000FBD2 File Offset: 0x0000DDD2
		public void WarnException(string msg, Exception ex)
		{
			this.baseLogger.WarnException(msg, ex);
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000FBE1 File Offset: 0x0000DDE1
		public void Error(string msg)
		{
			this.baseLogger.Error(msg);
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000FBEF File Offset: 0x0000DDEF
		public void ErrorFormat(string format, params object[] args)
		{
			this.baseLogger.ErrorFormat(format, args);
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000FBFE File Offset: 0x0000DDFE
		public void ErrorException(string msg, Exception ex)
		{
			this.baseLogger.ErrorException(msg, ex);
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000FC0D File Offset: 0x0000DE0D
		public void Progress(int progress, int overall)
		{
			this.baseLogger.Progress(progress, overall);
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000FC1C File Offset: 0x0000DE1C
		public void EndProgress()
		{
			this.baseLogger.EndProgress();
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000FC29 File Offset: 0x0000DE29
		public void Finish(bool successful)
		{
			if (!successful)
			{
				throw new ConfuserException(null);
			}
			this.baseLogger.Info("Finish protecting packer stub.");
		}

		// Token: 0x04000161 RID: 353
		private readonly ILogger baseLogger;
	}
}
