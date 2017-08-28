﻿using System.Threading.Tasks;
using BioHax.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace BioHax.Authorization
{
    public class ServiceIsOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Service>
    {
        UserManager<ApplicationUser> _userManager;

        public ServiceIsOwnerAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Service resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.FromResult(0);
            }

            // If were not asking for CRUD permission, return.
            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName )
            {
                return Task.FromResult(0);
            }

            if (resource.OwnerID == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}
