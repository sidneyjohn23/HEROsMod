using System;
using System.Collections.Generic;

namespace HEROsMod.HEROsModServices
{
	internal class ServiceController
	{
		public delegate void ServiceEventHandler(HEROsModService service);

		public event ServiceEventHandler ServiceAdded;

		public event ServiceEventHandler ServiceRemoved;

		/// <summary>
		/// HEROsMod Services laoded into the controller
		/// </summary>
		public List<HEROsModService> Services { get; }

		public ServiceController()
		{
			Services = new List<HEROsModService>();
			HEROsModNetwork.LoginService.MyGroupChanged += LoginService_MyGroupChanged;
		}

		private void LoginService_MyGroupChanged(object sender, EventArgs e) => MyGroupChanged();

		public void MyGroupChanged()
		{
			if (HEROsModNetwork.LoginService.MyGroup != null)
			{
				foreach (HEROsModService service in Services)
				{
					service.MyGroupUpdated();
				}
				ServiceRemoved(null);
			}
		}

		/// <summary>
		/// Add a HEROsModService to the ServiceController
		/// </summary>
		/// <param name="service">Service to add</param>
		public void AddService(HEROsModService service)
		{
			Services.Add(service);
			ServiceAdded?.Invoke(service);
		}

		/// <summary>
		/// Remove a HEROsModService from the ServiceController
		/// </summary>
		/// <param name="service">Service to Remove</param>
		public void RemoveService(HEROsModService service)
		{
			service.Destroy();
			Services.Remove(service);
			ServiceRemoved?.Invoke(service);
		}

		/// <summary>
		/// Remove all HEROsModServices from the ServiceController
		/// </summary>
		public void RemoveAllServices()
		{
			while (Services.Count > 0)
			{
				RemoveService(Services[0]);
			}
		}

		internal void ServiceRemovedCall() => ServiceRemoved(null);
	}
}