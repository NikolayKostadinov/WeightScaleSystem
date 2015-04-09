namespace WeightScale.MeasurementsClient
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.measurementsServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.measurementsServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // measurementsServiceProcessInstaller
            // 
            this.measurementsServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.measurementsServiceProcessInstaller.Password = null;
            this.measurementsServiceProcessInstaller.Username = null;
            // 
            // measurementsServiceInstaller
            // 
            this.measurementsServiceInstaller.Description = "Processing measuring data form Intersystems Cache";
            this.measurementsServiceInstaller.DisplayName = "Measurements Client";
            this.measurementsServiceInstaller.ServiceName = "MeasurementsService";
            this.measurementsServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.measurementsServiceProcessInstaller,
            this.measurementsServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller measurementsServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller measurementsServiceInstaller;
    }
}