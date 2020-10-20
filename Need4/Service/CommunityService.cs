using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Models;
using Need4Protocol;
using System.Linq;
using System.Net;
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


        public override Task<ActionResponse> DoReportMember(MemberReport r, ServerCallContext context)
        {
            var response = new ActionResponse();

            try
            {
                var z = from x in db.Members
                        join u in db.Users on x.UserId equals u.Id
                        where u.Name == r.ReportingMember.User.Name
                        select x;
                var q = from x in db.Members
                        join u in db.Users on x.UserId equals u.Id
                        join y in db.Members on r.ReportedMember.User.Name equals y.User.Name
                        where u.Name == r.ReportingMember.User.Name
                        select new MemberReport { ReportedMember = y, ReportingMember = x, Reason = r.Reason } ;
                db.Add(q.First());
                db.SaveChanges();
                response.Result = (int)HttpStatusCode.OK;
            }
            catch 
            {
                response.Result = (int)HttpStatusCode.Forbidden;
            }

            return Task.FromResult(response);
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
