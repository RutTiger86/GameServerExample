using log4net;

namespace AuthServer.Commons
{
    public interface ILogFactory
    {
        ILog CreateLogger<T>();
    }

    public class Log4NetFactory : ILogFactory
    {
        public ILog CreateLogger<T>()
        {
            return LogManager.GetLogger(typeof(T));
        }
    }
}
