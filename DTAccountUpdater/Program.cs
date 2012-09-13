using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTAccountUpdater.zuora;

namespace DTAccountUpdater
{
    class Program
    {

        static string USERNAME = "";
        static string PASSWORD = "";
        static string ENDPOINT = "https://apisandbox.zuora.com/apps/services/a/40.0";
        private zuora.ZuoraService binding;

        public Program()
        {
            binding = new zuora.ZuoraService();
            binding.Url = ENDPOINT;

        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.login();
            List<Account> acclist = p.queryAccounts();
            List<Account> updateList = new List<Account>(); 
            foreach (Account acc in acclist)
            {
                Account temp = new Account();
                temp.Id = acc.Id;
                temp.MPS_Failed_Payments__c = "0";
                temp.MPS_Next_Retry__c = "";
                //temp.MPS_Activity__c = "Videomeet_DE";
                updateList.Add(temp);
                if (updateList.Count == 50)
                {
                    p.update(updateList.ToArray());
                    updateList.Clear();
                }
            }
            p.update(updateList.ToArray());

        }
        //login
        private bool login()
        {

            try
            {
                //execute the login placing the results  
                //in a LoginResult object 
                zuora.LoginResult loginResult = binding.login(USERNAME, PASSWORD);

                //set the session id header for subsequent calls 
                binding.SessionHeaderValue = new zuora.SessionHeader();
                binding.SessionHeaderValue.session = loginResult.Session;

                //reset the endpoint url to that returned from login 
                // binding.Url = loginResult.ServerUrl;

                Console.WriteLine("Session: " + loginResult.Session);
                Console.WriteLine("ServerUrl: " + loginResult.ServerUrl);

                return true;
            }
            catch (Exception ex)
            {
                //Login failed, report message then return false 
                Console.WriteLine("Login failed with message: " + ex.Message);
                return false;
            }
        }
        private string create(zObject acc)
        {
            SaveResult[] result = binding.create(new zObject[] { acc });
            return result[0].Id;
        }

        private List<Account> queryAccounts()
        {
            QueryResult qResult = binding.query("SELECT id, name, accountnumber FROM account WHERE name = 'Kurt Lu'");
            List<Account> accList = new List<Account>();
            foreach (zObject z in qResult.records)
            {
                Account acc = (Account)z;
                accList.Add(acc);
            }
            
            return accList;
        }

        private string update(zObject[] accs)
        {
            SaveResult[] result = binding.update( accs );
            return result[0].Id;
        }

        private bool delete(String type, string id)
        {

            DeleteResult[] result = binding.delete(type, new string[] { id });
            return result[0].success;

        }
    }
}
