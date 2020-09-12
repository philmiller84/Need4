//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace Helpers
//{
//    public class TestAuthStateProvider : AuthenticationStateProvider
//    {
//        private readonly HttpContextAccessor httpContextAccessor;
//        public TestAuthStateProvider(HttpContextAccessor httpContextAccessor) { this.httpContextAccessor = httpContextAccessor; }
//        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
//        {
//            var anonymous = new ClaimsIdentity();
//            var req = httpContextAccessor.HttpContext.Request;
//            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonymous)));
//        }
//    }
//}