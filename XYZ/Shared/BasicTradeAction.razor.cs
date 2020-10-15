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

    public abstract class AuthenticatedTradeAction : BasicTradeAction
    {
        [Inject] protected IAuthorizationService AuthorizationService { get; set; }
        [Inject] protected IHttpContextAccessor HttpContextAccessor { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        protected Task<AuthorizationResult> authorization;
        protected override void InitializeParameters()
        {
            base.InitializeParameters();

            var requirements = new List<IAuthorizationRequirement> { new Helpers.BasicCommunityRequirement() };
            authorization = AuthorizationService.AuthorizeAsync(HttpContextAccessor.HttpContext.User, new Need4Protocol.Trade {Id = tradeId }, requirements);
            actionKeyValuePairs.Add("userId", user.Id.ToString());
        }
        protected override void OnParametersSet()
        {
            InitializeParameters();

            if(authorization.Result.Succeeded)
            {
                DoAction();
            }
        }
    }
    public abstract class BasicTradeAction : ComponentBase
    {
        public abstract void DoAction();
        [Inject] protected Need4Service Need4Service { get; set; }
        [Inject] protected Utility Utility { get; set; }

        [Parameter]
        public int tradeId { get; set; }

        protected Dictionary<string,string> actionKeyValuePairs { get; set; }
        protected Need4Protocol.User user { get; set; }
        protected TradeService.TradeServiceClient tradeClient;
        protected TradeUserInfo tradeUserInfo;
        protected TradeActionResponse tradeActionResponse;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            tradeClient = Need4Service.GetTradeClient();
            user = Utility.GetUser();
        }

        protected virtual void InitializeParameters()
        {
            base.OnParametersSet();
            tradeUserInfo = user != null ?
                new TradeUserInfo { AuthenticatedUserId = user.Id, TradeId = tradeId } :
                new TradeUserInfo { UnauthenticatedUser = new Empty(), TradeId = tradeId };

            actionKeyValuePairs = new Dictionary<string, string> { { "tradeId", tradeId.ToString()} };
        }
        protected override void OnParametersSet()
        {
            //NOTE: base.OnParametersSet() is called in InitializeParameters(). 
            //This function exists so that we can wrap the call to DoAction() in the subclasses.
            //Otherwise, using the base class call to OnParametersSet() will end up executing DoAction() twice.
            InitializeParameters();
            DoAction();
        }
    }
}
