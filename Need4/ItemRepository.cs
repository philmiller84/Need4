using Grpc.Core;
using Need4Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Need4
{
    class ItemRepositoryImpl : ItemRepository.ItemRepositoryBase
    {
        // Server side handler of the SayHello RPC
        public override Task<ActionResponse> AddNewItem(Item request, ServerCallContext context)
        {
            //using (var db = new ItemContext())
            //{
                //db.Add(request);
                //db.SaveChanges();
            //}
            
            return Task.FromResult(new ActionResponse { Result = 1 });
        }

        public override Task<ItemList> GetAllItems(Item request, ServerCallContext context)
        {
            return Task.FromResult(new ItemList {List = new Item{Name = "Food" } });
        }

        //public override Task<ActionResponse> GetAllItems(Item request, ServerCallContext context)
        //{
        //    using (var db = new ItemContext())
        //    {
        //        db.Add(request);
        //        db.SaveChanges();
        //    }
            
        //    return Task.FromResult(new ActionResponse { Result = 1 });
        //}
    }
}
