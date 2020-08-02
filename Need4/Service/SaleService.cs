using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Need4Protocol;
using System.Threading.Tasks;

namespace Need4
{
    public class SaleServiceImpl : SaleService.SaleServiceBase, IGenericCRUD
    {
        public override Task<SaleList> GetSales(Empty e, ServerCallContext context)
        {
            var s = new SaleList();
            var SaleItemList = new SaleItemList { Id = -1, SaleId = -1 };
            SaleItemList.SaleItemDetails.Add(new SaleItemDetails { Item = new Item { Name = "Fruit", EstimatedCost = 2.5 } });
            s.Sales.Add(new Sale { Id = -1, SaleItemList = SaleItemList });
            return Task.FromResult(s);
        }
    }
}
