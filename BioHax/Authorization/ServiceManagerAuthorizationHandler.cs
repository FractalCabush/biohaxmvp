using System.Threading.Tasks;
using BioHax.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace BioHax.Authorization
{
    public class ServiceManagerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Service>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Service resource)
        {
            if (context.User == null || resource == null) {
                return Task.FromResult(0);
            }

            // If not asking for approval/reject, return.
            if(requirement.Name != Constants.ApproveOperationName &&
               requirement.Name != Constants.RejectOperationName )
            {
                return Task.FromResult(0);
            }

            // Managers can approve or reject new types of services added
            if (context.User.IsInRole(Constants.ServiceManagersRole))
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}
