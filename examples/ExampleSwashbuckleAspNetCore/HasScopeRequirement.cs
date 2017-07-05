using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ExampleSwashbuckleAspNetCore
{
    /// <summary>
    /// Copied from Cerebro project.
    /// </summary>
    /// 
    public class HasScopeRequirement : AuthorizationHandler<HasScopeRequirement>, IAuthorizationRequirement
    {
        private readonly string _issuer;
        private readonly string _scope;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scope">Required scopes delimitied with space</param>
        /// <param name="issuer">Issuer of claim</param>
        public HasScopeRequirement(string scope, string issuer)
        {
            _scope = scope;
            _issuer = issuer;
        }

        /// <summary>
        /// Check if current request is authorized with correct scope and issuer
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == _issuer))
                return Task.FromResult(0);

            // Split the scopes string into an array
            var scopes = context.User.FindFirst(c => c.Type == "scope" && c.Issuer == _issuer).Value.Split(' ');

            // Succeed if the scope array contains the required scope
            if (scopes.Any(s => s == _scope))
                context.Succeed(requirement);

            return Task.FromResult(0);
        }
    }
}
