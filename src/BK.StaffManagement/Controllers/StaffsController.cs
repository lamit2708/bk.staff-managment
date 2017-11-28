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
    }
}
