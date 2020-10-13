using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Need4Protocol;
using Service;
using System.Linq;
using System.Threading.Tasks;
using StaticData.Constants;

namespace Helpers
{
    public class Requirements
    {
        public static void RegisterRequirements(IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, BasicCommunityAuthorizationHandler>();
        }
    }
    public class BasicCommunityAuthorizationHandler : AuthorizationHandler<BasicCommunityRequirement, Trade>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BasicCommunityRequirement requirement, Trade resource)
        {
            string email = Helpers.Claims.GetEmail(context.User.Claims);
            if (email == null)
            {
                return null;
            }

            Need4Service service = new Need4Service();
            UserService.UserServiceClient userClient = service.GetUserClient();
            Need4Protocol.User user = userClient.GetUser(new Need4Protocol.User { Email = email });

            TradeUserInfo tradeUserInfo = user != null ?
                new TradeUserInfo { AuthenticatedUserId = user.Id, TradeId = resource.Id} :
                new TradeUserInfo { UnauthenticatedUser = new Empty(), TradeId = resource.Id };

            TradeService.TradeServiceClient tradeClient = service.GetTradeClient();
            PermissionSet permissions = tradeClient.GetPermissions(tradeUserInfo);

            bool hasJoinTradePermission = (from p in permissions.Permissions
                                          where p.PermissionType.Name == _Permissions.BASIC
                                          select p).Any();
            if(hasJoinTradePermission)
            {
                context.Succeed(requirement);
            }
        
            return Task.CompletedTask;
        }
    }
    
    public class BasicCommunityRequirement : IAuthorizationRequirement { }
    
}
