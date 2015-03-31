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
            foreach (var item in result)
            {
                var message = injector.Get<IWeightScaleMessageDto>();
                message.Message = item.Message.ToDomainType();
                messages.Add(message);
            }

            foreach (var message in messages)
            {
                Console.WriteLine(message.Message.ToString());
            }

            var mapper = new MappingService();

            var cMessages = new List<SoapMessage>();

            var outMessages = result.ToList();

            for (int i = 0; i < messages.Count; i++)
            {
                mapper.ToProxy(messages[i], outMessages[i]);
            }

            foreach (var item in outMessages)
            {
                Console.WriteLine(item.ToString());
                repository.Update(item);
            }
        }
    }
}
