using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BK.StaffManagement.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(256)]
        public string FirstName { get; set; }
        [MaxLength(256)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public virtual string Code { get; set; }

        public virtual string Address { get; set; }

        public virtual string Description { get; set; }

        public virtual long BirthDay { get; set; }

       
    }
    
}
