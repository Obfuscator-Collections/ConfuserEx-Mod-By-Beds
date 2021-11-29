using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Confuser.Core
{
	/// <summary>
	///     Parameters of <see cref="T:Confuser.Core.ConfuserComponent" />.
	/// </summary>
	// Token: 0x02000079 RID: 121
	public class ProtectionParameters
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.ProtectionParameters" /> class.
		/// </summary>
		/// <param name="component">The component that this parameters applied to.</param>
		/// <param name="targets">The protection targets.</param>
		// Token: 0x060002F2 RID: 754 RVA: 0x00012645 File Offset: 0x00010845
		internal ProtectionParameters(ConfuserComponent component, IList<IDnlibDef> targets)
		{
			this.comp = component;
			this.Targets = targets;
		}

		/// <summary>
		///     Gets the targets of protection.
		///     Possible targets are module, types, methods, fields, events, properties.
		/// </summary>
		/// <value>A list of protection targets.</value>
		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060002F3 RID: 755 RVA: 0x0001265B File Offset: 0x0001085B
		// (set) Token: 0x060002F4 RID: 756 RVA: 0x00012663 File Offset: 0x00010863
		public IList<IDnlibDef> Targets
		{
			get;
			private set;
		}

		/// <summary>
		///     Obtains the value of a parameter of the specified target.
		/// </summary>
		/// <typeparam name="T">The type of the parameter value.</typeparam>
		/// <param name="context">The working context.</param>
		/// <param name="target">The protection target.</param>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="defValue">Default value if the parameter does not exist.</param>
		/// <returns>The value of the parameter.</returns>
		// Token: 0x060002F5 RID: 757 RVA: 0x0001266C File Offset: 0x0001086C
		public T GetParameter<T>(ConfuserContext context, IDnlibDef target, string name, T defValue = default(T))
		{
			if (this.comp == null)
			{
				return defValue;
			}
			if (this.comp is Packer && target == null)
			{
				target = context.Modules[0];
			}
			ProtectionSettings objParams = context.Annotations.Get<ProtectionSettings>(target, ProtectionParameters.ParametersKey, null);
			if (objParams == null)
			{
				return defValue;
			}
			Dictionary<string, string> parameters;
			if (!objParams.TryGetValue(this.comp, out parameters))
			{
				return defValue;
			}
			string ret;
			if (!parameters.TryGetValue(name, out ret))
			{
				return defValue;
			}
			Type paramType = typeof(T);
			Type nullable = Nullable.GetUnderlyingType(paramType);
			if (nullable != null)
			{
				paramType = nullable;
			}
			if (paramType.IsEnum)
			{
				return (T)((object)Enum.Parse(paramType, ret, true));
			}
			return (T)((object)Convert.ChangeType(ret, paramType));
		}

		/// <summary>
		///     Sets the protection parameters of the specified target.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="target">The protection target.</param>
		/// <param name="parameters">The parameters.</param>
		// Token: 0x060002F6 RID: 758 RVA: 0x0001271E File Offset: 0x0001091E
		public static void SetParameters(ConfuserContext context, IDnlibDef target, ProtectionSettings parameters)
		{
			context.Annotations.Set<ProtectionSettings>(target, ProtectionParameters.ParametersKey, parameters);
		}

		/// <summary>
		///     Gets the protection parameters of the specified target.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="target">The protection target.</param>
		/// <returns>The parameters.</returns>
		// Token: 0x060002F7 RID: 759 RVA: 0x00012732 File Offset: 0x00010932
		public static ProtectionSettings GetParameters(ConfuserContext context, IDnlibDef target)
		{
			return context.Annotations.Get<ProtectionSettings>(target, ProtectionParameters.ParametersKey, null);
		}

		// Token: 0x040001CE RID: 462
		private static readonly object ParametersKey = new object();

		/// <summary>
		///     A empty instance of <see cref="T:Confuser.Core.ProtectionParameters" />.
		/// </summary>
		// Token: 0x040001CF RID: 463
		public static readonly ProtectionParameters Empty = new ProtectionParameters(null, new IDnlibDef[0]);

		// Token: 0x040001D0 RID: 464
		private readonly ConfuserComponent comp;
	}
}
