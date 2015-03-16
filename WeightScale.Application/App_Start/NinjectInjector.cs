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
    using Ninject;
    using WeightScale.Application.Contracts;
    using WeightScale.ComunicationProtocol;
    using WeightScale.ComunicationProtocol.Contracts;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;
    using log4net;

    public class NinjectInjector
    {
        private readonly IKernel kernel;
        private static NinjectInjector injector;
        private static readonly object lObj = new object();

        private NinjectInjector()
        {
            this.kernel = new StandardKernel();
            InitializeKernel();
        }

        /// <summary>
        /// Initializes the kernel.
        /// </summary>
        private void InitializeKernel()
        {
            this.kernel.Bind<IComSerializer>().To<ComSerializer>();
            this.kernel.Bind<IChecksumService>().To<XorChecksumService>();
            this.kernel.Bind<ICommandFactory>().To<CommandFactory>();
            this.kernel.Bind<IComManager>().To<ComManager>();
            this.kernel.Bind<IValidationMessageCollection>().To<ValidationMessageCollection>();
            kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger(context.Request.Target.Member.ReflectedType));
        }

        public IKernel Kernel
        {
            get { return this.kernel; }
        }

        public static IKernel GetInjector()
        {
            if (injector == null)
            {
                lock (lObj)
                {
                    injector = new NinjectInjector();
                }
            }

            return injector.Kernel;
        }
    }
}
