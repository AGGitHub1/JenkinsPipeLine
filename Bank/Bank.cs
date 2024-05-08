using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bank_Application_Extended
{
    public class Bank
    {
        List<Account>  _accounts = new List<Account>();
        private const int MaxAccounts = 5;

        public Bank ()
        {
            _accounts = new List<Account>();
        }

        public void AddAccount(Account account)
        {
            if (_accounts.Count >= MaxAccounts)
            {
                Console.WriteLine("The maximum number of accounts has been reached. Cannot add more accounts.");
                //Console.WriteLine("Please close an account before adding a new one."); - will impement once I have the ability to close accounts
                Console.WriteLine("Please press any key to continue.");
                Console.ReadKey();
                return;
            }
            if (_accounts.Any(a => a.Name.Equals(account.Name, StringComparison.OrdinalIgnoreCase))) // checking for account name against existing accounts
            {
                Console.WriteLine("An account with this name already exists.");
                return;
            }
            

            try
            {
                _accounts.Add(account);
                Console.WriteLine("Account added successfully.");
                
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Account GetAccount(string name)
        {
            foreach (Account account in _accounts)
            {
                if (account.Name == name)
                {
                    return _accounts.FirstOrDefault(a => a.Name == name); //lambda expression checking for account name. returns the first account with the name or default account if not found
                }
            }
            return null; // if account is not found need to catch this everytime
        }

        // Returns a list of all accounts
        public List<Account> GetAllAccounts()
        {
            return new List<Account>(_accounts);
        }

        // Returns a list of all account names
        public List<string> GetAllAccountNames()
        {
            return _accounts.Select(a => a.Name).ToList(); //lambda expression checking for all account names and returning them as a list
        }
        public void ExecuteTransaction(DepositTransaction _deposit)
        {
            _deposit.Execute();
        }

        public void ExecuteTransaction(WithdrawTransaction _withdraw)
        {
            _withdraw.Execute();
        }

        public void ExecuteTransaction(TransferTransaction _transfer)
        {
            _transfer.Execute();
        }



    }
}
