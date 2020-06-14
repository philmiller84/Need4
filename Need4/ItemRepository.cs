using Grpc.Core;
using Models;
using Need4Protocol;
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
                try
                {
                    db.Add(request);
                    db.SaveChanges();
                }
                catch 
                {
                    return Task.FromResult(new ActionResponse { Result = (int) HttpStatusCode.Forbidden });
                }
            }

            return Task.FromResult(new ActionResponse { Result = (int) HttpStatusCode.OK });
        }

        public override Task<ItemList> GetAllItems(Item request, ServerCallContext context)
        {
            //ItemList list = new ItemList {List = new Item{Name = "Food" } };
            ItemList list = new ItemList();
            list.List.Add(new Item { Name = "Food" });
            return Task.FromResult(list);
        }
    }
}
