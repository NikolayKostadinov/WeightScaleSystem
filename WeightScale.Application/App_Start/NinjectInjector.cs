﻿//---------------------------------------------------------------------------------
// <copyright file="NinjectInjector.cs" company="Business Management Systems">
//     Copyright (c) Business Management Systems. All rights reserved.
// </copyright>
// <author>Nikolay Kostadinov</author>
//--------------------------------------------------------------------------------
namespace WeightScale.Application.AppStart
{
    using System;
    using System.Linq;
    using Ninject;
    using WeightScale.ComunicationProtocol;
    using WeightScale.ComunicationProtocol.Contracts;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;
    using log4net;

    /// <summary>
    /// Dependency injector provider
    /// </summary>
    public class NinjectInjector
    {
        private static readonly object LockObj = new object();
        private static NinjectInjector injector;
        private readonly IKernel kernel;

        /// <summary>
        /// Prevents a default instance of the <see cref="NinjectInjector" /> class from being created.
        /// </summary>
        private NinjectInjector()
        {
            this.kernel = new StandardKernel();
            this.InitializeKernel();
        }

        public IKernel Kernel
        {
            get { return this.kernel; }
        }

        public static IKernel GetInjector()
        {
            if (injector == null)
            {
                lock (LockObj)
                {
                    injector = new NinjectInjector();
                }
            }

            return injector.Kernel;
        }

        /// <summary>
        /// Initializes the kernel.
        /// </summary>
        private void InitializeKernel()
        {
            this.kernel.Bind<IComSerializer>().To<ComSerializer>();
            this.kernel.Bind<IChecksumService>().To<XorChecksumService>();
            this.kernel.Bind<ICommandFactory>().To<CommandFactory>();
            this.kernel.Bind<IComSettingsProvider>().ToMethod(m => ComSettingsProvider.GetComSettingsProvider());
            this.kernel.Bind<IComManager>().To<ComManager>();
            this.kernel.Bind<IValidationMessageCollection>().To<ValidationMessageCollection>();
            this.kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger(context.Request.Target.Member.ReflectedType));
        }
    }
}
