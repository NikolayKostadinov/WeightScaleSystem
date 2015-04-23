namespace WeigthScale.WebApiHost
{
    using System;
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.Linq;

    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
