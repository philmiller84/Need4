using Grpc.Core;
using Grpc.Net.Client;
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
        private readonly SaleService.SaleServiceClient saleClient;
        
        public ItemRepository.ItemRepositoryClient GetItemClient() { return itemClient; }
        public TradeService.TradeServiceClient GetTradeClient() { return tradeClient; }
        public SaleService.SaleServiceClient GetSaleClient() { return saleClient; }

        private readonly GrpcChannel channel;
        public Need4Service()
        {
            var serverAddress = "https://localhost:50051";
            channel = GrpcChannel.ForAddress(serverAddress);
            itemClient = new ItemRepository.ItemRepositoryClient(channel);
            tradeClient = new TradeService.TradeServiceClient(channel);
            saleClient = new SaleService.SaleServiceClient(channel);
        }
    }
}
