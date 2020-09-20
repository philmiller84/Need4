using Grpc.Core;
using Models;
using Need4Protocol;
using System.Linq;
using System.Threading.Tasks;

namespace Need4
{
    public class ItemRepositoryImpl : ItemRepository.ItemRepositoryBase, IGenericCRUD
    {
        public ItemRepositoryImpl(Need4Context db)
        {
            this.db = db;
        }
        private readonly Need4Context db;
        public override Task<ActionResponse> AddNewItem(Item request, ServerCallContext context)
        {
            this.GenericCreate<Item>(db, request);
            return (Task<ActionResponse>)Task.CompletedTask;
            //return this.GenericCreate<Item>(db, request);
        }

        public override Task<ItemList> GetMatchingItems(Item itemToMatch, ServerCallContext context)
        {
            ItemList i = new ItemList();
            var q = from r in db.Items where r.Name == itemToMatch.Name select r;
            foreach(var item in q)
                i.Items.Add(item) ;
            return Task.FromResult(i);
        }
        public override Task<ItemList> GetAllItems(Google.Protobuf.WellKnownTypes.Empty empty, ServerCallContext context)
        {
            ItemList i = new ItemList();
            var q = from r in db.Items select r;
            foreach (var item in q)
                i.Items.Add(item);
            return Task.FromResult(i);
        }
    }
}
