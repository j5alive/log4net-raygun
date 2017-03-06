using Microsoft.AspNetCore.Http;

namespace log4net.Raygun.DotNetCore
{
    public class HttpContextAdapter : IHttpContext
    {
        private readonly HttpContext _httpContext;

        public HttpContextAdapter(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public HttpContext Instance
        {
            get { return _httpContext; }
        }
    }
}