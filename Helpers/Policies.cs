using Microsoft.AspNetCore.Authorization;

namespace Helpers
{
    public class Policies
    {
        public const string EMAIL_TYPE = "Email";
        public const string HAS_TRADES_TYPE = "HasTrades";
        public const string JOIN_TRADE_TYPE = "JoinTrade";
        
 
        public static void SetPolicies(AuthorizationOptions options)
        {
            options.AddPolicy(Policies.EMAIL_TYPE, policy => policy.RequireClaim(Claims.EMAIL_ADDRESS_TYPE));
            options.AddPolicy(Policies.HAS_TRADES_TYPE, policy => policy.RequireClaim(Claims.HAS_TRADES_TYPE, "true"));
            options.AddPolicy(Policies.JOIN_TRADE_TYPE, policy => policy.Requirements.Add(new Helpers.JoinTradeRequirement()));
        }
    }
}
