using Grpc.Core;
using Models;
using Need4Protocol;
using System.Linq;
using System.Threading.Tasks;

namespace Need4
{
    public class ActivityServiceImpl : ActivityService.ActivityServiceBase, IGenericCRUD
    {
        public ActivityServiceImpl(Need4Context db)
        {
            this.db = db;
        }
        private readonly Need4Context db;
 
        public override Task<ActivityResponse> GetAllActivities(User u, ServerCallContext context)
        {
            // STUB IMPLEMENTATION FOR TESTING
            ActivityResponse response = new ActivityResponse();
            //if (u.Name == "Anonymouse")
            //    return Task.FromResult(response);

            //var activities = new List<ActivityDetails> { 
            //    new ActivityDetails { Name = "New Trade", Category = "Trade" }, 
            //    new ActivityDetails { Name = "Public Chat", Category = "Chat" } }.AsQueryable();

            this.GenericWrappedInvoke<User, ActivityDetails>(
                db,
                u,
                (db, u) => from x in db.ActivityDetails
                           select x,
                (x) => {response.Activities.Add(x); });

            return Task.FromResult(response);
        }
    }
}
