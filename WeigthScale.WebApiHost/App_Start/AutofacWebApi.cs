using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using WeightScale.Application;
using WeightScale.Application.Contracts;
using WeightScale.Application.Services;
using WeightScale.ComunicationProtocol;
using WeightScale.ComunicationProtocol.Contracts;
using WeightScale.Domain.Abstract;
using WeightScale.Domain.Common;
using WeightScale.WebApi.Controllers;
using log4net;

namespace WeigthScale.WebApiHost.App_Start
{
    public class AutofacWebApi
    {

            public static void Initialize(HttpConfiguration config)
            {

                Initialize(config,
                    RegisterServices(new ContainerBuilder()));
            }

            public static void Initialize(HttpConfiguration config, IContainer container)
            {

                config.DependencyResolver =
                    new AutofacWebApiDependencyResolver(container);
            }

            private static IContainer RegisterServices(ContainerBuilder builder)
            {

                builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

                // EF DbContext
                builder.RegisterType<ComSerializer>().As<IComSerializer>();//.InstancePerApiControllerType(typeof(MeasurementsController));
                builder.RegisterType<XorChecksumService>().As<IChecksumService>();//.InstancePerApiControllerType(typeof(MeasurementsController));
                builder.RegisterType<CommandFactory>().As<ICommandFactory>();//.InstancePerApiControllerType(typeof(MeasurementsController));
                builder.Register(context => ComSettingsProvider.GetComSettingsProvider).As<IComSettingsProvider>();//.InstancePerApiControllerType(typeof(MeasurementsController));
                builder.RegisterType<ComManager>().As<IComManager>();//.InstancePerApiControllerType(typeof(MeasurementsController));
                builder.RegisterType<ValidationMessageCollection>().As<IValidationMessageCollection>();//.InstancePerApiControllerType(typeof(MeasurementsController));
                builder.Register(context => LogManager.GetLogger("WeigthScale.WebApiHost.Program")).As<ILog>();//.InstancePerApiControllerType(typeof(MeasurementsController));
                builder.RegisterType<MeasurementService>().As<IMeasurementService>();//.InstancePerApiControllerType(typeof(MeasurementsController));
                builder.RegisterType<WeightScaleMessageDto>().As<IWeightScaleMessageDto>();//.InstancePerApiControllerType(typeof(MeasurementsController));

                return builder.Build();
            }
    }
}
