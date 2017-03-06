using Mindscape.Raygun4Net;
using System;

namespace log4net.Raygun.DotNetCore
{
    public class RaygunClientFactoryMethod : IRaygunClientFactory
    {
        private readonly Func<RaygunSettings, IRaygunClient> _raygunFactoryMethod;

        public static RaygunClientFactoryMethod From(Func<RaygunSettings, IRaygunClient> raygunFactoryMethod)
        {
            return new RaygunClientFactoryMethod(raygunFactoryMethod);
        }

        public RaygunClientFactoryMethod(Func<RaygunSettings, IRaygunClient> raygunFactoryMethod)
        {
            _raygunFactoryMethod = raygunFactoryMethod;
        }

        public IRaygunClient Create(RaygunSettings raygunSettings)
        {
            return _raygunFactoryMethod(raygunSettings);
        }
    }
}