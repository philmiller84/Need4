using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class Claims
    {
        public const string EMAIL_ADDRESS_TYPE = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        public const string HAS_TRADES_TYPE = "HasTrades";
        public const string JOIN_TRADE_TYPE = "JoinTrade";
        static public string GetEmail(IEnumerable<Claim> Claims)
        {
            string email_value = (from c in Claims where c.Type == EMAIL_ADDRESS_TYPE select c.Value).FirstOrDefault();
            return email_value;
        }
    }
}