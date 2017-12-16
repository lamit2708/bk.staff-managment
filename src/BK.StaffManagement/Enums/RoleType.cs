using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BK.StaffManagement.Enums
{

    public enum RoleType
    {
        [StringEnum("ADMIN")]
        Admin = 1,
        [StringEnum("STAFF")]
        Staff = 2,
        [StringEnum("CUSTOMER")]
        Customer =3
    }
    public static class UserRole
    {
        public const string Admin = "ADMIN";
        public const string Staff = "STAFF";
        public const string Customer = "CUSTOMER";

    }
    public sealed class StringEnum : System.Attribute
    {

        private string _value;

        public StringEnum(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
        public static string GetStringValue(Enum value)
        {
            Type type = value.GetType();
            FieldInfo fi = type.GetRuntimeField(value.ToString());
            return (fi.GetCustomAttributes(typeof(StringEnum), false).FirstOrDefault() as StringEnum).Value;
        }
       
        public static T GetFromAttribute<T>(string attributeName)
        {
            Type type = typeof(T);
            return (T)Enum.Parse(typeof(T), type.GetRuntimeFields().FirstOrDefault(
              x => (x.CustomAttributes.Count() > 0 && (x.CustomAttributes.FirstOrDefault().ConstructorArguments.FirstOrDefault().Value as string).Equals(attributeName))).Name);
        }
    }

}
