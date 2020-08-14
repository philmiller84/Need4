using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class Claims
    {
        static public string GetEmail(IEnumerable<Claim> Claims)
        {
            string email_type = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
            string email_value = (from c in Claims where c.Type == email_type select c.Value).FirstOrDefault();
            return email_value;
        }
    }
}