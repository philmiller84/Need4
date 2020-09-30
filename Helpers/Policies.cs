using Microsoft.AspNetCore.Authorization;

namespace Helpers
{
    public class Policies
    {
        public const string EMAIL_TYPE = "Email";
        public const string HAS_TRADES_TYPE = "HasTrades";
        public const string BASIC_COMMUNITY_TYPE = "BasicCommunity";
        
 
        public static void SetPolicies(AuthorizationOptions options)
        {
            options.AddPolicy(Policies.EMAIL_TYPE, policy => policy.RequireClaim(Claims.EMAIL_ADDRESS_TYPE));
            options.AddPolicy(Policies.HAS_TRADES_TYPE, policy => policy.RequireClaim(Claims.HAS_TRADES_TYPE, "true"));
            options.AddPolicy(Policies.BASIC_COMMUNITY_TYPE, policy => policy.Requirements.Add(new Helpers.BasicCommunityRequirement()));
        }
    }
}
