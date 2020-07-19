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
    public class SaleServiceImpl : SaleService.SaleServiceBase, IGenericCRUD
    {
        public override Task<SaleList> GetSales(Empty e, ServerCallContext context)
        {
            var s = new SaleList();
            return Task.FromResult(s);
        }

        //    public override Task<ActionResponse> CreateTrade(Trade request, ServerCallContext context)
        //    {
        //        using (Need4Context db = new Need4Context())
        //        { }
        //    }
    }
}
