using System.Reflection;

namespace log4net.Raygun.DotNetCore
{
    public class AssemblyAdapter : IAssembly
    {
        public Assembly GetEntryAssembly()
        {
            return Assembly.GetEntryAssembly();
        }
    }
}