using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Application_Extended
{
    public class WithdrawTransaction
    {
        private Account _account; // The account to withdraw from
        private decimal _amount; // The amount to withdraw
        private bool _executed; // Indicates if the transaction was attempted
        private bool _success; // Indicates if the transaction was successful
        private bool _reversed; // Indicates if the transaction was reversed

        public WithdrawTransaction(Account account, decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("Withdraw amount must be positive.");
            }
            _account = account;
            _amount = amount;
            _executed = false;
            _success = false;
            _reversed = false;
        }

        public void Execute()
        {
            if (_executed)
                throw new InvalidOperationException("This transaction has already been attempted.");
            try 
            { 
                _account.Withdraw(_amount);
                _success = true;
            }
            catch (Exception ex)
            {
                _success = false;
                Console.WriteLine("Withdrawal failed: " + ex.Message);
                return;
            }
            
            _executed = true;
            Console.WriteLine($"Withdrawal of {_amount:C2} was successful. The funds have been withdrawn from the account.");
        }

        public void Rollback()
        {
            if (!_executed || _reversed)
                throw new InvalidOperationException("Transaction cannot be reversed in its current state.");
            try
            {
                _account.Deposit(_amount); // Re-deposit the withdrawn amount
            }
            catch (Exception ex)
            {
                Console.WriteLine("Withdrawal reversal failed: " + ex.Message);
                return;
            }
           
            _reversed = true;
            Console.WriteLine($"Transaction reversed: {_amount:C2} has been deposited back into the account.");
        }


        public void Print()
        {
            if (!_executed)
                Console.WriteLine("Transaction has not been executed.");
            else if (_reversed)
                Console.WriteLine($"Withdrawal of {_amount:C2} was reversed. The funds have been deposited back into the account.");
            else if (_success)
                Console.WriteLine($"Withdrawal of {_amount:C2} was successful. The funds have been withdrawn from the account.");
            else
                Console.WriteLine("Withdrawal failed.");
        }

        // Properties to access the private fields
        public bool Executed { get { return _executed; } }
        public bool Success { get { return _success; } }
        public bool Reversed { get { return _reversed; } }
    }
}
