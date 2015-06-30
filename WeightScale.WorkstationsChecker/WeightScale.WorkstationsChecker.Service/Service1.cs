using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeightScale.WorkstationsChecker;
using log4net;

namespace WeightScale.WorkstationsChecker.Service
{
    public partial class Service1 : ServiceBase
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Service1));
        private static bool started = true;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            TraceWeightScales.Started = true;
            TraceWeightScales.Logger = logger;
                try
                {
                    logger.Info("Strating service");
                    TraceWeightScales.ApplicationBegin();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message + "\n" + ex.StackTrace);
                }
        }

        protected override void OnStop()
        {
            started = false;
            logger.Info("Service stopped");
        }
    }
}
