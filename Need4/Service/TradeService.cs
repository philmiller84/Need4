using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Models;
using Need4Protocol;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

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

        public Task<int?> GetRelationId(RelationshipType relationshipType, Relation relation)
        {
            int? relationId = null;
            switch(relationshipType.Name)
            {
                case "TradeUser":
                    Expression<Func<TradeUser, int?>> s = x => new int?(x.Id);
                    Expression<Func<TradeUser, bool>> w = x => x.TradeId == relation.Key1 && x.UserId == relation.Key2;
                    relationId = GetRelation<TradeUser>(db.TradeUsers, s, w).Result;
                    break;
            }
            return Task.FromResult(relationId);
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

            bool authenticated = request.UnauthenticatedUser == null;
            if (authenticated)
            {
                RelationshipType relationshipType = new RelationshipType { Name = StaticData.Constants.Tables.TRADE_USER};
                //var viewPermissionType = new PermissionType { Name = StaticData.Constants.Permissions.VIEW};
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

                    TradeUser t = new TradeUser { State = state, TradeId = request.TradeId, UserId = request.AuthenticatedUserId };
                    this.GenericCreate(db, t);
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

                if (state.Result.Description == StaticData.Constants.TradeUserStates.IOI)
                {
                    RelationshipType relationshipType = new RelationshipType { Name = StaticData.Constants.Tables.TRADE_USER};
                    PermissionType permissionType = new PermissionType { Name = StaticData.Constants.Permissions.JOIN};
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
                    join rt in db.RelationshipType
                    on p.RelationshipTypeId equals rt.Id
                    where request.RelationshipType.Name == rt.Name
                    && p.RelationId == request.RelationId
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
            //var tradeUserEntity = this.GenericCreate<TradeUser>(db, new TradeUser{TradeId = request.Key1, UserId = request.Key2 });

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

            //if(s.Result == null)
                //return Task.FromResult(ps);

            ps.Permissions.Add(newPermission);
            return Task.FromResult(ps);
        }
        public override Task<PermissionSet> GetPermissions(TradeUserRequest request, ServerCallContext context)
        {
            PermissionSet ps = new PermissionSet();
            string relationshipType = "TradeUser";
            int? relationId = GetRelationId(
                new RelationshipType { Name = relationshipType },
                new Relation { Key1 = request.TradeId, Key2 = request.AuthenticatedUserId }).Result;

            if (relationId == null)
            {
                return Task.FromResult(ps);
            }

            Permission permissionRequest = new Permission { 
                RelationshipType = new RelationshipType { Name = relationshipType }, 
                RelationId = relationId.Value
            };

            ps.Permissions.AddRange(GetPermissionSet(permissionRequest));

            return Task.FromResult(ps);
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

            string relationshipType = StaticData.Constants.Tables.TRADE_USER;
            Permission permissionRequest = new Permission { RelationId = relationshipId.Value, RelationshipType = new RelationshipType { Name = relationshipType } };
            IQueryable<Permission> permissionSet = GetPermissionSet(permissionRequest);

            string actionCategory = StaticData.Constants.Categories.TRADE_ACTION;
            var q = from ps in permissionSet
                    join r in db.Requirements
                    on new { rti = ps.RelationshipTypeId, pti = ps.PermissionTypeId } equals new { rti = r.RelationshipTypeId, pti = r.PermissionTypeId }
                    join a in db.ActionDetails
                    on r.ActionId equals a.Id
                    where a.Category == actionCategory
                    select a;

            foreach( var action in q)
            {
                t.Actions.Add(action);
            }

            return Task.FromResult(t);
        }

        private int? GetTradeRelationship(TradeUserRequest request)
        {
            return (from tu in db.TradeUsers
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
                    where t.Id == x.TradeId
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