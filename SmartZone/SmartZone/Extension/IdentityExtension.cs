using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SmartZone.Extension
{
    public static class IdentityExtension
    {
        public static string GetAccountID(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("UserID");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetRoleID(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("RoleId");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetUserName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Username");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetSpecClaim (this ClaimsPrincipal claimsPrincipal, string claimytype)
        {
            var claim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == claimytype);
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}
