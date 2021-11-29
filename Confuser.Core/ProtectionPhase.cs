using System;

namespace Confuser.Core
{
	/// <summary>
	///     Base class of protection phases.
	/// </summary>
	// Token: 0x02000075 RID: 117
	public abstract class ProtectionPhase
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.ProtectionPhase" /> class.
		/// </summary>
		/// <param name="parent">The parent component of this phase.</param>
		// Token: 0x060002BD RID: 701 RVA: 0x000122EF File Offset: 0x000104EF
		public ProtectionPhase(ConfuserComponent parent)
		{
			this.Parent = parent;
		}

		/// <summary>
		///     Gets the parent component.
		/// </summary>
		/// <value>The parent component.</value>
		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060002BE RID: 702 RVA: 0x000122FE File Offset: 0x000104FE
		// (set) Token: 0x060002BF RID: 703 RVA: 0x00012306 File Offset: 0x00010506
		public ConfuserComponent Parent
		{
			get;
			private set;
		}

		/// <summary>
		///     Gets the targets of protection.
		/// </summary>
		/// <value>The protection targets.</value>
		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060002C0 RID: 704
		public abstract ProtectionTargets Targets
		{
			get;
		}

		/// <summary>
		///     Gets the name of the phase.
		/// </summary>
		/// <value>The name of phase.</value>
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060002C1 RID: 705
		public abstract string Name
		{
			get;
		}

		/// <summary>
		///     Gets a value indicating whether this phase process all targets, not just the targets that requires the component.
		/// </summary>
		/// <value><c>true</c> if this phase process all targets; otherwise, <c>false</c>.</value>
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060002C2 RID: 706 RVA: 0x0001230F File Offset: 0x0001050F
		public virtual bool ProcessAll
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		///     Executes the protection phase.
		/// </summary>
		/// <param name="context">The working context.</param>
		/// <param name="parameters">The parameters of protection.</param>
		// Token: 0x060002C3 RID: 707
		protected internal abstract void Execute(ConfuserContext context, ProtectionParameters parameters);
	}
}
