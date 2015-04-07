namespace WeightScale.CacheConsumerDemo
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

    class Program
    {
        static void Main(string[] args)
        {
            AutomapperConfig.AutoMapperConfig();
            var injector = NinjectInjector.GetInjector;
            var repository = injector.Get<IRepository<SoapMessage, CValidationMessage>>();
            var deserializer = injector.Get<IJsonDeserializeService>();
            var mapper = injector.Get<IMappingService>();
            ILog logger = LogManager.GetLogger("WeightScale.CacheConsumerDemo");
            try
            {
                do
                {
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
                            logger.Debug(string.Format("Sended request message: {0}", JsonConvert.SerializeObject(message)));
                            var response = client.PostAsJsonAsync(item.URL, message).Result;

                            if (response.IsSuccessStatusCode)
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
                    }

                    logger.Info(string.Format("Total estimated time for transaction: {0}", (DateTime.Now - beginTotal).ToString(@"ss\:fff")));
                    System.Threading.Thread.Sleep(2000);

                } while (true);
            }
            catch (FaultException faultEx)
            {
                Console.WriteLine("An unknown exception was received. " + faultEx.Message + faultEx.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message + ex.StackTrace);
            }
        }
    }
}
