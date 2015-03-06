using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WeightScale.ComunicationProtocol.Contracts;
using WeightScale.Domain.Abstract;

namespace WeightScale.Application.Services
{
    public class MeasurementService : WeightScale.Application.Services.IMeasurementService
    {
        private const int INTERVAL = 60 * 1000;
        private bool wDTbreak = false;
        private bool received = false;
        private IWeightScaleMessage comInputMessage;
        ICommandFactory command;
        Timer WDTimer;

        MeasurementService(ICommandFactory commandParam) 
        {
            this.command = commandParam;
            WDTimer = new Timer(INTERVAL);
            WDTimer.Elapsed += WDTimer_Elapsed;

        }

        void WDTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.wDTbreak = true;
        }

        public IWeightScaleMessage Measure(IWeightScaleMessage message)
        {
            var comport = TryToOpenComport();
            var cmd = this.command.WeightScaleRequest(message);
            //comport.Receive += ComportReceive();
            //comport.Send(cmd);
            this.wDTbreak = false;
            this.WDTimer.Start();
            while (this.received || this.wDTbreak) 
            {
                //do nothing
            }

            return null;
        }

        void ComportReceive(object sender, ElapsedEventArgs e)
        {
            //TODO: Stop timer and receive message
        }
 
        /// <summary>
        /// Tries to open comport.
        /// </summary>
        /// <returns></returns>
        private object TryToOpenComport()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}
