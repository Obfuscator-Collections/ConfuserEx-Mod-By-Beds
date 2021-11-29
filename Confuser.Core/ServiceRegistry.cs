using System;
using System.Collections.Generic;

namespace Confuser.Core
{
	/// <summary>
	///     A registry of different services provided by protections
	/// </summary>
	// Token: 0x0200007D RID: 125
	public class ServiceRegistry : IServiceProvider
	{
		/// <inheritdoc />
		// Token: 0x06000309 RID: 777 RVA: 0x00012CD6 File Offset: 0x00010ED6
		object IServiceProvider.GetService(Type serviceType)
		{
			return this.services.GetValueOrDefault(serviceType, null);
		}

		/// <summary>
		///     Retrieves the service of type <typeparamref name="T" />.
		/// </summary>
		/// <typeparam name="T">The type of service.</typeparam>
		/// <returns>The service instance.</returns>
		// Token: 0x0600030A RID: 778 RVA: 0x00012CE5 File Offset: 0x00010EE5
		public T GetService<T>()
		{
			return (T)((object)this.services.GetValueOrDefault(typeof(T), null));
		}

		/// <summary>
		///     Registers the service with specified ID .
		/// </summary>
		/// <param name="serviceId">The service identifier.</param>
		/// <param name="serviceType">The service type.</param>
		/// <param name="service">The service.</param>
		/// <exception cref="T:System.ArgumentException">Service with same ID or type has already registered.</exception>
		// Token: 0x0600030B RID: 779 RVA: 0x00012D04 File Offset: 0x00010F04
		public void RegisterService(string serviceId, Type serviceType, object service)
		{
			if (!this.serviceIds.Add(serviceId))
			{
				throw new ArgumentException("Service with ID '" + this.serviceIds + "' has already registered.", "serviceId");
			}
			if (this.services.ContainsKey(serviceType))
			{
				throw new ArgumentException("Service with type '" + service.GetType().Name + "' has already registered.", "service");
			}
			this.services.Add(serviceType, service);
		}

		/// <summary>
		///     Determines whether the service with specified identifier has already registered.
		/// </summary>
		/// <param name="serviceId">The service identifier.</param>
		/// <returns><c>true</c> if the service with specified identifier has already registered; otherwise, <c>false</c>.</returns>
		// Token: 0x0600030C RID: 780 RVA: 0x00012D7F File Offset: 0x00010F7F
		public bool Contains(string serviceId)
		{
			return this.serviceIds.Contains(serviceId);
		}

		// Token: 0x040001EE RID: 494
		private readonly HashSet<string> serviceIds = new HashSet<string>();

		// Token: 0x040001EF RID: 495
		private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
	}
}
