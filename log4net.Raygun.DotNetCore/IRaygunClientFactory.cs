using Mindscape.Raygun4Net;

namespace log4net.Raygun.DotNetCore
{
    public interface IRaygunClientFactory
    {
        IRaygunClient Create(RaygunSettings raygunSettings);
    }
}