using Grpc.Core;
using Helloworld;
using System;
using System.Threading.Tasks;

namespace Need4
{
    
    class GreeterImpl : Greeter.GreeterBase
    {
        // Server side handler of the SayHello RPC
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
        }
    }

    public class ServiceHandler //: SharedService.IAsync
    {
        Server server;
        const int Port = 50051;
        public void Startup()
        {
            server = new Server
            {
                Services = { Greeter.BindService(new GreeterImpl()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();
        }

        public void Shutdown() => server.ShutdownAsync().Wait();
    }
}
