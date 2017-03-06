namespace log4net.Raygun.DotNetCore
{
    public class DoNothingMessageFilter : IMessageFilter
    {
        public string Filter(string message)
        {
            return message;
        }
    }
}