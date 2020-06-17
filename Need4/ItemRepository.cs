using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;
using Models;
using Need4Protocol;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Need4
{
    class ItemRepositoryImpl : ItemRepository.ItemRepositoryBase
    {
        // Server side handler of the SayHello RPC
        public override Task<ActionResponse> AddNewItem(Item request, ServerCallContext context)
        {
            using (var db = new ItemContext())
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

        public override Task<ItemList> GetMatchingItems(Item itemToMatch, ServerCallContext context)
        {
            using (var db = new ItemContext())
            {
                try
                {
                    var q = from r in db.Items where r.Name == itemToMatch.Name select r;
                    var i = new ItemList();
                    q.ToList().ForEach(x => i.List.Add(x));
                    return Task.FromResult(i);
                }
                catch
                {
                    return null;
                }

            }
        }
        public override Task<ItemList> GetAllItems(Empty empty, ServerCallContext context)
        {
            using (var db = new ItemContext())
            {
                try
                {
                    var q = from r in db.Items select r;
                    var i = new ItemList();
                    q.ToList().ForEach(x => i.List.Add(x));
                    return Task.FromResult(i);
                }
                catch
                {
                    return null;
                }

            }
        }
    }
}
