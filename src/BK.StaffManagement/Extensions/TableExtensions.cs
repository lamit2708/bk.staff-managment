using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BK.StaffManagement.Extensions
{

    public static class TableExtensions
    {
        public static string GetTableName(this Type t)
        {
            var tableAttribute = t.GetTypeInfo().GetCustomAttribute<TableAttribute>();
            return tableAttribute != null ? tableAttribute.Name : t.Name;
        }
    }
}
