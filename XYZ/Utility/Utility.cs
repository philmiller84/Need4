using Microsoft.AspNetCore.Http;
using Need4Protocol;

namespace XYZ
{
    public class Utility 
    {
        private readonly IHttpContextAccessor _context;
        private readonly Service.Need4Service _need4Service;

        public Utility(IHttpContextAccessor context, Service.Need4Service need4Service)
        {
            _context = context;
            _need4Service = need4Service;
        }
        public User GetUser()
        {
            string email = Helpers.Claims.GetEmail(_context.HttpContext.User.Claims);
            if (email == null)
                return null;

            return _need4Service.GetUserClient().GetUser(new User { Email = email });
        }
    }
}
