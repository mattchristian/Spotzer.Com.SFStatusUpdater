﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFRestApiManager.Commands
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
        }

        public String Spon { get; set; }

        public int Status { get; set; }

        public Boolean IsTest { get; set; }

        public String ApprovalURL { get; set; }

        public String OrderId { get; set; }

        public String FreelancerId { get; set; }

        public String TaskId { get; set; }

        public String Role { get; set; }
    
        public Boolean IsValid()
        {
            if (Spon == null) return false;
            if (Status == null) return false;
            if (IsTest == null) return false;
            if (ApprovalURL == null) return false;
            if (OrderId == null) return false;
            if (FreelancerId == null) return false;
            if (TaskId == null) return false;
            //if (Role == null) return false;
            return true;
        }
    }

}