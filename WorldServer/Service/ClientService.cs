using log4net;
using Server.Utill;

namespace WorldServer.Services
{
    public interface IClientService
    {

    }

    public class ClientService(ILogFactory logFactory, IServiceProvider serviceProvider) : IClientService
    {
        private readonly IServiceProvider serviceProvider = serviceProvider;
        private readonly ILog log = logFactory.CreateLogger<ClientService>();

    }
}
