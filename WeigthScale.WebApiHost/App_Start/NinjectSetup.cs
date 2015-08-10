namespace WeigthScale.WebApiHost.App_Start
{
    using System;
    using System.Linq;
    using Ninject;
    using WeightScale.Application;
    using WeightScale.Application.Contracts;
    using WeightScale.Application.Services;
    using WeightScale.ComunicationProtocol;
    using WeightScale.ComunicationProtocol.Contracts;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;
    using log4net;

    public static class NinjectSetup
    {
        public static void Config(IKernel kernel) 
        {
            kernel.Bind<IComSerializer>().To<ComSerializer>();
            kernel.Bind<IChecksumService>().To<XorChecksumService>();
            kernel.Bind<ICommandFactory>().To<CommandFactory>();
            kernel.Bind<IComSettingsProvider>().ToMethod(context => ComSettingsProvider.GetComSettingsProvider);
            kernel.Bind<IComManager>().To<ComManager>();
            kernel.Bind<IValidationMessageCollection>().To<ValidationMessageCollection>();
            kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger(context.Request.Target.Member.ReflectedType));
            kernel.Bind<IMeasurementService>().To<MeasurementService>();
            kernel.Bind<IWeightScaleMessageDto>().To<WeightScaleMessageDto>(); 
        }
    }
}
