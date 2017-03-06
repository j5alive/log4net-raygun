using System.Reflection;

namespace log4net.Raygun.DotNetCore
{
    public class WebAssemblyResolver
    {
        private readonly IHttpContext _httpContext;
        private readonly IAssembly _assemblyLoader;
        private const string AspNamespace = "ASP";

        public WebAssemblyResolver() : this(new AssemblyAdapter())
        {
        }

        internal WebAssemblyResolver(IAssembly assemblyLoader)
        {
            _assemblyLoader = assemblyLoader;
        }

        public Assembly GetApplicationAssembly()
        {
            return _assemblyLoader.GetEntryAssembly();
        }
    }
}