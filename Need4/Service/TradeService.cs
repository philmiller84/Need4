﻿using Grpc.Core;
using Models;
using Need4Protocol;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;

namespace Need4
{
    public class Relation
    {
        public int key1 {get;set;}
        public int key2 {get;set;}
        public Relation(int key1, int key2)
        {
            this.key1 = key1;
            this.key2 = key2;
        }
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
        public Task<int> GetRelation<T>(IQueryable<T> ts, Expression<Func<T,int>> s, Expression<Func<T,bool>> w)
        {
            var q = this.GenericWrappedInvoke<IQueryable<T>, int>(
                db,
                ts,
                (db, ts) => ts.Where(w).Select(s),
                x => {; }
                );

            var relationId = q.FirstOrDefault();

            return Task.FromResult(relationId);
        }

        public Task<int?> GetRelationId(RelationshipType relationshipType, Relation relation)
        {
            int? relationId = null;
            switch(relationshipType.Name)
            {
                case "TradeUser":
                    Expression<Func<TradeUser, int>> s = x => x.Id;
                    Expression<Func<TradeUser, bool>> w = x => x.TradeId == relation.key1 && x.UserId == relation.key2;
                    relationId = GetRelation<TradeUser>(db.TradeUsers, s, w).Result;
                    break;
            }
            return Task.FromResult(relationId);
        }

        public override Task<PermissionSet> CheckPermissions(TradeUserRequest request, ServerCallContext context)
        {
            var ps = GetPermissions(request, context).Result;

            bool authenticated = request.UnauthenticatedUser == null;
            if (authenticated && (ps == null || ps.Permissions.Count == 0))
            {
                var relationshipType = new RelationshipType { Name = Models.Constants.TRADE_USER_TABLE };
                var viewPermissionType = new PermissionType { Name = Models.Constants.VIEW_PERMISSION };
                var relationId = GetRelationId(relationshipType, new Relation(request.TradeId, request.AuthenticatedUserId)).Result;
                if (relationId == null)
                    return Task.FromResult(ps);

                var viewPermission = new Permission { PermissionType = viewPermissionType, RelationshipType = relationshipType, RelationId = relationId.Value};
                var tradeUserPermissionRequest = new TradeUserPermissionRequest { Permission = viewPermission, TradeUserRequest = request };

                AddPermission(tradeUserPermissionRequest, context);
            }
            return Task.FromResult(ps);
        }
        public IQueryable<Permission> GetPermissionSet(Permission request)
        {
            return this.GenericWrappedInvoke<Permission, Permission>(
                db,
                request,
                (db, request) =>
                    from p in db.Permissions
                    .Include(r => r.PermissionType)
                    .Include(r => r.RelationshipType)
                    join rt in db.RelationshipType
                    on p.RelationshipTypeId equals rt.Id
                    where request.RelationshipType.Name == rt.Name
                    && p.RelationId == request.RelationId
                    select p,
                (_) => {; }
                );
        }

