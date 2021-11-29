using System;

namespace Confuser.Core
{
	/// <summary>
	///     Defines a logger used to log Confuser events
	/// </summary>
	// Token: 0x02000042 RID: 66
	public interface ILogger
	{
		/// <summary>
		///     Logs a message at DEBUG level.
		/// </summary>
		/// <param name="msg">The message.</param>
		// Token: 0x06000183 RID: 387
		void Debug(string msg);

		/// <summary>
		///     Logs a message at DEBUG level with specified parameters.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		// Token: 0x06000184 RID: 388
		void DebugFormat(string format, params object[] args);

		/// <summary>
		///     Logs a message at INFO level.
		/// </summary>
		/// <param name="msg">The message.</param>
		// Token: 0x06000185 RID: 389
		void Info(string msg);

		/// <summary>
		///     Logs a message at INFO level with specified parameters.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		// Token: 0x06000186 RID: 390
		void InfoFormat(string format, params object[] args);

		/// <summary>
		///     Logs a message at WARN level.
		/// </summary>
		/// <param name="msg">The message.</param>
		// Token: 0x06000187 RID: 391
		void Warn(string msg);

		/// <summary>
		///     Logs a message at WARN level with specified parameters.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		// Token: 0x06000188 RID: 392
		void WarnFormat(string format, params object[] args);

		/// <summary>
		///     Logs a message at WARN level with specified exception.
		/// </summary>
		/// <param name="msg">The message.</param>
		/// <param name="ex">The exception.</param>
		// Token: 0x06000189 RID: 393
		void WarnException(string msg, Exception ex);

		/// <summary>
		///     Logs a message at ERROR level.
		/// </summary>
		/// <param name="msg">The message.</param>
		// Token: 0x0600018A RID: 394
		void Error(string msg);

		/// <summary>
		///     Logs a message at ERROR level with specified parameters.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		// Token: 0x0600018B RID: 395
		void ErrorFormat(string format, params object[] args);

		/// <summary>
		///     Logs a message at ERROR level with specified exception.
		/// </summary>
		/// <param name="msg">The message.</param>
		/// <param name="ex">The exception.</param>
		// Token: 0x0600018C RID: 396
		void ErrorException(string msg, Exception ex);

		/// <summary>
		///     Logs the progress of protection.
		/// </summary>
		/// <remarks>
		///     This method is intended to be used with <see cref="M:Confuser.Core.ILogger.EndProgress" />.
		/// </remarks>
		/// <example>
		///     <code> 
		///         for (int i = 0; i &lt; defs.Length; i++) {
		///             logger.Progress(i + 1, defs.Length);
		///         }
		///         logger.EndProgress();
		///     </code>
		/// </example>
		/// <param name="overall">The total work amount .</param>
		/// <param name="progress">The amount of work done.</param>
		// Token: 0x0600018D RID: 397
		void Progress(int progress, int overall);

		/// <summary>
		///     End the progress of protection.
		/// </summary>
		/// <seealso cref="M:Confuser.Core.ILogger.Progress(System.Int32,System.Int32)" />
		// Token: 0x0600018E RID: 398
		void EndProgress();

		/// <summary>
		///     Logs the finish of protection.
		/// </summary>
		/// <param name="successful">Indicated whether the protection process is successful.</param>
		// Token: 0x0600018F RID: 399
		void Finish(bool successful);
	}
}
