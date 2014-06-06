﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;
using Mindscape.Raygun4Net;
using Mindscape.Raygun4Net.Messages;

namespace log4net.Raygun
{
    public class RaygunAppender : AppenderSkeleton
    {
        public static readonly TimeSpan DefaultTimeBetweenRetries = TimeSpan.FromMilliseconds(5000);
        private TimeSpan _timeBetweenRetries;

		private readonly IUserCustomDataBuilder _userCustomDataBuilder;
		private readonly Func<string, IRaygunClient> _raygunClientFactory;
		private readonly TaskScheduler _taskScheduler;

		public RaygunAppender() 
			: this(new UserCustomDataBuilder(), apiKey => new RaygunClientAdapter(new RaygunClient(apiKey)), TaskScheduler.Default)
		{
		}

		internal RaygunAppender(IUserCustomDataBuilder userCustomDataBuilder, Func<string, IRaygunClient> raygunClientFactory, TaskScheduler taskScheduler)
		{
			_userCustomDataBuilder = userCustomDataBuilder;
			_raygunClientFactory = raygunClientFactory;
			_taskScheduler = taskScheduler;
		}

        public virtual string ApiKey { get; set; }
        public virtual int Retries { get; set; }
        public virtual TimeSpan TimeBetweenRetries
        {
            get { return _timeBetweenRetries == TimeSpan.Zero ? DefaultTimeBetweenRetries : _timeBetweenRetries; }
            set { _timeBetweenRetries = value; }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (loggingEvent.Level >= Level.Error)
            {
                Exception exception = null;
                
                var exceptionObject = loggingEvent.ExceptionObject;
                if (exceptionObject != null)
                {
                    exception = exceptionObject.GetBaseException();
                }

				if (exception == null) {
					var messageObject = loggingEvent.MessageObject;
					if (messageObject != null)
					{
						var messageObjectAsException = messageObject as Exception;
						if (messageObjectAsException != null)
						{
							exception = messageObjectAsException;
						}
					}
				}

                if (exception != null)
                {
					SendExceptionToRaygunInBackground(exception, loggingEvent);
                }
            }
        }

		private void SendExceptionToRaygunInBackground(Exception exception, LoggingEvent loggingEvent)
		{
			new TaskFactory(_taskScheduler)
				.StartNew(() => Retry.Action(() => SendExceptionToRaygun(exception, loggingEvent), Retries, TimeBetweenRetries));
		}

        private void SendExceptionToRaygun(Exception exception, LoggingEvent loggingEvent)
        {
			var raygunClient = _raygunClientFactory(ApiKey);

			var userCustomData = _userCustomDataBuilder.Build(loggingEvent);
            var raygunMessage = BuildRaygunExceptionMessage(exception, userCustomData);

            raygunClient.Send(raygunMessage);
        }

        private static RaygunMessage BuildRaygunExceptionMessage(Exception exception, Dictionary<string, string> userCustomData)
        {
			var applicationAssembly = AssemblyResolver.GetApplicationAssembly();

            var raygunMessage = RaygunMessageBuilder.New
                .SetExceptionDetails(exception)
                .SetClientDetails()
                .SetEnvironmentDetails()
                .SetMachineName(Environment.MachineName)
				.SetVersion(applicationAssembly != null ? applicationAssembly.GetName().Version.ToString() : null)
                .SetUserCustomData(userCustomData)
                .Build();

            return raygunMessage;
        }
    }
}