        public override Task<PermissionSet> AddPermission(TradeUserPermissionRequest request, ServerCallContext context)
        {
            var ps = new PermissionSet();

            var relationshipType = this.GenericWrappedInvoke<TradeUserPermissionRequest, RelationshipType>(
                    db,
                    request,
                    (db, request) =>
                        from rt in db.RelationshipType
                        where rt.Name == request.Permission.RelationshipType.Name
                        select rt,
                    (_) => {; }).FirstOrDefault();

            var permissionType = this.GenericWrappedInvoke<TradeUserPermissionRequest, PermissionType>(
                    db,
                    request,
                    (db, request) =>
                        from pt in db.PermissionTypes
                        join rt in db.RelationshipType
                        on request.Permission.RelationshipType.Name equals rt.Name
                        where pt.Name == request.Permission.PermissionType.Name
                        select pt,
                    (_) => {; }).FirstOrDefault();

            var tradeUserEntity = this.GenericCreate<TradeUser>(db, new TradeUser(request.TradeUserRequest));

            if(permissionType == null || relationshipType == null)
                return Task.FromResult(ps);

            request.Permission.PermissionType = permissionType;
            request.Permission.PermissionTypeId = permissionType.Id;
            request.Permission.RelationshipType = relationshipType;
            request.Permission.RelationId = tradeUserEntity.Result.Id;
            var s = this.GenericCreate<Permission>(db, request.Permission);

            //if(s.Result == null)
                //return Task.FromResult(ps);

            ps.Permissions.Add(request.Permission);
            return Task.FromResult(ps);
        }
        public override Task<PermissionSet> GetPermissions(TradeUserRequest request, ServerCallContext context)
        {
            var ps = new PermissionSet();
            string relationshipType = "TradeUser";
            int? relationId = GetRelationId(
                new RelationshipType { Name = relationshipType }, 
                new Relation(request.TradeId, request.AuthenticatedUserId)).Result;
                
            Permission permissionRequest = new Permission { 
                RelationshipType = new RelationshipType { Name = relationshipType }, 
                RelationId = relationId.Value
            };

            if(relationId != null)
            {
                ps.Permissions.AddRange(GetPermissionSet(permissionRequest));
            }

            return Task.FromResult(ps);
        }
        public override Task<TradeActionResponse> GetTradeActions(TradeUserRequest request, ServerCallContext context)
        {
            var t = new TradeActionResponse();
            bool tradeExists = CheckTradeExists(request);

            if (!tradeExists)
                return Task.FromResult(t);

            bool authenticated = request.UnauthenticatedUser == null;

            if(!authenticated)
                return Task.FromResult(t);

            int? relationshipId = GetTradeRelationship(request);

            if(relationshipId == null)
                return Task.FromResult(t);

            string relationshipType = Constants.TRADE_USER_TABLE;
            var permissionRequest = new Permission { RelationId = relationshipId.Value, RelationshipType = new RelationshipType { Name = relationshipType } };
            var permissionSet = GetPermissionSet(permissionRequest);

            string actionCategory = Constants.TRADE_ACTION_CATEGORY;
            this.GenericWrappedInvoke<TradeUserRequest, ActionDetails>(
                db,
                request,
                (db, request) =>
                    from ps in permissionSet
                    join r in db.Requirements
                    on new { rti = ps.RelationshipTypeId, pti = ps.PermissionTypeId} equals new { rti = r.RelationshipTypeId, pti = r.PermissionTypeId }
                    join a in db.ActionDetails
                    on r.ActionId equals a.Id
                    where a.Category == actionCategory
                    select a,
                (x) => t.Actions.Add(x));

                //TODO: THIS IS NOT IMPLEMENTED, JUST A STUB HERE
                //this.GenericWrappedInvoke<TradeUserRequest, ActionDetails>(
                //    request,
                //    (db, request) =>
                //        from a in db.ActionDetails
                //        join r in db.Requirements
                //        on a.Id equals r.ActionId
                //        join p in db.Permissions
                //        on r.PermissionTypeId equals p.PermissionTypeId
                //        join tu in db.TradeUsers
                //        on p.RelationId equals tu.Id
                //        join t in db.Trades
                //        on tu.TradeId equals t.Id
                //        join u in db.Users
                //        on tu.UserId equals u.Id
                //        where a.Category == actionCategory
                //        && r.RelationshipTypeId == (from t in db.RelationshipType
                //                                    where t.Name == relationshipType
                //                                    select t.Id).First()
                //        && p.RelationshipTypeId == r.RelationshipTypeId
                //        && t.Id == request.TradeId
                //        && u.Id == request.AuthenticatedUserId
                //        select a,
                //    (x) => t.Actions.Add(x));

            return Task.FromResult(t);
        }

        private int? GetTradeRelationship(TradeUserRequest request)
        {
            return this.GenericWrappedInvoke<TradeUserRequest, TradeUser>(
                db,
                request,
                (db, request) =>
                    from tu in db.TradeUsers
                    where tu.TradeId == request.TradeId && tu.UserId == request.AuthenticatedUserId
                    select tu,
                (_) => {; }).FirstOrDefault()?.Id;
        }

        private bool CheckTradeExists(TradeUserRequest request)
        {
            bool tradeExists = false;
            this.GenericWrappedInvoke<Empty, Trade>(
                db,
                new Empty(),
                (db, _) =>
                    from t in db.Trades
                    where t.Id == request.TradeId
                    select t,
                (x) => { tradeExists = true; });
            return tradeExists;
        }

        public override Task<Trade> GetDetailedTradeView(TradeUserRequest request, ServerCallContext context)
        {
            Trade trade = new Trade();
            this.GenericWrappedInvoke<int, Trade>(
                db, 
                request.TradeId,
                (db, tradeIdAsInt) => from t in db.Trades
                                      .Include(l => l.TradeItemList)
                                      .ThenInclude(y => y.TradeItemDetails)
                                      .ThenInclude(z => z.Item)
                                      where t.Id == request.TradeId
                                      select t,
                (x) => trade = x);

            return Task.FromResult(trade);
        }
        public override Task<TradeList> GetUserTrades(User u, ServerCallContext context)
        {
            TradeList tl = new TradeList();
            this.GenericWrappedInvoke<User, Trade>(
                db, 
                u,
                (db, u) => from t in db.Trades
                           .Include(l => l.TradeItemList)
                           .ThenInclude(y => y.TradeItemDetails)
                           .ThenInclude(z => z.Item)
                           join x in db.TradeUsers
                           on u.Id equals x.UserId
                           where t.Id == x.TradeId
                           select t,
                (x) => tl.Trades.Add(x));

            return Task.FromResult(tl);
        }
        public override Task<TradeList> GetOpenTrades(Empty e, ServerCallContext context)
        {
            TradeList tl = new TradeList();
            this.GenericWrappedInvoke<Empty, Trade>(
                db, 
                e,
                (db, e) => from t in db.Trades
                           .Include(l => l.TradeItemList) 
                           .ThenInclude(y => y.TradeItemDetails)
                           .ThenInclude(z => z.Item)
                           select t,
                (x) => tl.Trades.Add(x));

            return Task.FromResult(tl);
        }
    }
}