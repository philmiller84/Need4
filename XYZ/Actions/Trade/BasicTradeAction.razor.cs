using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Need4Protocol;
using Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XYZ.Actions
{
    public abstract class BasicTradeAction : ComponentBase
    {

        public abstract void DoAction();

        [Inject] protected IAuthorizationService AuthorizationService { get; set; }
        [Inject] protected IHttpContextAccessor HttpContextAccessor { get; set; }
        [Inject] protected Need4Service Need4Service { get; set; }
        [Inject] protected Utility Utility { get; set; }

        //public BasicTradeAction(
        //    IAuthorizationService AuthorizationService,
        //    IHttpContextAccessor HttpContextAccessor,
        //    Need4Service Need4Service,
        //    Utility Utility)
        //{
        //    this.AuthorizationService = AuthorizationService;
        //    this.HttpContextAccessor = HttpContextAccessor;
        //    this.Need4Service = Need4Service;
        //    this.Utility = Utility;
        //}

        [Parameter]
        public int tradeId { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        private Need4Protocol.User user { get; set; }

        protected Task<AuthorizationResult> authorization;

        protected TradeService.TradeServiceClient tradeClient;

        protected TradeUserRequest tradeUserRequest;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            tradeClient = Need4Service.GetTradeClient();
            user = Utility.GetUser();
        }

        protected override void OnParametersSet()
        {
            tradeUserRequest = user != null ?
                new TradeUserRequest { AuthenticatedUserId = user.Id, TradeId = tradeId } :
                new TradeUserRequest { UnauthenticatedUser = new Empty(), TradeId = tradeId };

            //var permissions = tradeClient.CheckPermissions(tradeUserRequest);

            var requirements = new List<IAuthorizationRequirement> { new Helpers.BasicCommunityRequirement() };
            authorization = AuthorizationService.AuthorizeAsync(HttpContextAccessor.HttpContext.User, new Need4Protocol.Trade {Id = tradeId }, requirements);

            if(authorization.Result.Succeeded)
            {
                DoAction();
            }
            
        }
    }
}
