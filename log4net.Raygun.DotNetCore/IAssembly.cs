using System.Reflection;

namespace log4net.Raygun.DotNetCore
{
    public interface IAssembly
    {
        Assembly GetEntryAssembly();
    }
}