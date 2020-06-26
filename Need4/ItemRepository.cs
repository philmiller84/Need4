using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Models;
using Need4Protocol;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Need4
{
    public class ItemRepositoryImpl : ItemRepository.ItemRepositoryBase
    {
        // Server side handler of the SayHello RPC
        public override Task<ActionResponse> AddNewItem(Item request, ServerCallContext context)
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

        public override Task<ItemList> GetMatchingItems(Item itemToMatch, ServerCallContext context)
        {
            using (Need4Context db = new Need4Context())
            {
                try
                {
                    IQueryable<Item> q = from r in db.Items where r.Name == itemToMatch.Name select r;
                    ItemList i = new ItemList();
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
            using (Need4Context db = new Need4Context())
            {
                try
                {
                    bool created = db.Database.EnsureCreated();
                    IQueryable<Item> q = from r in db.Items select r;
                    ItemList i = new ItemList();
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
