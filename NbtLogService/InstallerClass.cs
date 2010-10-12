using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;




namespace CswLogServiceNbt
{
    [RunInstaller( true )]
    public partial class InstallerClass : Installer
    {
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller processInstaller;
        public InstallerClass()
        {
            InitializeComponent();

            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            // Service will run under system account
            processInstaller.Account = ServiceAccount.LocalSystem;

            // Service will have Start Type of Manual
            serviceInstaller.StartType = ServiceStartMode.Manual;

            serviceInstaller.ServiceName = "NBT Log Service";

            Installers.Add( serviceInstaller );
            Installers.Add( processInstaller );
        }
    }
}
