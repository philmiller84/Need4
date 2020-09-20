using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Models;
using Need4Protocol;
using System.Linq;
using System.Threading.Tasks;

namespace Need4
{
    public class CommunityServiceImpl : CommunityService.CommunityServiceBase, IGenericCRUD
    {
        public CommunityServiceImpl(Need4Context db)
        {
            this.db = db;
        }
        private readonly Need4Context db;
        public override Task<CommunityList> GetAllCommunities(Empty e, ServerCallContext context)
        {
            CommunityList communityList = new CommunityList();
            var q = from x in db.CommunityDetails
                    select x;
            foreach(var community in q)
                communityList.Communities.Add(community);

            return Task.FromResult(communityList);
        }

    }
}
