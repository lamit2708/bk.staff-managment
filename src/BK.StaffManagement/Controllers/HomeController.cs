using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BK.StaffManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BK.StaffManagement.ViewModels;
using BK.StaffManagement.Repositories;
using BK.StaffManagement.Enums;

namespace BK.StaffManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly CustomerRepository _customerRepository;
        private readonly StaffRepository _staffRepository;
        
        public HomeController(UserManager<ApplicationUser> userManager,
            CustomerRepository customerRepository,
            StaffRepository staffRepository)
        {
            _userManager = userManager;
            _customerRepository = customerRepository;
            _staffRepository = staffRepository;
        }
        
        public IActionResult Index()
        {

            //return RedirectToAction(nameof(HomeController.Index), "Home");
            if (User.IsInRole(StringEnum.GetStringValue(RoleType.Admin)))
            {
                var vmDashboard = new DashboardViewModel();
                vmDashboard.NumOfStaffs = _staffRepository.Count(String.Empty);
                vmDashboard.NumOfCustomers = _customerRepository.Count(String.Empty);
                vmDashboard.TotalDebit = _customerRepository.GetSumDebit();
                return View(vmDashboard);
            }
            else if (User.IsInRole(StringEnum.GetStringValue(RoleType.Staff)))
            {
                return RedirectToAction(nameof(CustomersController.Index), "Customers");
            }
            else
            {
                return RedirectToAction(nameof(CustomersController.Profile), "Customers");
            }
            
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        //[Authorize(Roles = "STAFF")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
