using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightScale.Application;
using WeightScale.Application.AppStart;
using Ninject;
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
            var result = repository.GetAll();
            List<IWeightScaleMessageDto> messages = new List<IWeightScaleMessageDto>();
            var mapper = new MappingService();
            foreach (var item in result)
            {
                var message = injector.Get<IWeightScaleMessageDto>();
                message.Message = item.Message.ToDomainType();
                messages.Add(message);
                // Get result from HttpClient
                
                SoapMessage currentSoap = item;
                mapper.ToProxy(message, currentSoap);
                repository.Update(currentSoap);
            }

            foreach (var message in messages)
            {
                Console.WriteLine(message.Message.ToString());
            }
            
        }
    }
}
