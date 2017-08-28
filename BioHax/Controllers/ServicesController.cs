using BioHax.Authorization;
using BioHax.Data;
using BioHax.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BioHax
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ServicesController(
            ApplicationDbContext context,
            IAuthorizationService authorizationHandler,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationHandler;
        }

        // GET: Services
        public async Task<IActionResult> Index()
        {

            var services = from s in _context.Service
                           select s;

            var isAuthorized = User.IsInRole(Constants.ServiceManagersRole) ||
                               User.IsInRole(Constants.ServiceAdministratorsRole);

            var currentUserId = _userManager.GetUserId(User);

            // Only approved services are shown UNLESS youre authorized to see them, or you are the owner
            if(!isAuthorized)
            {
                services = services.Where(s => s.Status == ServiceStatus.Approved || s.OwnerID == currentUserId);
            }

            return View(await services.ToListAsync());
        }

        // GET: Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Service
                .SingleOrDefaultAsync(m => m.ServiceId == id);
            if (service == null)
            {
                return NotFound();
            }

            var isAuthorizedRead = await _authorizationService.AuthorizeAsync(User, service, ServiceOperations.Read);

            if (service.Status != ServiceStatus.Approved && !isAuthorizedRead)
            {
                return new ChallengeResult();
            }

            return View(service);
        }

        // GET: Services/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceEditViewModel editModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editModel);
            }
            var service = ViewModel_to_model(new Service(), editModel);

            service.OwnerID = _userManager.GetUserId(User);

            var isAuthorized = await _authorizationService.AuthorizeAsync(
                User, service, ServiceOperations.Create);

            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            _context.Add(service);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Services/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Service.SingleOrDefaultAsync(m => m.ServiceId == id);

            if (service == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, service, ServiceOperations.Update);

            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            var editModel = Model_to_viewModel(service);

            return View(editModel);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceEditViewModel editModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editModel);
            }

            // Fetch Service from DB to get OwnerID.
            var service = await _context.Service.SingleOrDefaultAsync(m => m.ServiceId == id);
            if (service == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, service, ServiceOperations.Update);
            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            service = ViewModel_to_model(service, editModel);

            if (service.Status == ServiceStatus.Approved)
            {
                // If the service is updated after approval,
                // and the user cannot approve set the status back to submitted
                var canApprove = await _authorizationService.AuthorizeAsync(User, service, ServiceOperations.Approve);
                if (!canApprove) service.Status = ServiceStatus.Submitted;
            }

            try
            {
                _context.Update(service);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(service.ServiceId))
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

        // GET: Services/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Service
                .SingleOrDefaultAsync(m => m.ServiceId == id);
            if (service == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, service, ServiceOperations.Delete);
            if(!isAuthorized)
            {
                return new ChallengeResult();
            }

            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Service.SingleOrDefaultAsync(m => m.ServiceId == id);
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, service, ServiceOperations.Delete);
            if(!isAuthorized)
            {
                return new ChallengeResult();
            }


            _context.Service.Remove(service);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ServiceExists(int id)
        {
            return _context.Service.Any(e => e.ServiceId == id);
        }

        private Service ViewModel_to_model(Service service, ServiceEditViewModel editModel)
        {
            service.Provider = editModel.Provider;
            service.Type = editModel.Type;

            return service;
        }

        private ServiceEditViewModel Model_to_viewModel(Service service)
        {
            var editModel = new ServiceEditViewModel();

            editModel.ServiceId = service.ServiceId;
            editModel.Provider = service.Provider;
            editModel.Type = service.Type;

            return editModel;
        }
    }
}
