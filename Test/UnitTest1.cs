using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Need4;
using Need4Protocol;
using System;
using System.Net;
using Xunit;

namespace TestAPIs
{

    public class ServiceFixture : IDisposable
    {
        private readonly ServiceHandler handler;

        private readonly ItemRepository.ItemRepositoryClient itemClient;
        private readonly TradeService.TradeServiceClient tradeClient;
        private readonly Channel channel;
        private readonly int port = 50051;
        private readonly string localhost_ip = "127.0.0.1";

        public ServiceFixture()
        {
            // This is the service startup
            handler = new ServiceHandler();
            handler.Startup();
            
            // This is the client startup
            string connection = string.Format("{0}:{1}", localhost_ip, port);
            channel = new Channel(connection, ChannelCredentials.Insecure);
            itemClient = new ItemRepository.ItemRepositoryClient(channel);
            tradeClient = new TradeService.TradeServiceClient(channel);
        }

        public ItemRepository.ItemRepositoryClient GetItemClient()
        {
            return itemClient;
        }

        public TradeService.TradeServiceClient GetTradeClient()
        {
            return tradeClient;
        }

        public void Dispose()
        {
            channel.ShutdownAsync().Wait();
            handler.Shutdown();
        }
    }

    public class InitData
    {

        public static Item orange = new Item { Name = "Oranges" };
        public static Item fruit = new Item { Name = "Fruit" };
    }
    //public class TestTrades : IClassFixture<ServiceFixture>
    //{
    //    // TODO: Set this up to run in parallel to detect problems
    //    // https://xunit.net/docs/running-tests-in-parallel.html

    //    ServiceFixture fixture;
    //    ItemRepository.ItemRepositoryClient itemClient;
    //    TradeService.TradeServiceClient tradeClient;

    //   public TestTrades(ServiceFixture fixture)
    //    {
    //        this.fixture = fixture;
    //        itemClient = fixture.GetItemClient();
    //        tradeClient = fixture.GetTradeClient();
    //    }

    //    [Fact]
    //    public void CreateTrade()
    //    {
    //        itemClient.AddNewItem(InitData.fruit);
    //        itemClient.AddNewItem(InitData.orange);
    //        var allItems = itemClient.GetAllItems(new Empty());

    //        var newTrade = new Trade();

    //        foreach(var item in allItems.List)
    //        {
    //            newTrade.TradeItemsList.Add(new TradeItemDetails { Item = item , NeedOffset = 1});
    //        }

    //        var reply = tradeClient.CreateTrade(newTrade);
    //    }
    //}

    public class TestItems : IClassFixture<ServiceFixture>
    {
        // TODO: Set this up to run in parallel to detect problems
        // https://xunit.net/docs/running-tests-in-parallel.html

        //private readonly ServiceFixture fixture;
        private readonly ItemRepository.ItemRepositoryClient client;

        public TestItems(ServiceFixture fixture)
        {
            //this.fixture = fixture;
            client = fixture.GetItemClient();
        }


        [Fact]
        public void AddItem()
        {
            ItemList allItems = client.GetAllItems(new Empty());

            client.AddNewItem(InitData.fruit);
            ActionResponse reply = client.AddNewItem(InitData.orange);

            Assert.Equal(!allItems.Items.Contains(InitData.orange) ? (int)HttpStatusCode.OK : (int)HttpStatusCode.Forbidden, reply.Result);
        }

        [Fact]
        public void GetAllItemsCheckOne()
        {
            ItemList reply = client.GetAllItems(new Empty());
            Assert.Contains<Item>(InitData.orange, reply.Items);
        }

        [Fact]
        public void GetMatchingItems()
        {
            ItemList reply = client.GetMatchingItems(InitData.orange);
            Assert.Contains<Item>(InitData.orange, reply.Items);
        }
    }
}
