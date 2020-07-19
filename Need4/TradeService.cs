using Grpc.Core;
using Models;
using Need4Protocol;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;

namespace Need4
{
    public class TradeServiceImpl : TradeService.TradeServiceBase, IGenericCRUD
    {
        public override Task<ActionResponse> CreateTrade(Trade request, ServerCallContext context)
        {
            using (Need4Context db = new Need4Context())
            {
                bool created = db.Database.EnsureCreated();
                //Console.WriteLine("Database was created (true), or existing (false): {0}", created);
                try
                {
                    db.Add(request);
                    db.SaveChanges();
                    return Task.FromResult(new ActionResponse { Result = (int)HttpStatusCode.OK });
                }
                catch
                {
                    return Task.FromResult(new ActionResponse { Result = (int)HttpStatusCode.Forbidden });
                }
            }
        }
        public override Task<TradeActionResponse> GetTradeActions(TradeActionRequest request, ServerCallContext context)
        {
            //NOTE: THIS IS NOT IMPLEMENTED, JUST A STUB HERE
            TradeActionResponse t = new TradeActionResponse();
            bool tradeExists = false;
            this.GenericWrappedInvoke<Empty, Trade>(new Empty(), (db, _) => from t in db.Trades where t.Id == request.TradeId select t, (x) => { tradeExists = true; });

            if(tradeExists)
                this.GenericWrappedInvoke<TradeActionRequest, ActionDetails>(request, (db,request) => from r in db.ActionDetails select r, (x) => t.Actions.Add(x));
            return Task.FromResult(t);
        }
        public override Task<Trade> GetDetailedTradeView(Int32Value tradeId, ServerCallContext context)
        {
            Trade trade = new Trade();

            int tradeIdAsInt = tradeId.Value;
            this.GenericWrappedInvoke<int, Trade>(
                 tradeIdAsInt,
                (db, tradeIdAsInt) => from t in db.Trades
                                      .Include(l => l.TradeItemList)
                                      .ThenInclude(y => y.TradeItemDetails)
                                      .ThenInclude(z => z.Item)
                                      where t.Id == tradeIdAsInt
                                      select t,
                (x) => trade = x);

            return Task.FromResult(trade);
        }
        public override Task<TradeList> GetOpenTrades(Empty e, ServerCallContext context)
        {
            TradeList tl = new TradeList();
            this.GenericWrappedInvoke<Empty, Trade>(
                 e,
                (db, e) => from t in db.Trades
                           .Include(l => l.TradeItemList) 
                           .ThenInclude(y => y.TradeItemDetails)
                           .ThenInclude(z => z.Item)
                           select t,
                (x) => tl.Trades.Add(x));

            return Task.FromResult(tl);
        }
    }
}