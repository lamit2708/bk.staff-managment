﻿using BK.StaffManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BK.StaffManagement.ViewModels
{
    public class StaffViewModel : ApplicationUser
    {
        //public virtual string StaffId { get; set; }

        //public virtual string CustomerCode { get; set; }

        //public virtual decimal DebitBalance { get; set; } = 0;

        [Display(Name = "Id")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Staff Code")]
        public string StaffCode { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Salary")]
        public decimal Salary { get; set; } = 0;

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Title")]
        [DataType(DataType.Text)]
        public virtual string Title { get; set; }

        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public virtual long HireDate { get; set; }


        [Display(Name = "Hire Date")]
        [DataType(DataType.Text)]
        public virtual string HireDateStr { get; set; }

        //public string HireDateStr => 
    }
}
