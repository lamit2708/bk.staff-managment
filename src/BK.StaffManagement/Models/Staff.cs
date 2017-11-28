using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BK.StaffManagement.Models
{
    public class Staff
    {
        [Required]
        [Key]
        [ForeignKey(nameof(ApplicationUser))]
        public virtual string Id { get; set; }

        [MaxLength(256)]
        public virtual string Title { get; set; }

        [MaxLength(100)]
        public virtual string StaffCode { get; set; }

        public virtual decimal Salary { get; set; }

        public virtual long HireDate { get; set; }
    }
}
