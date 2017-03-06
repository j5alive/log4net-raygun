using Microsoft.AspNetCore.Http;

namespace log4net.Raygun.DotNetCore
{
    public interface IHttpContext
    {
        HttpContext Instance { get; }
    }
}