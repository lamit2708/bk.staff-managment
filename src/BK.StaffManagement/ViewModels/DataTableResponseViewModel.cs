using BK.StaffManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BK.StaffManagement.ViewModels
{
    /// <summary>
    /// Class that encapsulates most common parameters sent by DataTables plugin
    /// </summary>
    public class DataTableResponseViewModel
    {
        public int draw { get; set; }
        public long recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public object[] data { get; set; }
        public string error { get; set; }
    }

    
}
