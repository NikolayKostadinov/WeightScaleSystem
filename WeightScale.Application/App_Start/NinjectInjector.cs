namespace WeightScale.Application.App_Start
{
    using System;
    using System.Linq;
    using Ninject;
    using WeightScale.Application.Contracts;
    using WeightScale.ComunicationProtocol;
    using WeightScale.ComunicationProtocol.Contracts;
    using WeightScale.Domain.Abstract;
    using WeightScale.Domain.Common;

    public class NinjectInjector
    {
        private readonly IKernel kernel;
        private static NinjectInjector injector;
        private static object lObj = new object();

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
