using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Models;
using Need4Protocol;
using System.Linq;
using System.Runtime.CompilerServices;
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
            var q = from x in db.Communities
                    orderby x.Id ascending
                    select x;
            foreach(var community in q)
                communityList.Communities.Add(community);

            return Task.FromResult(communityList);
        }


        public override Task<MemberList> GetCommunityMembers(Community c, ServerCallContext context)
        {
            MemberList memberList = new MemberList();
            var q = from x in db.Members
                    .Include(x => x.User)
                    join z in db.Communities
                    on c.Name equals z.Name 
                    join y in db.CommunityMembers
                    on new { memberId = x.Id, communityId = z.Id } equals new { memberId = y.MemberId, communityId = y.CommunityId }
                    join u in db.Users
                    on x.UserId equals u.Id
                    select x;

            foreach( var member in q )
            {
                memberList.Members.Add(new MemberDetails { Name = member.User.Name, Status = 0 });
            }

            return Task.FromResult(memberList);
        }
    }
}
