using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Gateway
{
    public class ValidateBlackListPolicyAttribute : IAuthorizationRequirement { }
    public class ValidateBlackListPolicyHandler : AuthorizationHandler<ValidateBlackListPolicyAttribute>
    {
        public ValidateBlackListPolicyHandler()
        {
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidateBlackListPolicyAttribute requirement)
        {
            var claimsIdentity = context.User.Identity as ClaimsIdentity;
            var claimUserName = claimsIdentity.Claims.FirstOrDefault(c => c.Type == nameof(UserAccount.UserName))?.Value;
            var claimIssueDate = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "iat")?.Value;
            var issueDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(claimIssueDate)).DateTime;
            if (BlackListHelper.CheckInBlackList(claimUserName, issueDate))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
