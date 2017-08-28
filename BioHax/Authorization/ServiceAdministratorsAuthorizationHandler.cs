using System.Threading.Tasks;
using BioHax.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace BioHax.Authorization
{
    public class ServiceAdministratorsAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Service>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Service resource)
        {
            if (context.User == null)
            {
                return Task.FromResult(0);
            }

            // Administrators can do anything

            if (context.User.IsInRole(Constants.ServiceAdministratorsRole))
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}
