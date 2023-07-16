using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Gateway
{
    public class ValidateAgePolicyAttribute : IAuthorizationRequirement { }
    public class ValidateAgePolicy : AuthorizationHandler<ValidateAgePolicyAttribute>
    {
        public ValidateAgePolicy()
        {
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidateAgePolicyAttribute requirement)
        {
            var claimsIdentity = context.User.Identity as ClaimsIdentity;
            var claimBirthdate = claimsIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.DateOfBirth).Value;
            var birthdate = DateTime.Parse(claimBirthdate);
            if (DateTime.Now.Year - birthdate.Year > 18)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
