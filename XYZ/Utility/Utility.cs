using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Need4Protocol;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XYZ
{
    public class Utility 
    {
        private readonly IHttpContextAccessor _context;
        private readonly Service.Need4Service _need4Service;

        public Utility(IHttpContextAccessor context, Service.Need4Service need4Service)
        {
            _context = context;
            _need4Service = need4Service;
        }
        public User GetUser()
        {
            string email = Helpers.Claims.GetEmail(_context.HttpContext.User.Claims);
            if (email == null)
                return null;

            return _need4Service.GetUserClient().GetUser(new User { Email = email });
        }

        const string actionToken = @"(\{[a-zA-Z0-9_]*\})+";
        public List<string> GetFormatElements(string actionString)
        {
            var elements = new List<string>();

            foreach (Match match in Regex.Matches(actionString, actionToken))
            {
                elements.Add(match.Value);
            }

            return elements;
        }

        public string ActionRoute(string actionString, IDictionary<string, string> keyValuePairs)
        {
            foreach (Match match in Regex.Matches(actionString, actionToken))
            {
                actionString = Regex.Replace(actionString, match.Value, keyValuePairs[match.Value.ToString().Trim(new char[]{ '{', '}'})]);
            }

            return actionString;
        }
    }
}
