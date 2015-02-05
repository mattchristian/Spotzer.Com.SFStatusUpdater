using Salesforce.Common;
using Salesforce.Force;
using SFRestApiUpdater.Commands;
using SFRestApiUpdater.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFRestApiUpdater
{
    class Program
    {
        private static readonly string SecurityToken = ConfigurationManager.AppSettings["SecurityToken"];
        private static readonly string ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
        private static readonly string ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
        private static readonly string Username = ConfigurationManager.AppSettings["Username"];
        private static readonly string Password = ConfigurationManager.AppSettings["Password"] + SecurityToken;
        private static readonly string IsSandboxUser = ConfigurationManager.AppSettings["IsSandboxUser"];
        private static readonly string SandboxUrl = ConfigurationManager.AppSettings["SandboxUrl"];
        private static readonly string ProductionUrl = ConfigurationManager.AppSettings["ProductionUrl"];

        static void Main(string[] args)
        {
            try
            {
                Log.Instance.Info("Args:" + args);
                var commands = new StatusCallCommands(new Arguments(args));
                
                if(!commands.IsValid())
                {
                    Log.Instance.Error("Error parsing incoming args.");
                    Log.Instance.Debug("Args:" + args);
                    return;
                }

                //Get Connected Force Client - as this is called statically it will connect each time.
                var clientTask = GetConnection();
                //wait until connection task has completed
                clientTask.Wait();
                //insert StatusCall object into SF. Aft Insert Trigger in SF will handle running the update.
                var insertTask = InsertStatusCallToSF(clientTask.Result, 
                    commands.Spon, 
                    commands.ApprovalURL, 
                    commands.Status, 
                    commands.OrderId, 
                    commands.TaskId, 
                    commands.FreelancerId, 
                    commands.IsTest, 
                    commands.Role);
                //wait for status call object to complete - using async and wait seems overkill here but when we extend this project to for further rest
                //operations and integrations this will become necessary.
                insertTask.Wait();

                //var task = RunSample();
                //task.Wait();
            }
            catch (Exception e)
            {
                Log.Instance.Error(e.Message,e);
                
                var innerException = e.InnerException;
                while (innerException != null)
                {
                    Log.Instance.Error(innerException.Message, innerException);
                    innerException = innerException.InnerException;
                }
            }
        }

        /// <summary>
        /// Gets a connected (OAuth) Force Client object
        /// </summary>
        /// <returns>ForceClient</returns>
        private static async Task<IForceClient> GetConnection()
        {
            var auth = new AuthenticationClient();
            
            var url = IsSandboxUser.Equals("true", StringComparison.CurrentCultureIgnoreCase)
                ? SandboxUrl
                : ProductionUrl;
            
            Log.Instance.Info(String.Format("Authenticating with Salesforce on URL:{0}",url));
            Log.Instance.Debug(String.Format("ConsumerKey:{0}|ConsumerSecret:{1}|Username:{2}|Password:{3}", ConsumerKey, ConsumerSecret, Username, Password));
            
            await auth.UsernamePasswordAsync(ConsumerKey, ConsumerSecret, Username, Password, url);
            
            Log.Instance.Info("Connected to Salesforce.");
            Log.Instance.Debug(String.Format("AuthClient values|InstanceURL:{0}|AccessToken:{1}|ApiVerison:{2}", auth.InstanceUrl, auth.AccessToken, auth.ApiVersion));
            
            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);
            
            return client;
        }

        /// <summary>
        /// Inserts a StatusCall Object to SF.
        /// </summary>
        /// <param name="client">The authenticated ForceClient Implementation of IForceClient </param>
        /// <param name="spon">The Spotzer Purchase Order Number</param>
        /// <param name="approvalUrl">Web Approval URL</param>
        /// <param name="status">Status</param>
        /// <param name="orderId">Salesforce Order.Id</param>
        /// <param name="taskId">Salesforce Task.Id</param>
        /// <param name="freelancerId">Salesforce Freelancer.Id</param>
        /// <param name="isTest">If true - transaction will be rolled back after completion</param>
        /// <param name="encryptedRole">Salesforce role identifier</param>
        /// <returns>String version of Id of Record to be inserted.</returns>
        private static async Task<String> InsertStatusCallToSF(IForceClient client, string spon, string approvalUrl, int? status, string orderId, string taskId, string freelancerId, bool? isTest, string encryptedRole)
        {
            //Create a custom Record
            Log.Instance.Info(String.Format("Inserting Statuscall_Log__c (Spon={0},approvalUrl={1},status={2},orderId={3},taskId={4},freelancerId={5},isTest={6},encryptedRole={7})",spon, approvalUrl, status, orderId, taskId, freelancerId, isTest, encryptedRole));
            
            //Tidy up - this can be created else where and this function made more generic.
            dynamic statusUpdate = new ExpandoObject();
            statusUpdate.SPON__c = spon;
            statusUpdate.encryptedRole__c = encryptedRole;
            statusUpdate.freelancerId__c = freelancerId;
            statusUpdate.isTest__c = isTest;
            statusUpdate.orderId__c = orderId;
            statusUpdate.Status__c = status;
            statusUpdate.taskId__c = taskId;
            statusUpdate.ApprovalURL__c = approvalUrl;
            statusUpdate.ProcessOnInsert__c = true;
            
            string recordId = await client.CreateAsync("Statuscall_Log__c", statusUpdate);
            
            if (String.IsNullOrEmpty(recordId))
            {
                Log.Instance.Info("Failed to create custom test record.");
                return null;
            }
            
            Log.Instance.Info("StatusCall ID:" + recordId);
            return recordId;
        }

        private static Dictionary<String,String> ParseArgs(String[] args)
        {
            Dictionary<String, String> commands = new Dictionary<String, String>();
            foreach (String arg in args)
            {
                String[] words = arg.Split('=');
                commands[words[0]] = words[1];
            }
            return commands;
        }

        /*
        private static async Task RunSample()
        {
            var auth = new AuthenticationClient();

            // Authenticate with Salesforce
            Console.WriteLine("Authenticating with Salesforce");
            //var url = IsSandboxUser.Equals("true", StringComparison.CurrentCultureIgnoreCase)
            //    ? "https://test.salesforce.com/services/oauth2/token"
            //    : "https://eu5.salesforce.com/services/oauth2/token";
            
            var url = IsSandboxUser.Equals("true", StringComparison.CurrentCultureIgnoreCase)
                ? SandboxUrl
                : ProductionUrl;

            await auth.UsernamePasswordAsync(ConsumerKey, ConsumerSecret, Username, Password, url);
            Console.WriteLine("Connected to Salesforce");

            var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);

            // retrieve all accounts
            Console.WriteLine("Get Accounts");

            const string qry = "SELECT ID, Name FROM Account limit 1000";
            var accts = new List<Account>();
            var results = await client.QueryAsync<Account>(qry);
            var totalSize = results.totalSize;

            Console.WriteLine("Queried " + totalSize + " records.");

            accts.AddRange(results.records);
            var nextRecordsUrl = results.nextRecordsUrl;

            if (!string.IsNullOrEmpty(nextRecordsUrl))
            {
                Console.WriteLine("Found nextRecordsUrl.");

                while (true)
                {
                    var continuationResults = await client.QueryContinuationAsync<Account>(nextRecordsUrl);
                    totalSize = continuationResults.totalSize;
                    Console.WriteLine("Queried an additional " + totalSize + " records.");

                    accts.AddRange(continuationResults.records);
                    if (string.IsNullOrEmpty(continuationResults.nextRecordsUrl)) break;

                    //pass nextRecordsUrl back to client.QueryAsync to request next set of records
                    nextRecordsUrl = continuationResults.nextRecordsUrl;
                }
            }
            Console.WriteLine("Retrieved accounts = " + accts.Count() + ", expected size = " + totalSize);

            //Create a custom Record
            Console.WriteLine("Creating custom test record.");
            dynamic statusUpdate = new ExpandoObject();
            statusUpdate.SPON__c = "999999999";
            statusUpdate.encryptedRole__c = "";
            statusUpdate.freelancerId__c = "";
            statusUpdate.isTest__c = false;
            statusUpdate.orderId__c = "";
            statusUpdate.Status__c = 0;
            statusUpdate.taskId__c = "";
            statusUpdate.ApprovalURL__c = "https://www.someapprovalurl.com";

            string Id = await client.CreateAsync("Statuscall_Log__c", statusUpdate);
            if(String.IsNullOrEmpty(Id))
            {
                Console.WriteLine("Failed to create custom test record.");
                return;
            }

            // Create a sample record
            Console.WriteLine("Creating test record.");
            var account = new Account { Name = "Test Account" };
            account.Id = await client.CreateAsync(Account.SObjectTypeName, account);
            if (account.Id == null)
            {
                Console.WriteLine("Failed to create test record.");
                return;
            }

            Console.WriteLine("Successfully created test record.");

            // Update the sample record
            // Shows that annonymous types can be used as well
            Console.WriteLine("Updating test record.");
            var success = await client.UpdateAsync(Account.SObjectTypeName, account.Id, new { Name = "Test Update" });
            if (!string.IsNullOrEmpty(success.errors.ToString()))
            {
                Console.WriteLine("Failed to update test record!");
                return;
            }

            Console.WriteLine("Successfully updated the record.");

            // Retrieve the sample record
            // How to retrieve a single record if the id is known
            Console.WriteLine("Retrieving the record by ID.");
            account = await client.QueryByIdAsync<Account>(Account.SObjectTypeName, account.Id);
            if (account == null)
            {
                Console.WriteLine("Failed to retrieve the record by ID!");
                return;
            }

            Console.WriteLine("Retrieved the record by ID.");

            // Query for record by name
            Console.WriteLine("Querying the record by name.");
            var accounts = await client.QueryAsync<Account>("SELECT ID, Name FROM Account WHERE Name = '" + account.Name + "'");
            account = accounts.records.FirstOrDefault();
            if (account == null)
            {
                Console.WriteLine("Failed to retrieve account by query!");
                return;
            }

            Console.WriteLine("Retrieved the record by name.");

            // Delete account
            Console.WriteLine("Deleting the record by ID.");
            var deleted = await client.DeleteAsync(Account.SObjectTypeName, account.Id);
            if (!deleted)
            {
                Console.WriteLine("Failed to delete the record by ID!");
                return;
            }
            Console.WriteLine("Deleted the record by ID.");

            // Selecting multiple accounts into a dynamic
            Console.WriteLine("Querying multiple records.");
            var dynamicAccounts = await client.QueryAsync<dynamic>("SELECT ID, Name FROM Account LIMIT 10");
            foreach (dynamic acct in dynamicAccounts.records)
            {
                Console.WriteLine("Account - " + acct.Name);
            }

            // Creating parent - child records using a Dynamic
            Console.WriteLine("Creating a parent record (Account)");
            dynamic a = new ExpandoObject();
            a.Name = "Account from .Net Toolkit";
            a.Id = await client.CreateAsync("Account", a);
            if (a.Id == null)
            {
                Console.WriteLine("Failed to create parent record.");
                return;
            }

            Console.WriteLine("Creating a child record (Contact)");
            dynamic c = new ExpandoObject();
            c.FirstName = "Joe";
            c.LastName = "Blow";
            c.AccountId = a.Id;
            c.Id = await client.CreateAsync("Contact", c);
            if (c.Id == null)
            {
                Console.WriteLine("Failed to create child record.");
                return;
            }

            Console.WriteLine("Deleting parent and child");

            // Delete account (also deletes contact)
            Console.WriteLine("Deleting the Account by Id.");
            deleted = await client.DeleteAsync(Account.SObjectTypeName, a.Id);
            if (!deleted)
            {
                Console.WriteLine("Failed to delete the record by ID!");
                return;
            }
            Console.WriteLine("Deleted the Account and Contact.");

        }

        private class Account
        {
            public const String SObjectTypeName = "Account";

            public String Id { get; set; }
            public String Name { get; set; }
        }*/
    }

}
