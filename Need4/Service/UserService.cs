using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Need4Protocol;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Need4
{
    public class UserServiceImpl : UserService.UserServiceBase, IGenericCRUD
    {
        public override Task<User> GetUser(User u, ServerCallContext context)
        {
            // STUB IMPLEMENTATION FOR TESTING
            this.GenericWrappedInvoke<User, User>(
                 u,
                (db, u) => from x in db.Users
                           where x.Email == u.Email
                           select x,
                (x) => u = x);

            return Task.FromResult(u);
        }

        public override Task<User> CreateUser(User u, ServerCallContext context)
        {
            // STUB IMPLEMENTATION FOR TESTING
            var reply = this.GenericCreate(u);
            u.Created = (reply.Result.Result == (int)HttpStatusCode.OK);
            return Task.FromResult(u);
        }

    }
}
