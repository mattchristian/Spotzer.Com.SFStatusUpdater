using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFRestApiUpdater.Extensions
{
    public static class DynamicExtensions
    {
        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            SalesforceAttribute sfAttribute = null;
            String propertyName = String.Empty;

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
            {
                sfAttribute = property.Attributes.OfType<SalesforceAttribute>().FirstOrDefault();
                if (sfAttribute != null && sfAttribute.Ignore == true) continue;
                propertyName = (sfAttribute != null) ? sfAttribute.ApiName : property.Name;
                expando.Add(propertyName, property.GetValue(value));
            }
            return expando as ExpandoObject;
        }

        public static string ToJsonString(this object value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }
    }
}
