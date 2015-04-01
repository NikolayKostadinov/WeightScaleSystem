using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Ninject;
using WeightScale.Application.AppStart;
using WeightScale.Application.Contracts;
using WeightScale.Application.Services;
using WeightScale.CacheApi.Contract;
using WeightScale.CacheApi.SoapProxy;
using WeightScale.Domain.Abstract;

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
            do
            {
                var result = repository.GetAll();
                var mapper = new MappingService();

                if (result != null)
                {
                    foreach (var item in result)
                    {
                        var message = injector.Get<IWeightScaleMessageDto>();
                        message.Message = item.Message.ToDomainType();
                        message.ValidationMessages = injector.Get<IValidationMessageCollection>();
                        string messageType = message.Message.GetType().Name;
                        // Get result from HttpClient
                        HttpClient client = new HttpClient();
                        //client.BaseAddress = new Uri(""http://10.94.23.144:8111/api/Measurements/PostMeasurement"");

                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("X-MessageType", messageType);
                         var response = client.PostAsJsonAsync(item.URL, message).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string jsonAnswer = response.Content.ReadAsStringAsync().Result;
                            IWeightScaleMessageDto incommmingMeasurementResult = deserializer.GetResultFromJson(jsonAnswer, message) as IWeightScaleMessageDto;
                            //var dto = JsonConvert.DeserializeObject<WeightScaleMessageDto>(json);
                            //var dtoMessage = JObject.Parse(json).Root["Message"].ToString();
                            //var dtoValidationMessages = JObject.Parse(json).Root["ValidationMessages"].ToString();

                            SoapMessage currentSoap = item;
                            mapper.ToProxy(incommmingMeasurementResult, currentSoap);
                            var validationMessages = repository.Update(currentSoap);
                            if (validationMessages !=null && validationMessages.Count() > 0)
                            {
                                foreach (var vm in validationMessages)
                                {
                                    Console.WriteLine(vm.ToString());   
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                        }
                    }
                }

                System.Threading.Thread.Sleep(1000);

            } while (true);
            //foreach (var message in messages)
            //{
            //    Console.WriteLine(message.Message.ToString());
            //}
        }
    }
}
