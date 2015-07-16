using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using WeightScale.WorkstationsChecker.Data;
using WeightScale.WorkstationsChecker.Model;
using WeightScale.WorkstationsChecker.Contracts;
using log4net;
using log4net.Config;

namespace WeightScale.WorkstationsChecker
{
    public class TraceWeightScales
    {
        private static int counter = 0;
        public static ILog Logger = LogManager.GetLogger(typeof(TraceWeightScales));
        public static bool Started;

        static void Main(string[] args)
        {
            ApplicationBegin();
            Console.ReadKey();
        }

        /// <summary>
        /// Applications the begin.
        /// </summary>
        public static async void ApplicationBegin()
        {
            Mapper.CreateMap<PingReply, PingReplyView>()
               .ForMember(p => p.Buffer, opt => opt.MapFrom(p => Encoding.ASCII.GetString(p.Buffer)))
               .ForMember(p => p.Ttl, opt => opt.MapFrom(p => p.Options.Ttl))
               .ForMember(p => p.DontFragment, opt => opt.MapFrom(p => p.Options.DontFragment));

            using (var context =new UowData(new ApplicationDbContext()))
            {
                while (Started)
                {
                    List<WeightScaleWorkStation> scales = new List<WeightScaleWorkStation>();
                    try
                    {
                        scales = context.WeightScales.All().Where(x=>x.IsStopped==false).ToList();
                    }
                    catch (Exception ex)
                    {
                        Logger.Info(ex.Message + ex.StackTrace);
                    }

                    foreach (var scale in scales)
                    {
                        try
                        {
                            await StartingProcedure(scale.Address).ContinueWith(result => DisplayReply(scale.Address, result.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message + ex.StackTrace);
                        }
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }

        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <typeparam name="TResult">The type of the T result.</typeparam>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        private static void LogError(Exception result)
        {
            Logger.Error(result.Message + result.StackTrace);
        }

        private static Task<PingReply> StartingProcedure(params string[] args)
        {
            if (args.Length == 0)
                throw new ArgumentException("Ping needs a host or IP Address.");

            string who = args[0];
            AutoResetEvent waiter = new AutoResetEvent(false);

            Ping pingSender = new Ping();

            // Create a buffer of 32 bytes of data to be transmitted. 
            string data = @"{""Id"":704,""Message"":{""ExciseDocumentNumber"":null,""TotalNetOfInput"":0,""TotalNetOfOutput"":134352020,""TotalNetByProductInput"":0,""TotalNetByProductOutput"":3652960,""Number"":3,""Direction"":2,""TimeOfFirstMeasure"":""2015-06-17T08:18:51"",""TimeOfSecondMeasure"":null,""MeasurementStatus"":0,""SerialNumber"":27577,""TransactionNumber"":11188,""MeasurementNumber"":1,""ProductCode"":144,""TareWeight"":10600,""GrossWeight"":0,""NetWeight"":0,""Vehicle"":""A6784BX"",""DocumentNumber"":14970},""ValidationMessages"":[]}";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            // Wait 8 seconds for a reply. 
            int timeout = 8000;

            // Set options for transmission: 
            // The data can go through 64 gateways or routers 
            // before it is destroyed, and the data packet 
            // cannot be fragmented.
            PingOptions options = new PingOptions(64, true);

            // Send the ping asynchronously. 
            // Use the waiter as the user token. 
            // When the callback completes, it can wake up this thread.
            Task<PingReply> task =
                Task.Factory
                .StartNew(() => { return pingSender.Send(who, timeout, buffer, options); });
            task.ContinueWith(ex => LogError(ex.Exception), TaskContinuationOptions.NotOnFaulted);
            return task;

        }

        public static void DisplayReply(string address, PingReply reply)
        {
            if (reply == null)
                return;

            using (var context = new UowData(new ApplicationDbContext()))
            {
                try
                {
                    PingReplyView replyView = Mapper.Map<PingReplyView>(reply);
                    if (replyView.Status != "Success")
                    {
                        replyView.Address = address;
                        replyView.RoundtripTime = 8000L;
                    }
                    var pingPole = new PingPole() { PingReply = replyView, WeightScaleWorkStation = context.WeightScales.All().Where(x => x.Address == address).FirstOrDefault() };
                    context.Pings.Add(pingPole);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message + ex.StackTrace);
                }
            }
        }
    }
}