using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BK.StaffManagement.Models
{
    public class Customer
    {
        [Required]
        [Key]
        [ForeignKey(nameof(ApplicationUser))]
        public virtual string Id { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public virtual string StaffId { get; set; }

        [MaxLength(100)]
        public virtual string CustomerCode { get; set; }

        public virtual decimal DebitBalance { get; set; } = 0;
    }
}
