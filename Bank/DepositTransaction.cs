using System;

namespace Bank_Application_Extended
{
    public class DepositTransaction
    {
        private Account _account; // The account to deposit into
        private decimal _amount; // The amount to deposit
        private bool _executed; // Indicates if the transaction was attempted
        private bool _success; // Indicates if the transaction was successful
        private bool _reversed; // Indicates if the transaction was reversed
        

        public DepositTransaction(Account account, decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("Deposit amount must be positive.");
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
                _account.Deposit(_amount);
                _success = true;
            }
            catch (Exception ex)
            {
                _success = false;
                Console.WriteLine("Deposit failed: " + ex.Message);
                return;
            } 
            _executed = true;
            Console.WriteLine($"Deposit of {_amount:C2} was successful. The funds have been added to the account.");
        }

        public void Rollback()
        {
            if (!_executed || _reversed)
                throw new InvalidOperationException("Transaction cannot be reversed in its current state.");

            try
            {
                _account.Withdraw(_amount); // Withdraw the deposited amount
            }
            catch (Exception ex)
            {
                Console.WriteLine("Deposit reversal failed: " + ex.Message);
                return;
            }

            _reversed = true; // Mark the transaction as reversed only if the reversal was successful
            Console.WriteLine($"Transaction reversed: {_amount:C2} has been withdrawn from the account.");
        }
        public void Print()
        {
            if (!_executed)
            {
                Console.WriteLine("Deposit transaction has not been executed.");
            }
            else if (_reversed)
            {
                Console.WriteLine($"Deposit of {_amount:C2} was reversed. The funds have been withdrawn from the account.");
            }
            else if (_success)
            {
                Console.WriteLine($"Deposit of {_amount:C2} was successful. The funds have been added to the account.");
            }
            else
            {
                Console.WriteLine("Deposit failed. Please check the amount and try again.");
            }
        }

        // Properties to access the private fields
        public bool Executed { get { return _executed; } }
        public bool Success { get { return _success; } }
        public bool Reversed { get { return _reversed; } }
    }
}