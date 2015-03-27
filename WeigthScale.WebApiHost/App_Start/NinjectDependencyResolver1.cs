namespace WeigthScale.WebApiHost.App_Start
{
    using System.Web.Http.Dependencies;
    using Ninject;

    public class NinjectDependencyResolver1 : NinjectDependencyScope, IDependencyResolver 
    {
        private readonly IKernel kernel;

        public NinjectDependencyResolver1(IKernel kernel) : base(kernel)
        {
            this.kernel = kernel;
        }

        public IDependencyScope BeginScope()
        {
            return new NinjectDependencyScope(kernel.BeginBlock());
        }
    }
}