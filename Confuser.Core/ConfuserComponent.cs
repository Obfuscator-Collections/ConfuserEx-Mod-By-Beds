using System;

namespace Confuser.Core
{
	/// <summary>
	///     Represent a component in Confuser
	/// </summary>
	// Token: 0x02000038 RID: 56
	public abstract class ConfuserComponent
	{
		/// <summary>
		///     Gets the name of component.
		/// </summary>
		/// <value>The name of component.</value>
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600012A RID: 298
		public abstract string Name
		{
			get;
		}

		/// <summary>
		///     Gets the description of component.
		/// </summary>
		/// <value>The description of component.</value>
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600012B RID: 299
		public abstract string Description
		{
			get;
		}

		/// <summary>
		///     Gets the identifier of component used by users.
		/// </summary>
		/// <value>The identifier of component.</value>
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600012C RID: 300
		public abstract string Id
		{
			get;
		}

		/// <summary>
		///     Gets the full identifier of component used in Confuser.
		/// </summary>
		/// <value>The full identifier of component.</value>
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600012D RID: 301
		public abstract string FullId
		{
			get;
		}

		/// <summary>
		///     Initializes the component.
		/// </summary>
		/// <param name="context">The working context.</param>
		// Token: 0x0600012E RID: 302
		protected internal abstract void Initialize(ConfuserContext context);

		/// <summary>
		///     Inserts protection stages into processing pipeline.
		/// </summary>
		/// <param name="pipeline">The processing pipeline.</param>
		// Token: 0x0600012F RID: 303
		protected internal abstract void PopulatePipeline(ProtectionPipeline pipeline);
	}
}
