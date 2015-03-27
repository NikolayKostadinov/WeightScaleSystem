//---------------------------------------------------------------------------------
// <copyright file="NinjectInjector.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeigthScale.WebApiHost.App_Start
{
    using System;
    using System.Linq;
    using System.Web;
    using WeightScale.Application;
    using log4net;
    using Ninject;
    using Ninject.Web.Common;
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
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
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
            //kernel.Bind<IKernel>().ToMethod(x=>NinjectInjector.GetInjector).InSingletonScope();
            kernel.Bind<IComSerializer>().To<ComSerializer>().InRequestScope();
            kernel.Bind<IChecksumService>().To<XorChecksumService>().InRequestScope();
            kernel.Bind<ICommandFactory>().To<CommandFactory>().InRequestScope();
            kernel.Bind<IComSettingsProvider>().ToMethod(context => ComSettingsProvider.GetComSettingsProvider).InRequestScope();
            kernel.Bind<IComManager>().To<ComManager>().InTransientScope();
            kernel.Bind<IValidationMessageCollection>().To<ValidationMessageCollection>().InTransientScope();
            kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger(context.Request.Target.Member.ReflectedType)).InRequestScope();
            kernel.Bind<IMeasurementService>().To<MeasurementService>().InRequestScope();
            kernel.Bind<IWeightScaleMessageDto>().To<WeightScaleMessageDto>().InRequestScope();
        }
    }
}
