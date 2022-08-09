using System.Management;

namespace Ingenico
{



	internal class ProcessConnection
	{
		public static ConnectionOptions ProcessConnectionOptions()
		{
			return new ConnectionOptions
			{
				Impersonation = ImpersonationLevel.Impersonate,
				Authentication = AuthenticationLevel.Default,
				EnablePrivileges = true
			};
		}

		public static ManagementScope ConnectionScope(string machineName, ConnectionOptions options, string path)
		{
			ManagementScope managementScope = new ManagementScope();
			managementScope.Path = new ManagementPath("\\\\" + machineName + path);
			managementScope.Options = options;
			managementScope.Connect();
			return managementScope;
		}
	}
}