﻿using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Models;
using Need4Protocol;
using System.Linq;
using System.Net;
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
            // STUB IMPLEMENTATION FOR TESTING
            this.GenericWrappedInvoke<Empty, CommunityDetails>(
                db,
                e,
                (db, u) => from x in db.CommunityDetails
                           select x,
                (x) => communityList.Communities.Add(x));

            return Task.FromResult(communityList);
        }

    }
}