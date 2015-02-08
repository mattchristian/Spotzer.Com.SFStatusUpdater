using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFRestApiUpdater.Extensions
{
    public class SalesforceAttribute : Attribute
    {
        public String ApiName { get; set; }
        public Boolean Ignore { get; set; }

        public SalesforceAttribute()
        {
            Ignore = false;
        }
    }
}
