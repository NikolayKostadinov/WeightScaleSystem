//---------------------------------------------------------------------------------
// <copyright file="NinjectInjector.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Application.AppStart
{
    using System;
    using System.Linq;
    using Ninject.Web.Common;
    using WeightScale.LogFileService;
    using WeightScale.CacheApi.Concrete;
    using WeightScale.CacheApi.Contract;
    using WeightScale.CacheApi.SoapProxy;
    using log4net;
    using Ninject;
    using WeightScale.Application.Contracts;
    using WeightScale.Application.Services;
    using WeightScale.ComunicationProtocol;
    using WeightScale.ComunicationProtocol.Contracts;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;

    /// <summary>
    /// Dependency injector provider
    /// </summary>
    public class NinjectInjector
    {
        private static readonly object lockObj = new object();
        private static NinjectInjector injector;
        private static IKernel kernel;

        /// <summary>
        /// Prevents a default instance of the <see cref="NinjectInjector" /> class from being created.
        /// </summary>
        private NinjectInjector()
        {
            kernel = new StandardKernel();
            InitializeKernel(kernel);
        }

        public static IKernel GetInjector
        {
            get
            {
                if (injector == null)
                {
                    lock (lockObj)
                    {
                        if (injector == null)
                        {
                            injector = new NinjectInjector();
                        }
                    }
                }

                return kernel;
            }
        }

        /// <summary>
        /// Initializes the kernel.
        /// </summary>
        public static void InitializeKernel(IKernel kernel)
        {
            kernel.Bind<IComSerializer>().To<ComSerializer>().InRequestScope();
            kernel.Bind<IChecksumService>().To<XorChecksumService>().InRequestScope();
            kernel.Bind<ICommandFactory>().To<CommandFactory>().InRequestScope();
            kernel.Bind<IComSettingsProvider>().ToMethod(context => ComSettingsProvider.GetComSettingsProvider).InRequestScope();
            kernel.Bind<IComManager>().To<ComManager>().InSingletonScope();
            kernel.Bind<IValidationMessageCollection>().To<ValidationMessageCollection>().InRequestScope();
            kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger(context.Request.Target.Member.ReflectedType)).InRequestScope();
            kernel.Bind<IMeasurementService>().To<MeasurementService>().InRequestScope();
            kernel.Bind<IWeightScaleMessageDto>().To<WeightScaleMessageDto>().InRequestScope();
            kernel.Bind<IRepository<SoapMessage, CValidationMessage>>().To<MeasurementRequestsRepository>();
            kernel.Bind<IJsonDeserializeService>().To<JsonDeserializeService>();
            kernel.Bind<IMappingService>().To<MappingService>();
            kernel.Bind<IFileService>().To<LogFileService>();
        }
    }
}
