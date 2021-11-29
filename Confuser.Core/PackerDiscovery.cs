using System;
using System.Collections.Generic;

namespace Confuser.Core
{
	// Token: 0x02000053 RID: 83
	internal class PackerDiscovery : PluginDiscovery
	{
		// Token: 0x060001FA RID: 506 RVA: 0x0000FCBC File Offset: 0x0000DEBC
		public PackerDiscovery(Protection prot)
		{
			this.prot = prot;
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000FCCB File Offset: 0x0000DECB
		protected override void GetPluginsInternal(ConfuserContext context, IList<Protection> protections, IList<Packer> packers, IList<ConfuserComponent> components)
		{
			base.GetPluginsInternal(context, protections, packers, components);
			protections.Add(this.prot);
		}

		// Token: 0x04000163 RID: 355
		private readonly Protection prot;
	}
}
