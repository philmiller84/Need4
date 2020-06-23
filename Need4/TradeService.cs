using Grpc.Core;
using Models;
using Need4Protocol;
using System.Net;
using System.Threading.Tasks;

namespace Need4
{
    class TradeServiceImpl : TradeService.TradeServiceBase
    {
        // Server side handler of the SayHello RPC
        public override Task<ActionResponse> CreateTrade(Trade request, ServerCallContext context)
        {
            using (var db = new Need4Context())
            {
                var created = db.Database.EnsureCreated();
                //Console.WriteLine("Database was created (true), or existing (false): {0}", created);
                try
                {
                    db.Add(request);
                    db.SaveChanges();
                    return Task.FromResult(new ActionResponse { Result = (int) HttpStatusCode.OK });
                }
                catch 
                {
                    return Task.FromResult(new ActionResponse { Result = (int) HttpStatusCode.Forbidden });
                }
            }
        }
    }
}