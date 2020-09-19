using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Need4Protocol;
using Service;
using System.Linq;
using System.Threading.Tasks;

namespace Helpers
{
    public class Requirements
    {
        public static void RegisterRequirements(IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, JoinTradeAuthorizationHandler>();
        }
    }

    public class JoinTradeAuthorizationHandler : AuthorizationHandler<JoinTradeRequirement, Trade>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, JoinTradeRequirement requirement, Trade resource)
        {
            string email = Helpers.Claims.GetEmail(context.User.Claims);
            if (email == null)
                return null;
            
            var service = new Need4Service();
            var userClient = service.GetUserClient();
            var user = userClient.GetUser(new Need4Protocol.User { Email = email });

            TradeUserRequest tradeUserRequest = user != null ?
                new TradeUserRequest { AuthenticatedUserId = user.Id, TradeId = resource.Id} :
                new TradeUserRequest { UnauthenticatedUser = new Empty(), TradeId = resource.Id };


            var tradeClient = service.GetTradeClient();
            var permissions = tradeClient.GetPermissions(tradeUserRequest);

            //if (context.User.Identity?.Name == resource.Author)
            //{
            //    context.Succeed(requirement);
            //}
            var hasJoinTradePermission = (from p in permissions.Permissions
                                          where p.PermissionType.Name == StaticData.Constants.JOIN_PERMISSION
                                          select p).Any();
            if(hasJoinTradePermission)
            {
                context.Succeed(requirement);
            }
        
            return Task.CompletedTask;
        }
    }
    
    public class JoinTradeRequirement : IAuthorizationRequirement { }
    
}
