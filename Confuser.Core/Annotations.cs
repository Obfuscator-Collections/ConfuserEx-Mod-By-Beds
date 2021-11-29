using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Confuser.Core
{
	/// <summary>
	///     Provides methods to annotate objects.
	/// </summary>
	/// <remarks>
	///     The annotations are stored using <see cref="T:System.WeakReference" />
	/// </remarks>
	// Token: 0x02000002 RID: 2
	public class Annotations
	{
		/// <summary>
		///     Retrieves the annotation on the specified object associated with the specified key.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="obj">The object.</param>
		/// <param name="key">The key of annotation.</param>
		/// <param name="defValue">The default value if the specified annotation does not exists on the object.</param>
		/// <returns>The value of annotation, or default value if the annotation does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///     <paramref name="obj" /> or <paramref name="key" /> is <c>null</c>.
		/// </exception>
		// Token: 0x06000001 RID: 1 RVA: 0x000020D0 File Offset: 0x000002D0
		public TValue Get<TValue>(object obj, object key, TValue defValue = default(TValue))
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			ListDictionary objAnno;
			if (!this.annotations.TryGetValue(obj, out objAnno))
			{
				return defValue;
			}
			if (!objAnno.Contains(key))
			{
				return defValue;
			}
			Type valueType = typeof(TValue);
			if (valueType.IsValueType)
			{
				return (TValue)((object)Convert.ChangeType(objAnno[key], typeof(TValue)));
			}
			return (TValue)((object)objAnno[key]);
		}

		/// <summary>
		///     Retrieves the annotation on the specified object associated with the specified key.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="obj">The object.</param>
		/// <param name="key">The key of annotation.</param>
		/// <param name="defValueFactory">The default value factory function.</param>
		/// <returns>The value of annotation, or default value if the annotation does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///     <paramref name="obj" /> or <paramref name="key" /> is <c>null</c>.
		/// </exception>
		// Token: 0x06000002 RID: 2 RVA: 0x00002154 File Offset: 0x00000354
		public TValue GetLazy<TValue>(object obj, object key, Func<object, TValue> defValueFactory)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			ListDictionary objAnno;
			if (!this.annotations.TryGetValue(obj, out objAnno))
			{
				return defValueFactory(key);
			}
			if (!objAnno.Contains(key))
			{
				return defValueFactory(key);
			}
			Type valueType = typeof(TValue);
			if (valueType.IsValueType)
			{
				return (TValue)((object)Convert.ChangeType(objAnno[key], typeof(TValue)));
			}
			return (TValue)((object)objAnno[key]);
		}

		/// <summary>
		///     Retrieves or create the annotation on the specified object associated with the specified key.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="obj">The object.</param>
		/// <param name="key">The key of annotation.</param>
		/// <param name="factory">The factory function to create the annotation value when the annotation does not exist.</param>
		/// <returns>The value of annotation, or the newly created value.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///     <paramref name="obj" /> or <paramref name="key" /> is <c>null</c>.
		/// </exception>
		// Token: 0x06000003 RID: 3 RVA: 0x000021E4 File Offset: 0x000003E4
		public TValue GetOrCreate<TValue>(object obj, object key, Func<object, TValue> factory)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			ListDictionary objAnno;
			if (!this.annotations.TryGetValue(obj, out objAnno))
			{
				objAnno = (this.annotations[new Annotations.WeakReferenceKey(obj)] = new ListDictionary());
			}
			if (!objAnno.Contains(key))
			{
				TValue ret;
				objAnno[key] = (ret = factory(key));
				return ret;
			}
			Type valueType = typeof(TValue);
			if (valueType.IsValueType)
			{
				return (TValue)((object)Convert.ChangeType(objAnno[key], typeof(TValue)));
			}
			return (TValue)((object)objAnno[key]);
		}

		/// <summary>
		///     Sets an annotation on the specified object.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="obj">The object.</param>
		/// <param name="key">The key of annotation.</param>
		/// <param name="value">The value of annotation.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///     <paramref name="obj" /> or <paramref name="key" /> is <c>null</c>.
		/// </exception>
		// Token: 0x06000004 RID: 4 RVA: 0x00002294 File Offset: 0x00000494
		public void Set<TValue>(object obj, object key, TValue value)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			ListDictionary objAnno;
			if (!this.annotations.TryGetValue(obj, out objAnno))
			{
				objAnno = (this.annotations[new Annotations.WeakReferenceKey(obj)] = new ListDictionary());
			}
			objAnno[key] = value;
		}

		/// <summary>
		///     Trims the annotations of unreachable objects from this instance.
		/// </summary>
		// Token: 0x06000005 RID: 5 RVA: 0x00002314 File Offset: 0x00000514
		public void Trim()
		{
			foreach (object key in from kvp in this.annotations
			where !((Annotations.WeakReferenceKey)kvp.Key).IsAlive
			select kvp.Key)
			{
				this.annotations.Remove(key);
			}
		}

		// Token: 0x04000001 RID: 1
		private readonly Dictionary<object, ListDictionary> annotations = new Dictionary<object, ListDictionary>(Annotations.WeakReferenceComparer.Instance);

		/// <summary>
		///     Equality comparer of weak references.
		/// </summary>
		// Token: 0x02000003 RID: 3
		private class WeakReferenceComparer : IEqualityComparer<object>
		{
			/// <summary>
			///     Prevents a default instance of the <see cref="T:Confuser.Core.Annotations.WeakReferenceComparer" /> class from being created.
			/// </summary>
			// Token: 0x06000009 RID: 9 RVA: 0x000023C4 File Offset: 0x000005C4
			private WeakReferenceComparer()
			{
			}

			/// <inheritdoc />
			// Token: 0x0600000A RID: 10 RVA: 0x000023CC File Offset: 0x000005CC
			public bool Equals(object x, object y)
			{
				if (y is Annotations.WeakReferenceKey && !(x is WeakReference))
				{
					return this.Equals(y, x);
				}
				Annotations.WeakReferenceKey xWeak = x as Annotations.WeakReferenceKey;
				Annotations.WeakReferenceKey yWeak = y as Annotations.WeakReferenceKey;
				if (xWeak != null && yWeak != null)
				{
					return xWeak.IsAlive && yWeak.IsAlive && object.ReferenceEquals(xWeak.Target, yWeak.Target);
				}
				if (xWeak != null && yWeak == null)
				{
					return xWeak.IsAlive && object.ReferenceEquals(xWeak.Target, y);
				}
				if (xWeak == null && yWeak == null)
				{
					return xWeak.IsAlive && object.ReferenceEquals(xWeak.Target, y);
				}
				throw new UnreachableException();
			}

			/// <inheritdoc />
			// Token: 0x0600000B RID: 11 RVA: 0x00002469 File Offset: 0x00000669
			public int GetHashCode(object obj)
			{
				if (obj is Annotations.WeakReferenceKey)
				{
					return ((Annotations.WeakReferenceKey)obj).HashCode;
				}
				return obj.GetHashCode();
			}

			/// <summary>
			///     The singleton instance of this comparer.
			/// </summary>
			// Token: 0x04000004 RID: 4
			public static readonly Annotations.WeakReferenceComparer Instance = new Annotations.WeakReferenceComparer();
		}

		/// <summary>
		///     Represent a key using <see cref="T:System.WeakReference" />.
		/// </summary>
		// Token: 0x02000004 RID: 4
		private class WeakReferenceKey : WeakReference
		{
			/// <inheritdoc />
			// Token: 0x0600000D RID: 13 RVA: 0x00002491 File Offset: 0x00000691
			public WeakReferenceKey(object target) : base(target)
			{
				this.HashCode = target.GetHashCode();
			}

			/// <summary>
			///     Gets the hash code of the target object.
			/// </summary>
			/// <value>The hash code.</value>
			// Token: 0x17000001 RID: 1
			// (get) Token: 0x0600000E RID: 14 RVA: 0x000024A6 File Offset: 0x000006A6
			// (set) Token: 0x0600000F RID: 15 RVA: 0x000024AE File Offset: 0x000006AE
			public int HashCode
			{
				get;
				private set;
			}
		}
	}
}
