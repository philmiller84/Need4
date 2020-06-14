using Grpc.Core;
using Helloworld;
using System;
using Xunit;
using Need4;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace TestAPIs
{
    public class ServiceFixture : IDisposable
    {
        Greeter.GreeterClient client;
        Channel channel;
        ServiceHandler handler;
        public ServiceFixture()
        {
            handler = new ServiceHandler();
            handler.Startup();

            channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

            client = new Greeter.GreeterClient(channel);
        }
    
        public Greeter.GreeterClient GetClient() => client;
    
        public void Dispose()
        {
            channel.ShutdownAsync().Wait();
            handler.Shutdown();
        }
    }

    public class UnitTest1 : IClassFixture<ServiceFixture>
    {
        ServiceFixture fixture;
        public UnitTest1(ServiceFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Test1()
        {
            String user = "you";
            var reply = fixture.GetClient().SayHello(new HelloRequest { Name = user });
            Assert.Equal("Hello you", reply.Message);
        }
    }
}
