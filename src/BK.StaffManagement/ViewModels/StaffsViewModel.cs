using BK.StaffManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BK.StaffManagement.ViewModels
{
    public class StaffsViewModel 
    {
        // properties are not capital due to json mapping
        public string Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        //public string StaffCode { get; set; }
    }
}
