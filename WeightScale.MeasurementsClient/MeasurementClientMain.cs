namespace WeightScale.MeasurementsClient
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.ServiceModel;
    using Ninject;
    using WeightScale.Application.AppStart;
    using WeightScale.Application.Contracts;
    using WeightScale.CacheApi.Contract;
    using WeightScale.CacheApi.SoapProxy;
    using WeightScale.Domain.Abstract;
    using log4net;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ServiceProcess;

    class MeasurementClientMain
    {
        private static IKernel injector;
        private static IRepository<SoapMessage, CValidationMessage> repository;
        private static IJsonDeserializeService deserializer;
        private static IMappingService mapper;
        private static ILog logger;

        private static volatile bool stopProcessMeasurementThread;

        public static bool StopProcessMeasurementThread
        {
            get 
            {
                return stopProcessMeasurementThread; 
            }
            set 
            {
                stopProcessMeasurementThread = value; 
            }
        }

        static MeasurementClientMain()
        {
            AutomapperConfig.AutoMapperConfig();
            injector = NinjectInjector.GetInjector;
            repository = injector.Get<IRepository<SoapMessage, CValidationMessage>>();
            deserializer = injector.Get<IJsonDeserializeService>();
            mapper = injector.Get<IMappingService>();
            logger = LogManager.GetLogger("WeightScale.MeasurementsClient");
        }

        static void Main()
        {
            //ServiceBase[] servicesToRun;
            //servicesToRun = new ServiceBase[] 
            //{ 
            //    new MeasurementsService(logger) 
            //};
            //ServiceBase.Run(servicesToRun);

            ProcessLogs();
        }

        internal static void ProcessMeasurements()
        {
            //AutomapperConfig.AutoMapperConfig();
            //var injector = NinjectInjector.GetInjector;
            //var repository = injector.Get<IRepository<SoapMessage, CValidationMessage>>();
            //var deserializer = injector.Get<IJsonDeserializeService>();
            //var mapper = injector.Get<IMappingService>();

            try
            {
                do
                {
                    logger.Info("ProcessMeasurements");
                    DateTime beginTotal = DateTime.Now;

                    IEnumerable<SoapMessage> result = null;
                    try
                    {
                        result = repository.GetAll();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message, ex);
                    }

                    if (result != null)
                    {
                        foreach (var item in result)
                        {
                            var message = injector.Get<IWeightScaleMessageDto>();
                            message.Id = item.Id;
                            message.Message = item.Message.ToDomainType();
                            message.ValidationMessages = injector.Get<IValidationMessageCollection>();
                            string messageType = message.Message.GetType().Name;

                            HttpClient client = new HttpClient();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            client.DefaultRequestHeaders.Add("X-MessageType", messageType);
                            logger.Debug(string.Format("------------- Processing message Id: {0} -------------", message.Id));
                            logger.Debug(string.Format("Sended request message: {0} - {1}", item.URL, JsonConvert.SerializeObject(message)));
                            HttpResponseMessage response = null;
                            try
                            {
                                response = client.PostAsJsonAsync(item.URL, message).Result;
                            }
                            catch (HttpRequestException ex)
                            {
                                logger.Error(ex.Message, ex);
                            }

                            if (response != null && response.IsSuccessStatusCode)
                            {
                                string jsonAnswer = response.Content.ReadAsStringAsync().Result;
                                logger.Debug(string.Format("Received response message: {0}", jsonAnswer));
                                IWeightScaleMessageDto incommmingMeasurementResult = deserializer.GetResultFromJson(jsonAnswer, message) as IWeightScaleMessageDto;
                                SoapMessage currentSoap = item;
                                mapper.ToProxy(incommmingMeasurementResult, currentSoap);
                                var validationMessages = repository.Update(currentSoap);

                                if (validationMessages != null && validationMessages.Count() > 0)
                                {
                                    foreach (var vm in validationMessages)
                                    {
                                        logger.Error(string.Format("ValidationMessage: {0}", vm.ToString()));
                                    }
                                }
                            }
                            else
                            {
                                logger.Error(string.Format("Error Code: {0} : Message: {1}", response.StatusCode, response.ReasonPhrase));
                            }
                        }
                        logger.Info(string.Format("Total estimated time for transaction: {0}", (DateTime.Now - beginTotal).ToString(@"ss\:fff")));
                    }

                } while (!StopProcessMeasurementThread);
            }
            catch (FaultException faultEx)
            {
                logger.Error(string.Format("An unknown fault exception was received. {0} {1} {2}",
                    faultEx.Message,
                    Environment.NewLine,
                    faultEx.StackTrace));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("An unknown exception was received. {0} {1} {2}",
                    ex.Message,
                    Environment.NewLine,
                    ex.StackTrace));
            }
        }

        internal static void ProcessLogs()
        {
            try
            {
                    logger.Info("ProcessLogs");

                    var urlsList = repository.GetTargetUrls();
                    if (urlsList != null)
                    {
                        foreach (var url in urlsList)
                        {
                            Console.WriteLine(url);
 
                        }    
                    }
                    else 
                    { 
                    
                    }
            }
            catch (FaultException faultEx)
            {
                logger.Error(string.Format("An unknown fault exception was received. {0} {1} {2}",
                    faultEx.Message,
                    Environment.NewLine,
                    faultEx.StackTrace));
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("An unknown exception was received. {0} {1} {2}",
                    ex.Message,
                    Environment.NewLine,
                    ex.StackTrace));
            }
        }
    }

  
}
