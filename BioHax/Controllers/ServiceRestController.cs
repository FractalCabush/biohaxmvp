using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BioHax.Models;
using BioHax.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using BioHax.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace BioHax.Controllers
{
    [Produces("application/json")]
    [Route("api/Services")]
    public class ServiceRestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        public ServiceRestController(        
            ApplicationDbContext context,
            IAuthorizationService authorizationHandler,
            UserManager<ApplicationUser> userManager,
            ILogger< ServicesController > logger)
        {
                _context = context;
                _userManager = userManager;
                _authorizationService = authorizationHandler;
                _logger = logger;
        }

        [HttpGet]
        public ActionResult GetAll()
        {

            var services = _context.NDEFUri.Include(r => r.Record).ToList();

            if (services != null)
            {
                var isAuthorized = User.IsInRole(Constants.ServiceManagersRole) ||
                                   User.IsInRole(Constants.ServiceAdministratorsRole);

                var currentUserId = _userManager.GetUserId(User);

                // Only approved services are shown UNLESS youre authorized to see them, or you are the owner
                if (!isAuthorized)
                {
                    services = services.Where(s => s.OwnerID == currentUserId).ToList();
                }

                return new JsonResult(RestifyNDEFUriRest(services));
            }
            return null;
        }

        public List<ServiceRest> RestifyNDEFUriRest(List<NDEFUri> modelList)
        {
            List<ServiceRest> list = new List<Models.ServiceRest>();
            foreach (var model in modelList)
            {
                list.Add(new Models.ServiceRest(model));
            }

            return list;
        }

        [HttpGet("{id}", Name = "GetNDEFUri")]
        public async Task<IActionResult> Get(int? id)
        {
            if (id == null)
            {
                return new JsonResult(null);
            }

            var service = await _context.NDEFUri.Include(r => r.Record)
                .SingleOrDefaultAsync(m => m.ServiceId == id);


            if (service == null)
            {
                return new JsonResult(null);
            }

            var isAuthorizedRead = await _authorizationService.AuthorizeAsync(User, service, ServiceOperations.Read);

            if (service.Status != ServiceStatus.Approved && !isAuthorizedRead)
            {
                return new ChallengeResult();
            }

            return new JsonResult(new ServiceDetailsRest(service));
        }
    }
}