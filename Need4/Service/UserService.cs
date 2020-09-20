using Grpc.Core;
using Models;
using Need4Protocol;
using System.Linq;
using System.Threading.Tasks;

namespace Need4
{
    public class UserServiceImpl : UserService.UserServiceBase, IGenericCRUD
    {
        public UserServiceImpl(Need4Context db)
        {
            this.db = db;
        }
        private readonly Need4Context db;
        public override Task<User> GetUser(User u, ServerCallContext context)
        {
            var q = from x in db.Users
                    where x.Email == u.Email
                    select x;

            return Task.FromResult(q.First());
        }

        public override Task<User> CreateUser(User u, ServerCallContext context)
        {
            Task<User> reply = this.GenericCreate(db, u);
            return Task.FromResult(u);
        }

    }
}
