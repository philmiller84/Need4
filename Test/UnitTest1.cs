using Grpc.Core;
using Need4Protocol;
using System;
using Xunit;
using Need4;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace TestAPIs
{
    public class ServiceFixture : IDisposable
    {
        ItemRepository.ItemRepositoryClient client;
        Channel channel;
        ServiceHandler handler;

        int port = 50051;
        string localhost_ip = "127.0.0.1";

        public ServiceFixture()
        {
            handler = new ServiceHandler();
            handler.Startup();
            string connection = string.Format("{0}:{1}", localhost_ip, port);
            channel = new Channel(connection, ChannelCredentials.Insecure);
            client = new ItemRepository.ItemRepositoryClient(channel);
        }
    
        public ItemRepository.ItemRepositoryClient GetClient() => client;
    
        public void Dispose()
        {
            channel.ShutdownAsync().Wait();
            handler.Shutdown();
        }
    }

    public class TestItems : IClassFixture<ServiceFixture>
    {
        ServiceFixture fixture;

        public TestItems(ServiceFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Test1()
        {
            Item a = new Item { Name = "Food" };

            var reply = fixture.GetClient().AddNewItem(a);
            Assert.Equal(0, reply.Result);
        }
    }
}
