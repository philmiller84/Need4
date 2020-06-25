using Grpc.Core;
using Need4;
using Need4Protocol;
using System;
using System.Threading;

namespace Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ServiceFixture service = new ServiceFixture();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }

    public class ServiceFixture : IDisposable
    {
        private readonly ItemRepository.ItemRepositoryClient itemClient;
        private readonly TradeService.TradeServiceClient tradeClient;
        private readonly Channel channel;
        private readonly ServiceHandler handler;
        private readonly int port = 50051;
        private readonly string localhost_ip = "127.0.0.1";

        public ServiceFixture()
        {
            handler = new ServiceHandler();
            handler.Startup();
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
}
