using Grpc.Net.Client;
using Need4Protocol;

namespace XYZ.Data
{
    public class Need4Service 
    {
        private readonly ItemRepository.ItemRepositoryClient itemClient;
        private readonly TradeService.TradeServiceClient tradeClient;
        private readonly SaleService.SaleServiceClient saleClient;
        private readonly UserService.UserServiceClient userClient;

        public ItemRepository.ItemRepositoryClient GetItemClient() { return itemClient; }
        public TradeService.TradeServiceClient GetTradeClient() { return tradeClient; }
        public SaleService.SaleServiceClient GetSaleClient() { return saleClient; }
        public UserService.UserServiceClient GetUserClient() { return userClient; }

        private readonly GrpcChannel channel;
        public Need4Service()
        {
            var serverAddress = "https://localhost:50051";
            channel = GrpcChannel.ForAddress(serverAddress);
            itemClient = new ItemRepository.ItemRepositoryClient(channel);
            tradeClient = new TradeService.TradeServiceClient(channel);
            saleClient = new SaleService.SaleServiceClient(channel);
            userClient = new UserService.UserServiceClient(channel);
        }

        //public static async Task<string> GetAccessToken()
        //{
        //    try
        //    {
        //        // TODO: THIS FUNCTION IS PROBABLY NOT THE RIGHT PLACE OR LAYER. FIX THAT!
        //        var appAuth0Settings = Program.GetAppSettings().GetSection("Need4-API");
        //        var auth0Client = new AuthenticationApiClient(appAuth0Settings["Domain"]);
        //        var tokenRequest = new ClientCredentialsTokenRequest()
        //        {
        //            ClientId = appAuth0Settings["ClientId"],
        //            ClientSecret = appAuth0Settings["ClientSecret"],
        //            Audience = appAuth0Settings["Audience"]
        //        };
        //        //var tokenResponse = await auth0Client.GetTokenAsync(tokenRequest);
        //        var tokenResponse = auth0Client.GetTokenAsync(tokenRequest).Result;
        //        var user = auth0Client.GetUserInfoAsync(tokenResponse.AccessToken).Result;
        //        //TODO: CHECK THESE CLAIMS SOMEWHERE
        //        var claims = new List<Claim>()
        //            {
        //                new Claim(ClaimTypes.NameIdentifier, user.UserId),
        //                new Claim(ClaimTypes.Name, user.UserId)
        //            };

        //        return tokenResponse.AccessToken;
        //    }
        //    catch
        //    {
        //        return String.Empty;
        //    }
        //}
    }
}
