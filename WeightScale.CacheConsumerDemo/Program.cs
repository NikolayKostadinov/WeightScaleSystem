//#define Debug

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using Ninject;
using WeightScale.Application.AppStart;
using WeightScale.Application.Contracts;
using WeightScale.Application.Services;
using WeightScale.CacheApi.Contract;
using WeightScale.CacheApi.SoapProxy;
using WeightScale.Domain.Abstract;
using log4net;

namespace WeightScale.CacheConsumerDemo
{

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
#if Debug
                    DateTime beginTotal = DateTime.Now;
                    DateTime beginDateTime = DateTime.Now;
#endif
                    var result = repository.GetAll();
#if Debug
                    logger.Info(string.Format("Step 1 : repository.GetAll {0}", (DateTime.Now - beginDateTime).ToString(@"ss\:fff")));
#endif
                    if (result != null)
                    {
                        foreach (var item in result)
                        {
#if Debug
                            beginDateTime = DateTime.Now;
#endif
                            var message = injector.Get<IWeightScaleMessageDto>();
                            message.Message = item.Message.ToDomainType();
                            message.ValidationMessages = injector.Get<IValidationMessageCollection>();
#if Degug
                            logger.Info(string.Format("Step 2 : ToDomainType {0}", (DateTime.Now - beginDateTime).ToString(@"ss\:fff")));
                            beginDateTime = DateTime.Now;
#endif
                            string messageType = message.Message.GetType().Name;

                            HttpClient client = new HttpClient();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            client.DefaultRequestHeaders.Add("X-MessageType", messageType);
                            var response = client.PostAsJsonAsync(item.URL, message).Result;
#if Degug
                            logger.Info(string.Format("Step 3 : PostAsJsonAsync {0}", (DateTime.Now - beginDateTime).ToString(@"ss\:fff")));                          
#endif
                            if (response.IsSuccessStatusCode)
                            {
                                string jsonAnswer = response.Content.ReadAsStringAsync().Result;
#if Debug
                                beginDateTime = DateTime.Now;
#endif
                                IWeightScaleMessageDto incommmingMeasurementResult = deserializer.GetResultFromJson(jsonAnswer, message) as IWeightScaleMessageDto;
#if Debug
                                logger.Info(string.Format("Step 4 : deserializer.GetResultFromJson {0}", (DateTime.Now - beginDateTime).ToString(@"ss\:fff")));
#endif
                                SoapMessage currentSoap = item;
#if Debug
                                beginDateTime = DateTime.Now;
#endif
                                mapper.ToProxy(incommmingMeasurementResult, currentSoap);
#if Debug
                                logger.Info(string.Format("Step 5 : ToProxy {0}", (DateTime.Now - beginDateTime).ToString(@"ss\:fff")));
                                beginDateTime = DateTime.Now;
#endif
                                var validationMessages = repository.Update(currentSoap);
#if Debug
                                logger.Info(string.Format("Step 6 : repository.Update {0}", (DateTime.Now - beginDateTime).ToString(@"ss\:fff")));
#endif
                                if (validationMessages != null && validationMessages.Count() > 0)
                                {
                                    foreach (var vm in validationMessages)
                                    {
                                        logger.Error(string.Format("ValidationMessage: {0}",vm.ToString()));
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                            }
                        }
                    }
#if Debug
                    logger.Info(string.Format("Total estimated time for transaction: {0}", (DateTime.Now - beginTotal).ToString(@"ss\:fff")));
#endif
                } while (true);
            }
            catch (FaultException faultEx)
            {
                Console.WriteLine("An unknown exception was received. " + faultEx.Message + faultEx.StackTrace);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message + ex.StackTrace);
            }
        }
    }
}
