using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Models;
using Need4Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using StaticData.Constants;
using _States = StaticData.Constants._States;
using _Actions = StaticData.Constants._Actions;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Need4
{
    //public class Relation
    //{
    //    public int key1 {get;set;}
    //    public int key2 {get;set;}
    //    public Relation(int key1, int key2)
    //    {
    //        this.key1 = key1;
    //        this.key2 = key2;
    //    }
    //}

    public class StateAction
    {
        public State state;
        public ActionDetails action;
    }

    public class TradeServiceImpl : TradeService.TradeServiceBase, IGenericCRUD
    {
        public TradeServiceImpl(Need4Context db)
        {
            this.db = db;
        }
        private readonly Need4Context db;

        public override Task<ActionResponse> CreateTrade(Trade request, ServerCallContext context)
        {
            //using Need4Context db = new Need4Context();
            bool created = db.Database.EnsureCreated();
            //Console.WriteLine("Database was created (true), or existing (false): {0}", created);

            int result;
            try
            {
                db.Add(request);
                db.SaveChanges();
                result = (int)HttpStatusCode.OK;
            }
            catch
            {
                result = (int)HttpStatusCode.Forbidden;
            }
                
            return Task.FromResult(new ActionResponse {Result  = result});
        }
        public Task<int?> GetRelation<T>(IQueryable<T> ts, Expression<Func<T,int?>> s, Expression<Func<T,bool>> w)
        {
            try
            {
                return Task.FromResult(ts.Where(w).Select(s).First());
            }
            catch
            {
                return null;
            }
        }
            
        //int? GetRelationId<T>(IQueryable<T> t, Func<T, int?> s, Func<T, bool> w)
        //{
        //    return GetRelation<T>(t, x => s(x), x => w(x)).Result;
        //}

        public Task<int?> GetRelationId(RelationshipType relationshipType, Relation relation)
        {
            int? relationId = null;
            try
            {

                switch(relationshipType.Id)
                {
                    case (int)_RelationshipType.ID.TRADE_USER:
                        {
                            Expression<Func<TradeUser, int?>> s = x => new int?(x.Id);
                            Expression<Func<TradeUser, bool>> w = x => x.TradeId == relation.Key1 && x.UserId == relation.Key2;
                            relationId = GetRelation<TradeUser>(db.TradeUsers, s, w).Result;
                            //relationId = GetRelationId<TradeUser>(
                            //    db.TradeUsers,
                            //    x => new int?(x.Id),
                            //    x => x.TradeId == relation.Key1 && x.UserId == relation.Key2 );
                            break;
                        }
                    case (int)_RelationshipType.ID.COMMUNITY_TRADE:
                        {
                            Expression<Func<CommunityTrade, int?>> s = x => new int?(x.Id);
                            Expression<Func<CommunityTrade, bool>> w = x => x.CommunityId == relation.Key1 && x.TradeId == relation.Key2;
                            relationId = GetRelation<CommunityTrade>(db.CommunityTrades, s, w).Result;
                            break;
                        }
                    case (int)_RelationshipType.ID.COMMUNITY_MEMBER:
                        {
                            Expression<Func<CommunityMember, int?>> s = x => new int?(x.Id);
                            Expression<Func<CommunityMember, bool>> w = x => x.CommunityId == relation.Key1 && x.MemberId == relation.Key2;
                            relationId = GetRelation<CommunityMember>(db.CommunityMembers, s, w).Result;
                            break;
                        } 
                }
                return Task.FromResult(relationId);
            }
            catch
            {
                return null;
            }
        }
        
        private Task<State> GetRelationshipState(Relationship relationship, ServerCallContext context)
        {
            try
            {
                var q =  from x in db.TradeUsers where x.Id == relationship.Id select x.State;
                return Task.FromResult(q.First());
            }
            catch
            {
                return null;
            }
        }
        
        public override Task<State> GetTradeUserState(TradeUserRequest request, ServerCallContext context)
        {
            State st = new State();

            try
            {

                bool authenticated = request.UnauthenticatedUser == null;
                if (authenticated)
                {
                    RelationshipType relationshipType = new RelationshipType { Id = (int)_RelationshipType.ID.TRADE_USER};
                    //var viewPermissionType = new PermissionType { Name = _Permissions.VIEW};
                    Relation relation = new Relation { Key1 = request.TradeId, Key2 = request.AuthenticatedUserId };
                    int? relationId = GetRelationId(relationshipType, relation).Result;
                    if (relationId == null)
                    {
                        return Task.FromResult(st);
                    }
    
                    Relationship relationship = new Relationship { Id = relationId.Value, Relation = relation, RelationshipType = relationshipType };
                    //st.RelationId = relationId.Value;
                    st = GetRelationshipState(relationship, context).Result;
                }
                return Task.FromResult(st);
            }
            catch
            {
                return Task.FromResult(st);
            }
        }

        public override Task<State> AddTradeUserState(TradeUserRequest request, ServerCallContext context)
        {
            State st = new State();
            bool authenticated = request.UnauthenticatedUser == null;
            if (authenticated)
            {
                try
                {
                    var q = from x in db.States where request.State.Description == x.Description select x;
                    var state = q.First();

                    var r = from x in db.TradeUsers where x.TradeId == request.TradeId && x.UserId == request.AuthenticatedUserId select x;

                    if(r.Count() > 0)
                    {
                        r.First().State = state;
                        db.SaveChanges();
                    }
                    else
                    {
                        TradeUser t = new TradeUser { State = state, TradeId = request.TradeId, UserId = request.AuthenticatedUserId };
                        this.GenericCreate(db, t);
                    }
                }
                catch
                {
                    return null;
                }
            }

            return Task.FromResult(st);
        }

        public override Task<PermissionSet> CheckPermissions(TradeUserRequest request, ServerCallContext context)
        {
            PermissionSet ps = GetPermissions(request, context).Result;

            bool authenticated = request.UnauthenticatedUser == null;
            if (authenticated && (ps == null || ps.Permissions.Count == 0))
            {

                Task<State> state = GetTradeUserState(request, context);

                if (state.Result == null)
                {
                    return Task.FromResult(ps);
                }

                if (state.Result.Id == (int)_States._TradeUser.ID.IOI)
                {
                    RelationshipType relationshipType = new RelationshipType { Name = _Tables.TRADE_USER};
                    PermissionType permissionType = new PermissionType { Name = _Permissions.JOIN};
                    PermissionRequest permissionRequest = new PermissionRequest
                    {
                        PermissionType = permissionType,
                        RelationshipType = relationshipType,
                        Key1 = request.TradeId,
                        Key2 = request.AuthenticatedUserId
                    };

                    AddPermission(permissionRequest, context);
                }
            }
            return Task.FromResult(ps);
        }
        public IQueryable<Permission> GetPermissionSet(Permission request)
        {
            var q = from p in db.Permissions
                    .Include(r => r.PermissionType)
                    .Include(r => r.RelationshipType)
                    where p.RelationId == request.RelationId
                    && p.RelationshipTypeId == request.RelationshipType.Id
                    select p;
            return q;
        }

        public override Task<PermissionSet> AddPermission(PermissionRequest request, ServerCallContext context)
        {
            PermissionSet ps = new PermissionSet();

            RelationshipType relationshipType =
                        (from rt in db.RelationshipType
                        where rt.Name == request.RelationshipType.Name
                        select rt).FirstOrDefault();

            PermissionType permissionType = 
                        (from pt in db.PermissionTypes
                        join rt in db.RelationshipType
                        on request.RelationshipType.Name equals rt.Name
                        where pt.Name == request.PermissionType.Name
                        select pt).FirstOrDefault();
            

            if(permissionType == null || relationshipType == null)
            {
                return Task.FromResult(ps);
            }

            int? relationId = GetRelationId(relationshipType, new Relation { Key1 = request.Key1, Key2 = request.Key2 }).Result;

            if(relationId == null)
            {
                return Task.FromResult(ps);
            }

            Permission newPermission = new Permission
            {
                PermissionType = permissionType,
                PermissionTypeId = permissionType.Id,
                RelationshipType = relationshipType,
                RelationId = relationId.Value
            };

            Task<Permission> s = this.GenericCreate<Permission>(db, newPermission);

            ps.Permissions.Add(newPermission);

            return Task.FromResult(ps);
        }
        private int GetCommunityForMember(int memberId)
        {
            return 1;
        }
        private int GetActiveMemberIdForUser(int authenticatedUserId)
        {
            return -1;
        }

        public override Task<PermissionSet> GetPermissions(TradeUserRequest request, ServerCallContext context)
        {
            PermissionSet ps = new PermissionSet();

            if (request.UnauthenticatedUser != null)
                return Task.FromResult(ps);

            try
            {
                RelationshipType tradeUserType = new RelationshipType { Id = (int)_RelationshipType.ID.TRADE_USER };
                int? relationId = GetRelationId(tradeUserType, new Relation { Key1 = request.TradeId, Key2 = request.AuthenticatedUserId }).Result;

                if (relationId != null)
                {
                    Permission permissionRequest = new Permission { RelationshipType = tradeUserType, RelationId = relationId.Value };
                    var permissionSet = GetPermissionSet(permissionRequest);
                    ps.Permissions.AddRange(permissionSet);
                }
                int memberId = GetActiveMemberIdForUser(request.AuthenticatedUserId);
                int communityId = GetCommunityForMember(memberId);
                RelationshipType communityMemberType = new RelationshipType { Id = (int)_RelationshipType.ID.COMMUNITY_MEMBER };
                relationId = GetRelationId(communityMemberType, new Relation { Key1 = communityId, Key2 = memberId }).Result;

                if (relationId != null)
                {
                    Permission permissionRequest = new Permission { RelationshipType = communityMemberType, RelationId = relationId.Value };
                    var permissionSet = GetPermissionSet(permissionRequest);
                    ps.Permissions.AddRange(permissionSet);
                }

                return Task.FromResult(ps);
            }
            catch
            {
                return Task.FromResult(ps);
            }

        }

 
        public List<int> GetAvailableTradeUserActionsForState(int stateId)
        {
            Dictionary<int, List<int>> map = new Dictionary<int, List<int>>
            {
                {
                    (int)_States._TradeUser.ID.IOI, 
                    new List<int>(){
                        (int)_Actions._Trade.ID.JOIN,
                        (int)_Actions._Trade.ID.WATCH
                    }
                },
                {
                    (int)_States._TradeUser.ID.JOINED, 
                    new List<int>(){
                        (int)_Actions._Trade.ID.WITHDRAW,
                        (int)_Actions._Trade.ID.EXCLUDE_USER,
                        (int)_Actions._Trade.ID.ADD_USER,
                        (int)_Actions._Trade.ID.FINALIZE
                    }
                }
            };

            return map[stateId];
        }
        public override Task<TradeActionResponse> GetTradeActions(TradeUserRequest request, ServerCallContext context)
        {
            TradeActionResponse t = new TradeActionResponse();
            bool tradeExists = CheckTradeExists(request);

            if (!tradeExists)
            {
                return Task.FromResult(t);
            }

            bool authenticated = request.UnauthenticatedUser == null;

            if(!authenticated)
            {
                return Task.FromResult(t);
            }

            int? relationshipId = GetTradeRelationship(request);

            if(relationshipId == null)
            {
                return Task.FromResult(t);
            }

            request.State = GetTradeUserState(request, context).Result;

            Task<PermissionSet> permissionSet = GetPermissions(request, context);
            IQueryable<Permission> permissions = permissionSet.Result.Permissions.AsQueryable<Permission>();

            try
            {
                var stateId = request.State.Id;
                var nextStates = GetAvailableTradeUserActionsForState(stateId);

                string actionCategory = _Categories.TRADE_ACTION;

                var permissionedActions =
                    from ps in permissions
                    join r in db.Requirements on new { rti = ps.RelationshipTypeId, pti = ps.PermissionTypeId } equals new { rti = r.RelationshipTypeId, pti = r.PermissionTypeId }
                    select r.ActionId;

                var nextActions =
                    from ps in permissionedActions
                    join a in db.ActionDetails
                    on ps equals a.Id
                    where a.Category == actionCategory && nextStates.Contains(a.Id)
                    select a;

                foreach( var action in nextActions)
                {
                    t.Actions.Add(action);
                }

            }
            catch
            {
                return null;
            }

            return Task.FromResult(t);
        }

        private int? GetTradeRelationship(TradeUserRequest request)
        {
            return (from tu in db.TradeUsers
                    .Include(d => d.State)
                    where tu.TradeId == request.TradeId && tu.UserId == request.AuthenticatedUserId
                    select tu).FirstOrDefault()?.Id;
        }

        private bool CheckTradeExists(TradeUserRequest request)
        {
            var q = from t in db.Trades
                    where t.Id == request.TradeId
                    select t;
            return q.Count() > 0;
        }

        public override Task<Trade> GetDetailedTradeView(TradeUserRequest request, ServerCallContext context)
        {
            Trade trade = new Trade();
            var q = from t in db.Trades
                    .Include(l => l.TradeItemList)
                    .ThenInclude(y => y.TradeItemDetails)
                    .ThenInclude(z => z.Item)
                    where t.Id == request.TradeId
                    select t;

            return Task.FromResult(q.First());
        }
        public override Task<TradeList> GetUserTrades(User u, ServerCallContext context)
        {
            TradeList tl = new TradeList();
            var q = from t in db.Trades
                    .Include(l => l.TradeItemList)
                    .ThenInclude(y => y.TradeItemDetails)
                    .ThenInclude(z => z.Item)
                    join x in db.TradeUsers
                    on u.Id equals x.UserId
                    where t.Id == x.TradeId && x.State.Description  == _States._TradeUser.JOINED
                    select t;

            foreach(var trade in q)
            {
                tl.Trades.Add(trade);
            }

            return Task.FromResult(tl);
        }
        public override Task<TradeList> GetOpenTrades(Empty e, ServerCallContext context)
        {
            TradeList tl = new TradeList();
            var q = from t in db.Trades
                    .Include(l => l.TradeItemList)
                    .ThenInclude(y => y.TradeItemDetails)
                    .ThenInclude(z => z.Item)
                    select t;

            foreach(var trade in q)
            {
                tl.Trades.Add(trade);
            }

            return Task.FromResult(tl);
        }
    }
}