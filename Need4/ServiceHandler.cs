using Grpc.Core;
using Need4Protocol;
using System;

namespace Need4
{
    public class ServiceHandler : IDisposable
    {
        private Server server;
        private const int Port = 50051;
        public void Startup()
        {
            server = new Server
            {
                Services = { 
                    ItemRepository.BindService(new ItemRepositoryImpl()) ,
                    TradeService.BindService(new TradeServiceImpl())
                },
                Ports = { new ServerPort("127.0.0.1", Port, ServerCredentials.Insecure) }
            };
            server.Start();
        }

        public void Shutdown()
        {
            server.ShutdownAsync().Wait();
        }
        public void Dispose()
        {
            Shutdown();
        }
    }

}
