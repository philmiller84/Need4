using Grpc.Core;
using Models;
using Need4Protocol;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Need4
{
    public class ItemRepositoryImpl : ItemRepository.ItemRepositoryBase, IGenericCRUD
    {

        public override Task<ActionResponse> AddNewItem(Item request, ServerCallContext context)
        {
            return this.GenericCreate(request, typeof(Item));
        }

        public override Task<ItemList> GetMatchingItems(Item itemToMatch, ServerCallContext context)
        {
            using (Need4Context db = new Need4Context())
            {
                try
                {
                    IQueryable<Item> q = from r in db.Items where r.Name == itemToMatch.Name select r;
                    ItemList i = new ItemList();
                    q.ToList().ForEach(x => i.Items.Add(x));
                    return Task.FromResult(i);
                }
                catch
                {
                    return null;
                }

            }
        }
        public override Task<ItemList> GetAllItems(Google.Protobuf.WellKnownTypes.Empty empty, ServerCallContext context)
        {
            using (Need4Context db = new Need4Context())
            {
                try
                {
                    bool created = db.Database.EnsureCreated();
                    IQueryable<Item> q = from r in db.Items select r;
                    ItemList i = new ItemList();
                    q.ToList().ForEach(x => i.Items.Add(x));
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
