using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowDisplayService
{
    [RunInstaller(true)]
    public class WindowPlacementServiceInstaller : Installer
    {
        public WindowPlacementServiceInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            //set the privileges
            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.DisplayName = "Zack Display Service";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            serviceInstaller.ServicesDependedOn = new string[] { "TermService" };

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = "ZackDisplayService2";
            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
