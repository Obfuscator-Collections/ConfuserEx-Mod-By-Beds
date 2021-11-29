using System;
using System.Collections.Generic;
using System.Linq;

namespace Confuser.Core
{
	/// <summary>
	///     Resolves dependency between protections.
	/// </summary>
	// Token: 0x0200003A RID: 58
	internal class DependencyResolver
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.DependencyResolver" /> class.
		/// </summary>
		/// <param name="protections">The protections for resolution.</param>
		// Token: 0x06000138 RID: 312 RVA: 0x0000A7F7 File Offset: 0x000089F7
		public DependencyResolver(IEnumerable<Protection> protections)
		{
			this.protections = (from prot in protections
			orderby prot.FullId
			select prot).ToList<Protection>();
		}

		/// <summary>
		///     Sort the protection according to their dependency.
		/// </summary>
		/// <returns>Sorted protections with respect to dependencies.</returns>
		/// <exception cref="T:CircularDependencyException">
		///     The protections contain circular dependencies.
		/// </exception>
		// Token: 0x06000139 RID: 313 RVA: 0x0000A85C File Offset: 0x00008A5C
		public IList<Protection> SortDependency()
		{
			List<DependencyResolver.DependencyGraphEdge> edges = new List<DependencyResolver.DependencyGraphEdge>();
			HashSet<Protection> roots = new HashSet<Protection>(this.protections);
			Dictionary<string, Protection> id2prot = this.protections.ToDictionary((Protection prot) => prot.FullId, (Protection prot) => prot);
			foreach (Protection prot2 in this.protections)
			{
				Type protType = prot2.GetType();
				BeforeProtectionAttribute before = protType.GetCustomAttributes(typeof(BeforeProtectionAttribute), false).Cast<BeforeProtectionAttribute>().SingleOrDefault<BeforeProtectionAttribute>();
				if (before != null)
				{
					IEnumerable<Protection> targets = from id in before.Ids
					select id2prot[id];
					foreach (Protection target in targets)
					{
						edges.Add(new DependencyResolver.DependencyGraphEdge(prot2, target));
						roots.Remove(target);
					}
				}
				AfterProtectionAttribute after = protType.GetCustomAttributes(typeof(AfterProtectionAttribute), false).Cast<AfterProtectionAttribute>().SingleOrDefault<AfterProtectionAttribute>();
				if (after != null)
				{
					IEnumerable<Protection> targets2 = from id in after.Ids
					select id2prot[id];
					foreach (Protection target2 in targets2)
					{
						edges.Add(new DependencyResolver.DependencyGraphEdge(target2, prot2));
						roots.Remove(prot2);
					}
				}
			}
			IEnumerable<Protection> sorted = this.SortGraph(roots, edges);
			return sorted.ToList<Protection>();
		}

		/// <summary>
		///     Topologically sort the dependency graph.
		/// </summary>
		/// <param name="roots">The root protections.</param>
		/// <param name="edges">The dependency graph edges.</param>
		/// <returns>Topological sorted protections.</returns>
		// Token: 0x0600013A RID: 314 RVA: 0x0000AD10 File Offset: 0x00008F10
		private IEnumerable<Protection> SortGraph(IEnumerable<Protection> roots, IList<DependencyResolver.DependencyGraphEdge> edges)
		{
			Queue<Protection> queue = new Queue<Protection>(from prot in roots
			orderby prot.FullId
			select prot);
			while (queue.Count > 0)
			{
				Protection root = queue.Dequeue();
				yield return root;
				using (List<DependencyResolver.DependencyGraphEdge>.Enumerator enumerator = (from edge in edges
				where edge.From == root
				select edge).ToList<DependencyResolver.DependencyGraphEdge>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DependencyResolver.DependencyGraphEdge edge = enumerator.Current;
						edges.Remove(edge);
						if (!edges.Any((DependencyResolver.DependencyGraphEdge e) => e.To == edge.To))
						{
							queue.Enqueue(edge.To);
						}
					}
				}
			}
			if (edges.Count != 0)
			{
				throw new CircularDependencyException(edges[0].From, edges[0].To);
			}
			yield break;
		}

		// Token: 0x0400011F RID: 287
		private readonly List<Protection> protections;

		/// <summary>
		///     An edge of dependency graph.
		/// </summary>
		// Token: 0x0200003B RID: 59
		private class DependencyGraphEdge
		{
			/// <summary>
			///     Initializes a new instance of the <see cref="T:Confuser.Core.DependencyResolver.DependencyGraphEdge" /> class.
			/// </summary>
			/// <param name="from">The source protection node.</param>
			/// <param name="to">The destination protection node.</param>
			// Token: 0x0600013F RID: 319 RVA: 0x0000AD3B File Offset: 0x00008F3B
			public DependencyGraphEdge(Protection from, Protection to)
			{
				this.From = from;
				this.To = to;
			}

			/// <summary>
			///     The source protection node.
			/// </summary>
			// Token: 0x17000018 RID: 24
			// (get) Token: 0x06000140 RID: 320 RVA: 0x0000AD51 File Offset: 0x00008F51
			// (set) Token: 0x06000141 RID: 321 RVA: 0x0000AD59 File Offset: 0x00008F59
			public Protection From
			{
				get;
				private set;
			}

			/// <summary>
			///     The destination protection node.
			/// </summary>
			// Token: 0x17000019 RID: 25
			// (get) Token: 0x06000142 RID: 322 RVA: 0x0000AD62 File Offset: 0x00008F62
			// (set) Token: 0x06000143 RID: 323 RVA: 0x0000AD6A File Offset: 0x00008F6A
			public Protection To
			{
				get;
				private set;
			}
		}
	}
}
