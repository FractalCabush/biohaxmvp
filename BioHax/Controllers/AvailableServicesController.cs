using BioHax.Authorization;
using BioHax.Data;
using BioHax.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioHax.Controllers
{
    public class AvailableServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AvailableServicesController(
            ApplicationDbContext context,
            IAuthorizationService authorizationHandler,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationHandler;
        }

        // GET: AvailableServices (/Available)
        [AllowAnonymous]
        public async Task<IActionResult> Available()
        {
            var availableServices = from s in _context.AvailableService
                                    select s;
            return View(await availableServices.ToListAsync());
        }

        public async Task<IActionResult> Index()
        {
            var availableServices = from s in _context.AvailableService
                                    select s;
            var isAuthorizedRead = await _authorizationService.AuthorizeAsync(User, availableServices.First(), ServiceOperations.Read);
            if (!isAuthorizedRead)
            {
                return new ChallengeResult();
            }

            return View(await availableServices.ToListAsync());
        }

        // GET: AvailableServices/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var availableService = await _context.AvailableService
                .SingleOrDefaultAsync(m => m.ServiceId == id);

            if (availableService == null)
            {
                return NotFound();
            }

            return View(availableService);
        }

        // GET: AvailableServices/Create
        public IActionResult Create()
        {
            var isAuthorized = (User.IsInRole(Constants.ServiceManagersRole)
                            || User.IsInRole(Constants.ServiceAdministratorsRole));

            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            return View();
        }

        // POST: AvailableServices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AvailableServiceEditViewModel editModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editModel);
            }
            var availableService = ViewModel_to_model(new AvailableService(), editModel);
            availableService.OwnerID = _userManager.GetUserId(User);

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, availableService, ServiceOperations.Create);

            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            _context.Add(availableService);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: AvailableServices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var availableService = await _context.AvailableService.SingleOrDefaultAsync(m => m.ServiceId == id);
            if (availableService == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, availableService, ServiceOperations.Update);

            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            var editModel = Model_to_viewModel(availableService);
            return View(editModel);
        }

        // POST: AvailableServices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AvailableServiceEditViewModel editModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editModel);
            }

            // fetch available services and get ownerID
            var availableService = await _context.AvailableService.SingleOrDefaultAsync(m => m.ServiceId == id);
            if(availableService == null)
            {
                return NotFound();
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, availableService, ServiceOperations.Update);

            if(!isAuthorized)
            {
                return new ChallengeResult();
            }

            availableService = ViewModel_to_model(availableService, editModel);
            
            if(availableService.Status == ServiceStatus.Approved)
            {
                var canApprove = await _authorizationService.AuthorizeAsync(User, availableService, ServiceOperations.Approve);
                if (!canApprove) availableService.Status = ServiceStatus.Submitted;
            }

    
            try
            {
                _context.Update(availableService);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AvailableServiceExists(availableService.ServiceId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }

        // GET: AvailableServices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var availableService = await _context.AvailableService
                .SingleOrDefaultAsync(m => m.ServiceId == id);
            if (availableService == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, availableService, ServiceOperations.Delete);
            
            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            return View(availableService);
        }

        // POST: AvailableServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var availableService = await _context.AvailableService.SingleOrDefaultAsync(m => m.ServiceId == id);
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, availableService, ServiceOperations.Delete)
                                        && (User.IsInRole(Constants.ServiceManagersRole)
                                        || User.IsInRole(Constants.ServiceAdministratorsRole));

            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            _context.AvailableService.Remove(availableService);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(int id, ServiceStatus status)
        {
            var availableService = await _context.AvailableService.SingleOrDefaultAsync(s => s.ServiceId == id);
            var serviceOperation = (status == ServiceStatus.Approved) ? ServiceOperations.Approve
                                                                      : ServiceOperations.Reject;
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, availableService, serviceOperation);
            if(!isAuthorized)
            {
                return new ChallengeResult();
            }

            availableService.Status = status;
            _context.AvailableService.Update(availableService);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AvailableServiceExists(int id)
        {
            return _context.AvailableService.Any(e => e.ServiceId == id);
        }

        private AvailableService ViewModel_to_model(AvailableService availableService, AvailableServiceEditViewModel editModel)
        {
            availableService.Provider = editModel.Provider;
            availableService.Type = editModel.Type;
            availableService.Description = editModel.Description;
            return availableService;
        }

        private AvailableServiceEditViewModel Model_to_viewModel(AvailableService availableService)
        {
            var editModel = new AvailableServiceEditViewModel();
            editModel.ServiceId = availableService.ServiceId;
            editModel.Provider = availableService.Provider;
            editModel.Type = availableService.Type;
            editModel.Description = availableService.Description;

            return editModel;
        }
    }
}
