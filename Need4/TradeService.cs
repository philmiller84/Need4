﻿using Grpc.Core;
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
            this.GenericWrappedInvoke<TradeActionRequest, ActionDetails>(request, db => from r in db.ActionDetails select r, (x) => t.Actions.Add(x));
            return Task.FromResult(t);
        }
        public override Task<TradeList> GetOpenTrades(Empty e, ServerCallContext context)
        {
            TradeList tl = new TradeList();
            this.GenericWrappedInvoke<Empty, Trade>(e,
                db => from t in db.Trades.Include(l => l.TradeItemList).ThenInclude(j => j.Joins).ThenInclude(d => d.TradeItemDetails).ThenInclude(i => i.Item)
                      //join til in db.TradeItemLists
                      //on t.TradeItemListId equals til.Id
                      select t
                      , (x) => tl.Trades.Add(x));
            //foreach(var t in tl.TradeList_.ToList())
            //{
            //    this.GenericWrappedInvoke<Trade>(t, db => from r in db.TradeItemLists select r, (x) => t.TradeList_.Add(x));
            //}
            return Task.FromResult(tl);
        }
    }
}