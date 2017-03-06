using Mindscape.Raygun4Net.Messages;

namespace log4net.Raygun.DotNetCore
{
    public interface IRaygunClient
    {
        void Send(RaygunMessage raygunMessage);
    }
}