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
        private static void InitializeKernel(IKernel kernel)
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
