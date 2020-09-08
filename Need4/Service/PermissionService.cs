using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Models;
using Need4Protocol;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Need4
{
    public class PermissionServiceImpl : PermissionService.PermissionServiceBase, IGenericCRUD
    {
        public PermissionServiceImpl(Need4Context db)
        {
            this.db = db;
        }
        private readonly Need4Context db;
        public override Task<PermissionSet> GetAllPermissions(User u, ServerCallContext context)
        {
            // STUB IMPLEMENTATION FOR TESTING
            PermissionSet ps = new PermissionSet();
            this.GenericWrappedInvoke<User, Permission>(
                db,
                u,
                (db, u) => from x in db.Permissions
                           .Include(x => x.PermissionType)
                           .Include(x => x.RelationshipType)
                           select x,
                (x) => {ps.Permissions.Add(x); });

            return Task.FromResult(ps);
        }

        public override Task<ActionResponse> IsUserPermissioned(UserPermission u, ServerCallContext context)
        {
            ActionResponse ar = new ActionResponse();
            //// STUB IMPLEMENTATION FOR TESTING
            //var reply = this.GenericCreate(db, u);
            //u.Created = (reply.Result.Result == (int)HttpStatusCode.OK);
            return Task.FromResult(ar);
        }

    }
}
