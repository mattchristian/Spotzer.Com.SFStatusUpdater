using SFRestApiUpdater.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFRestApiUpdater.Commands
{
    public class StatusCallCommands
    {

        public StatusCallCommands(Arguments args)
        {
            Spon = !String.IsNullOrEmpty(args["S"]) ? args["S"] : String.Empty;
            Status = !String.IsNullOrEmpty(args["T"]) ? int.Parse(args["T"]): 0;
            IsTest = !String.IsNullOrEmpty(args["TE"]) ? bool.Parse(args["TE"].Trim().ToLower()) : false;
            ApprovalURL = !String.IsNullOrEmpty(args["U"]) ? args["U"] : String.Empty;
            OrderId = !String.IsNullOrEmpty(args["O"]) ? args["O"] : String.Empty;
            FreelancerId = !String.IsNullOrEmpty(args["FI"]) ? args["FI"] : String.Empty;
            TaskId = !String.IsNullOrEmpty(args["TI"]) ? args["TI"] : String.Empty;
            Role = !String.IsNullOrEmpty(args["R"]) ? args["R"] : String.Empty;
            ProcessOnInsert = true;
        }

        [SalesforceAttribute(ApiName="Spon__c")]
        public String Spon { get; set; }

        [SalesforceAttribute(ApiName = "Status__c")]
        public int Status { get; set; }

        [SalesforceAttribute(ApiName = "IsTest__c")]
        public Boolean IsTest { get; set; }

        [SalesforceAttribute(ApiName = "ApprovalURL__c")]
        public String ApprovalURL { get; set; }

        [SalesforceAttribute(ApiName = "OrderId__c")]
        public String OrderId { get; set; }

        [SalesforceAttribute(ApiName = "FreelancerId__c")]
        public String FreelancerId { get; set; }

        [SalesforceAttribute(ApiName = "TaskId__c")]
        public String TaskId { get; set; }

        [SalesforceAttribute(ApiName = "EncryptedRole__c")]
        public String Role { get; set; }

        [SalesforceAttribute(ApiName = "ProcessOnInsert__c")]
        public Boolean ProcessOnInsert { get; set; }

    
        public Boolean IsValid()
        {
            if (String.IsNullOrEmpty(Spon)) return false;
            if (ApprovalURL == null) return false;
            if (OrderId == null) return false;
            if (FreelancerId == null) return false;
            if (TaskId == null) return false;
            //if (Role == null) return false;
            return true;
        }
    }

}
