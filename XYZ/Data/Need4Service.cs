using Grpc.Core;
using Need4Protocol;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace XYZ.Data
{
    public class Need4Service 
    {
        private readonly ItemRepository.ItemRepositoryClient itemClient;
        private readonly TradeService.TradeServiceClient tradeClient;
        
        public ItemRepository.ItemRepositoryClient GetItemClient()
        {
            return itemClient;
        }

        public TradeService.TradeServiceClient GetTradeClient()
        {
            return tradeClient;
        }

        private readonly Channel channel;
        private readonly int port = 50051;
        private readonly string localhost_ip = "127.0.0.1";
        public Need4Service()
        {
               // This is the client startup
            string connection = string.Format("{0}:{1}", localhost_ip, port);
            channel = new Channel(connection, ChannelCredentials.Insecure);
            itemClient = new ItemRepository.ItemRepositoryClient(channel);
            tradeClient = new TradeService.TradeServiceClient(channel);
        }
    }
}
