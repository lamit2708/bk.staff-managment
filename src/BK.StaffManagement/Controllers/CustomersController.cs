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
using BK.StaffManagement.ViewModels;
using AutoMapper;
using BK.StaffManagement.Enums;
using BK.StaffManagement.Repositories;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BK.StaffManagement.Data;

namespace BK.StaffManagement.Controllers
{
    //[Authorize]
    [Route("[controller]/[action]")]
    public class CustomersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CustomerRepository _customerRepository;
        private readonly ILogger _logger;
        private readonly IDbConnection _connection;
        protected IDbTransaction Transaction;
        public CustomersController(
            UserManager<ApplicationUser> userManager,
            IDbConnection conn,
            CustomerRepository customerRepository,
            ILogger<AccountController> logger,
            IDbTransaction trans)
        {
            _userManager = userManager;
            _customerRepository = customerRepository;
            _connection = conn;
            _logger = logger;
            Transaction = trans;
        }

        public IActionResult Index()
        {
            var customers = _customerRepository.All();
            return View(customers);


        }
        [HttpGet]
        public IActionResult Add()
        {
           
            //EditCustomerViewModel editCustomer = new EditCustomerViewModel();
            var customer = new CustomerViewModel();
            return View(customer);
            
        }
        [HttpPost]
        //public async Task<IActionResult> AddAsync(EditCustomerViewModel model, string returnUrl = null)
        public async Task<IActionResult> AddAsync(CustomerViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName=model.FirstName,
                    LastName=model.LastName,
                    PhoneNumber=model.PhoneNumber,
                    Address=model.Address,
                    Description=model.Description
                };

                try
                {
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var role = StringEnum.GetStringValue(RoleType.Customer);
                        var result1 = await _userManager.AddToRoleAsync(user, role);

                        var customerParam = new DynamicParameters();
                        customerParam.Add(nameof(Customer.Id), user.Id);
                        customerParam.Add(nameof(Customer.CustomerCode), model.CustomerCode);
                        customerParam.Add(nameof(Customer.StaffId), user.Id);
                        customerParam.Add(nameof(Customer.DebitBalance), model.DebitBalance);

                        _customerRepository.Create(customerParam);
                        _customerRepository.Commit();
                        _logger.LogInformation("Customer created a new account with password.");
                        //return RedirectToLocal(returnUrl);
                        return RedirectToAction(nameof(CustomersController.Index), "Customers");
                    }
                    AddErrors(result);
                }
            catch (Exception ex)
            {
                _customerRepository.RollBack();
                _logger.LogError(default(EventId), ex, "Error creating customer");
                throw;
            }
        }
            

            return View(model);
        }

        [HttpGet("{id}")]
        public IActionResult Edit(string id)
        {
            var editEustomer = _customerRepository.Get(id);
            return View(editEustomer);

        }

        [HttpPost("{id}")]
        //public async Task<IActionResult> Edit(string id, EditCustomerViewModel model)
        public async Task<IActionResult> Edit(string id, CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await  _userManager.FindByIdAsync(model.Id);

                // Update it with the values from the view model
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address; //custom property
                user.Description = model.Description;
                if(!string.IsNullOrWhiteSpace(model.Password ))
                    user.PasswordHash= _userManager.PasswordHasher.HashPassword(user, model.Password);
                try
                {
                    //await _userManager.RemovePasswordAsync(user);
                    //var addPasswordResult = await _userManager.AddPasswordAsync(user, model.Password);
                    //if (!addPasswordResult.Succeeded)
                    //{
                    //    AddErrors(addPasswordResult);
                    //    return View(model);
                    //}
                    
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        //var role = StringEnum.GetStringValue(RoleType.Customer);
                        //var result1 = await _userManager.AddToRoleAsync(user, role);

                        var customerParam = new DynamicParameters();
                        customerParam.Add(nameof(Customer.CustomerCode), model.CustomerCode);
                        customerParam.Add(nameof(Customer.StaffId), user.Id);
                        customerParam.Add(nameof(Customer.DebitBalance), model.DebitBalance);

                        _customerRepository.Update(user.Id, customerParam);
                        _customerRepository.Commit();
                        _logger.LogInformation("Customer updated");
                        //return RedirectToLocal(returnUrl);
                        return RedirectToAction(nameof(CustomersController.Edit), "Customers", new {id=user.Id });
                        //return RedirectToAction(nameof(CustomersController.Index), "Customers");
                    }
                    AddErrors(result);
                    return View(model);
                }
                catch (Exception ex)
                {
                    _customerRepository.RollBack();
                    _logger.LogError(default(EventId), ex, "Error updating customer");
                    throw;
                }
            }


            return View(model);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            try
            {
                _customerRepository.Delete(id);
                var user = await _userManager.FindByIdAsync(id);
                var result = await _userManager.DeleteAsync(user);
                _logger.LogInformation("Customer deleted");
                return RedirectToAction(nameof(CustomersController.Index), "Customers");
            }
            catch (Exception ex)
            {
                _customerRepository.RollBack();
                _logger.LogError(default(EventId), ex, "Error deleting customer");
                throw;
            }
           
            
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

        #endregion
    }
}
