using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BK.StaffManagement.Models;
using BK.StaffManagement.Models.AccountViewModels;
using BK.StaffManagement.Services;
using Microsoft.Extensions.Configuration;
using System.Data;
using Dapper;
using BK.StaffManagement.Repositories;
using BK.StaffManagement.ViewModels;
using BK.StaffManagement.Enums;

namespace BK.StaffManagement.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class StaffsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly StaffRepository _staffRepository;
        private readonly ILogger _logger;
        private readonly IDbConnection _connection;
        protected IDbTransaction Transaction;
        public StaffsController(
            UserManager<ApplicationUser> userManager,
            IDbConnection conn,
            StaffRepository staffRepository,
            ILogger<AccountController> logger,
            IDbTransaction trans)
        {
            _userManager = userManager;
            _staffRepository = staffRepository;
            _connection = conn;
            _logger = logger;
            Transaction = trans;
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            var staffs = _staffRepository.All();
            return View(staffs);


        }

        [HttpGet]
        public IActionResult Add()
        {

            //EditCustomerViewModel editCustomer = new EditCustomerViewModel();
            var customer = new StaffViewModel();
            return View(customer);

        }
        [HttpPost]
        //public async Task<IActionResult> AddAsync(EditCustomerViewModel model, string returnUrl = null)
        public async Task<IActionResult> Add(StaffViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    Description = model.Description
                };

                try
                {
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var role = StringEnum.GetStringValue(RoleType.Staff);
                        var result1 = await _userManager.AddToRoleAsync(user, role);

                        var staffParam = new DynamicParameters();
                        staffParam.Add(nameof(Staff.Id), user.Id);
                        staffParam.Add(nameof(Staff.StaffCode), model.StaffCode);
                        staffParam.Add(nameof(Staff.Title), model.Title);
                        staffParam.Add(nameof(Staff.Salary), model.Salary);
                        //var createdAt = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();


                        var unixHireDate=ConvertStringToUnixTimestamp(model.HireDateStr);
                        staffParam.Add(nameof(Staff.HireDate), unixHireDate);
                        
                        _staffRepository.Create(staffParam);
                        _staffRepository.Commit();
                        _logger.LogInformation("Staff created a new account with password.");
                        //return RedirectToLocal(returnUrl);
                        return RedirectToAction(nameof(StaffsController.Index), "Staffs");
                    }
                    AddErrors(result);
                }
                catch (Exception ex)
                {
                    _staffRepository.RollBack();
                    _logger.LogError(default(EventId), ex, "Error creating staff");
                    throw;
                }
            }


            return View(model);
        }
        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        private long ConvertStringToUnixTimestamp(string dateStr)
        {
            var hireDate = DateTime.ParseExact(dateStr, "dd-MM-yyyy", null);
            var dateTimeOffset = new DateTimeOffset(hireDate);
            var unixHireDate = dateTimeOffset.ToUnixTimeMilliseconds();
            return unixHireDate;
        }

        #endregion
    }
}
