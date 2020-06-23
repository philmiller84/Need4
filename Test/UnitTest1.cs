using Grpc.Core;
using Need4Protocol;
using System;
using Xunit;
using Need4;
using System.Net;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TestAPIs
{

    public class ServiceFixture : IDisposable
    {
        ItemRepository.ItemRepositoryClient itemClient;
        TradeService.TradeServiceClient tradeClient;
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
            itemClient = new ItemRepository.ItemRepositoryClient(channel);
            tradeClient = new TradeService.TradeServiceClient(channel);
        }
    
        public ItemRepository.ItemRepositoryClient GetItemClient() => itemClient;
        public TradeService.TradeServiceClient GetTradeClient() => tradeClient;
    
        public void Dispose()
        {
            channel.ShutdownAsync().Wait();
            handler.Shutdown();
        }
    }

    public class InitData {

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

        ServiceFixture fixture;
        ItemRepository.ItemRepositoryClient client;

        public TestItems(ServiceFixture fixture)
        {
            this.fixture = fixture;
            client = fixture.GetItemClient();
        }


        [Fact]
        public void AddItem()
        {
            var allItems = client.GetAllItems(new Empty());

            client.AddNewItem(InitData.fruit);
            var reply = client.AddNewItem(InitData.orange);
            if(!allItems.List.Contains(InitData.orange))
                Assert.Equal((int)HttpStatusCode.OK, reply.Result);
            else
                Assert.Equal((int)HttpStatusCode.Forbidden, reply.Result);
        }

        [Fact]
        public void GetAllItemsCheckOne()
        {
            var reply = client.GetAllItems(new Empty());
            Assert.Contains<Item>(InitData.orange, reply.List);
        }

        [Fact]
        public void GetMatchingItems()
        {
            var reply = client.GetMatchingItems(InitData.orange);
            Assert.Contains<Item>(InitData.orange, reply.List);
        }
    }
}
