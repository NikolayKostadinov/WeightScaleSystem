namespace WeightScale.Application.Infrastructure
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Web.Http.Dispatcher;

    public class CustomAssemblyResolver : IAssembliesResolver
    {
        private readonly string path;

        public CustomAssemblyResolver(string path)
        {
            this.path = path;
        }

        public ICollection<Assembly> GetAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>();
            assemblies.Add(Assembly.LoadFrom(this.path));
            return assemblies;
        }
    }
}
