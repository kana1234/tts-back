using System;
using System.ComponentModel.DataAnnotations;

namespace Charts.Shared.Logic.Extensions
{
    public static class EnumExtension
    {
        public static string GetDisplayName(this Enum obj){
            var type = obj.GetType();
            var fields = type.GetFields();

            foreach (var field in fields){
                var value = field.GetValue(obj);
                if (value.Equals(obj)){
                    var attrs = field.GetCustomAttributes(typeof(DisplayAttribute), true);
                    if (attrs != null && attrs.Length > 0)
                        if (attrs[0] is DisplayAttribute)
                            return ((DisplayAttribute)attrs[0]).GetName();
                }
            }
            return null;
        }
    }

    
}
