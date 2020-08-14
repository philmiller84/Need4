using Microsoft.AspNetCore.Http;
using Need4Protocol;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XYZ.Utility
{
    public class Utility
    {

        static public User GetUser(IHttpContextAccessor context, UserService.UserServiceClient UserClient)
        {
            string email = Helpers.Claims.GetEmail(context.HttpContext.User.Claims);
            if (email == null)
                return null;

            return UserClient.GetUser(new User { Email = email });
        }
    }
}
