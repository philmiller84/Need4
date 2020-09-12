//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Need4Protocol;

//namespace Helpers
//{
//    public class TradeRequirementAttribute : TypeFilterAttribute
//    {
//        public TradeRequirementAttribute(string actionType) : base(typeof(TradeRequirementFilter))
//        {
//            Arguments = new object[] { 0 };
//        }
//    }


//    public class TradeRequirementFilter : IAuthorizationFilter
//    {
//        readonly int trade;

//        public TradeRequirementFilter(int tradeId)
//        {
//            trade = tradeId;
//        }

//        public void OnAuthorization(AuthorizationFilterContext context)
//        {
//            context.Result = new ForbidResult();
//        }
//    }
//}
