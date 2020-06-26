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

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            var rng = new Random();
            return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToArray());
        }
    }
}
