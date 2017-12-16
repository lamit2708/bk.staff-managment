using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BK.StaffManagement.Models;
using System.Data;
using Dapper;
using BK.StaffManagement.Repositories;
using BK.StaffManagement.ViewModels;
using BK.StaffManagement.Enums;
using Microsoft.AspNetCore.Http;
using System.Reflection;

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
        [HttpGet]
        [Authorize(Roles = UserRole.Admin)]
        public IActionResult Index()
        {
            //var staffs = _staffRepository.All();
            //return View(staffs);
            return View();

        }

        [HttpGet]
        [Authorize(Roles = UserRole.Admin)]
        public IActionResult Add()
        {

            //EditCustomerViewModel editCustomer = new EditCustomerViewModel();
            var staff = new StaffViewModel();
            return View(staff);

        }
        [HttpPost]
        [Authorize(Roles = UserRole.Admin)]
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


                        var unixHireDate = ConvertStringToUnixTimestamp(model.HireDateStr);
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
        [HttpPost]
        [Authorize(Roles = UserRole.Admin)]
        public IActionResult IndexAjax(DataTableParamViewModel param)
        {
            var requestFormData = Request.Form;
            var search = param.search.value;
            var total = _staffRepository.Count(search);
            var listItems = _staffRepository.All(search, param.length, param.start);
            return Json(new
            {
                Data = listItems,
                Draw = requestFormData["draw"],
                RecordsFiltered = total,
                RecordsTotal = total
            });
        }
        [HttpGet("{id}")]
        [Authorize(Roles = UserRole.Admin)]
        public IActionResult Edit(string id)
        {

            //EditCustomerViewModel editCustomer = new EditCustomerViewModel();
            var staff = _staffRepository.Get(id);
            return View(staff);

        }

        [HttpPost("{id}")]
        [Authorize(Roles = UserRole.Admin)]
        //public async Task<IActionResult> AddAsync(EditCustomerViewModel model, string returnUrl = null)
        public async Task<IActionResult> Edit(StaffViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                // Update it with the values from the view model
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address; //custom property
                user.Description = model.Description;
                if (!string.IsNullOrWhiteSpace(model.Password))
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
                try
                {
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        var staff = _staffRepository.Get(user.Id);
                        var staffParam = new DynamicParameters();
                        staffParam.Add(nameof(Staff.Id), user.Id);
                        staffParam.Add(nameof(Staff.StaffCode), staff.StaffCode); //Don't allow your user to edit
                        staffParam.Add(nameof(Staff.Title), model.Title);
                        staffParam.Add(nameof(Staff.Salary), model.Salary);
                        //var createdAt = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();


                        var unixHireDate = ConvertStringToUnixTimestamp(model.HireDateStr);
                        staffParam.Add(nameof(Staff.HireDate), unixHireDate);

                        _staffRepository.Update(user.Id,staffParam);
                        _staffRepository.Commit();
                        _logger.LogInformation("Staff updated.");
                        //return RedirectToLocal(returnUrl);
                        return RedirectToAction(nameof(StaffsController.Index), "Staffs");
                    }
                    AddErrors(result);

                }
                catch (Exception ex)
                {
                    _staffRepository.RollBack();
                    _logger.LogError(default(EventId), ex, "Error updating staff");
                    throw;
                }
            }


            return View(model);
        }

        /// <summary>
        /// Get a list of Items
        /// </summary>
        /// <returns>list of items</returns>
        private List<Models.Item> GetData()
        {
            List<Models.Item> lstItems = new List<Models.Item>()
            {
                new Models.Item() { ItemId =1030,Name ="Bose Mini II", Description ="Wireless and ultra-compact so you can take Bose sound anywhere"  },
                new Models.Item() { ItemId =1031,Name ="Ape Case Envoy Compact - Black (AC520BK)", Description ="Ape Case Envoy Compact Messenger-Style Case for Camera - Black (AC520BK) Removable padded interior in Ape Case signature Hi-Vis yellow protects your equipment"  },
                new Models.Item() { ItemId =1032,Name ="Xbox Wireless Controller - White", Description ="Precision controller compatible with Xbox One, Xbox One S and Windows 10."  },
                new Models.Item() { ItemId =1033,Name ="GoPro HERO5 Black", Description ="Stunning 4K video and 12MP photos in Single, Burst and Time Lapse modes."  },
                new Models.Item() { ItemId =1034,Name ="PNY Elite 240GB USB 3.0 Portable SSD", Description ="PNY Elite 240GB USB 3.0 Portable Solid State Drive (SSD) - (PSD1CS1050-240-FFS)"  },
                new Models.Item() { ItemId =1035,Name ="Quick Charge 2.0 AUKEY 3-Port USB Wall Charger", Description ="Quick Charge 2.0 - Charge compatible devices up to 75% faster than conventional charging"  },
                new Models.Item() { ItemId =1036,Name ="Bose SoundLink Color Bluetooth speaker II - Soft black", Description ="Innovative Bose technology packs bold sound into a small, water-resistant speaker"  },
                new Models.Item() { ItemId = 1010,Name ="RayBan 12300", Description ="Polarized sunglasses"  },
                new Models.Item() { ItemId =1011,Name ="HDMI Cable", Description ="Amzon Basic hdmi cable 3 feet"  },
                new Models.Item() { ItemId =1020,Name ="Anket Portable Charger 500", Description =@"PowerCore Slim 5000
The Slimline Portable Charger
From ANKER, America's Leading USB Charging Brand
• Faster and safer charging with our advanced technology
• 20 million+ happy users and counting"  },
                new Models.Item() { ItemId =1021,Name ="Zippo lighter", Description ="Zippo pocket lighter, black matte"  }

            };

            return lstItems;
        }

        /// <summary>
        /// Get a property info object from Item class filtering by property name.
        /// </summary>
        /// <param name="name">name of the property</param>
        /// <returns>property info object</returns>
        private PropertyInfo getProperty(string name)
        {
            var properties = typeof(Models.Item).GetProperties();
            PropertyInfo prop = null;
            foreach (var item in properties)
            {
                if (item.Name.ToLower().Equals(name.ToLower()))
                {
                    prop = item;
                    break;
                }
            }
            return prop;
        }

        /// <summary>
        /// Process a list of items according to Form data parameters
        /// </summary>
        /// <param name="lstElements">list of elements</param>
        /// <param name="requestFormData">collection of form data sent from client side</param>
        /// <returns>list of items processed</returns>
        private List<Models.Item> ProcessCollection(List<Models.Item> lstElements, IFormCollection requestFormData)
        {

            var skip = Convert.ToInt32(requestFormData["start"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            Microsoft.Extensions.Primitives.StringValues tempOrder = new[] { "" };

            if (requestFormData.TryGetValue("order[0][column]", out tempOrder))
            {
                var columnIndex = requestFormData["order[0][column]"].ToString();
                var sortDirection = requestFormData["order[0][dir]"].ToString();
                tempOrder = new[] { "" };
                if (requestFormData.TryGetValue($"columns[{columnIndex}][data]", out tempOrder))
                {
                    var columName = requestFormData[$"columns[{columnIndex}][data]"].ToString();

                    if (pageSize > 0)
                    {
                        var prop = getProperty(columName);
                        if (sortDirection == "asc")
                        {
                            return lstElements.OrderBy(prop.GetValue).Skip(skip).Take(pageSize).ToList();
                        }
                        else
                            return lstElements.OrderByDescending(prop.GetValue).Skip(skip).Take(pageSize).ToList();
                    }
                    else
                        return lstElements;
                }
            }
            return null;
        }
        // POST api/values
        [HttpPost]
        public IActionResult AjaxHandlerTest(DataTableParamViewModel model)
        {
            try
            {
                // var TableIn = DataTablesIn.ParseJSONString(myDataTableParameter);
                
                var requestFormData = Request.Form;
                List<Models.Item> lstItems = GetData();

                var listItems = ProcessCollection(lstItems, requestFormData);

                // Custom response to bind information in client side
                dynamic response = new 
                {
                    Data = listItems,
                    Draw = requestFormData["draw"],
                    RecordsFiltered = lstItems.Count,
                    RecordsTotal = lstItems.Count
                };
                return Json(response);
            }
            catch (Exception e)
            {
                return ThrowJSONError(e);
            }
        }
        private JsonResult ThrowJSONError(Exception e)
        {
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            //Log your exception
            return Json(new { error = e.Message }); //Message
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
