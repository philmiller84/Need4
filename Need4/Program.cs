using Grpc.Core;
using Need4Protocol;

namespace Need4
{
    public class ServiceHandler
    {
        private Server server;
        private const int Port = 50051;
        public void Startup()
        {
            server = new Server
            {
                Services = { ItemRepository.BindService(new ItemRepositoryImpl()) },
                Ports = { new ServerPort("127.0.0.1", Port, ServerCredentials.Insecure) }
            };
            server.Start();
        }

        public void Shutdown()
        {
            server.ShutdownAsync().Wait();
        }
    }
}
