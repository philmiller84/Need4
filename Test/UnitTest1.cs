using Grpc.Core;
using Need4Protocol;
using System;
using Xunit;
using Need4;
using System.Net;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;


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
        // TODO: Set this up to run in parallel to detect problems
        // https://xunit.net/docs/running-tests-in-parallel.html

        ServiceFixture fixture;
        ItemRepository.ItemRepositoryClient client;

        public TestItems(ServiceFixture fixture)
        {
            this.fixture = fixture;
            client = fixture.GetClient();
        }

        Item orange = new Item { Name = "Oranges" };
        Item fruit = new Item { Name = "Fruit" };

        [Fact]
        public void AddItem()
        {
            var allItems = client.GetAllItems(new Empty());

            client.AddNewItem(fruit);
            var reply = client.AddNewItem(orange);
            if(!allItems.List.Contains(orange))
                Assert.Equal((int)HttpStatusCode.OK, reply.Result);
            else
                Assert.Equal((int)HttpStatusCode.Forbidden, reply.Result);
        }

        [Fact]
        public void GetAllItemsCheckOne()
        {
            var reply = client.GetAllItems(new Empty());
            Assert.Contains<Item>(orange, reply.List);
        }

        [Fact]
        public void GetMatchingItems()
        {
            var reply = client.GetMatchingItems(orange);
            Assert.Contains<Item>(orange, reply.List);
        }
    }
}
