namespace Bank_Application_Extended
{
    public class TransferTransaction
    {
        private Account _fromAccount;
        private Account _toAccount;
        private decimal _amount;
        private bool _executed;
        private bool _reversed;

        private WithdrawTransaction _withdrawTransaction;
        private DepositTransaction _depositTransaction;

        public TransferTransaction(Account fromAccount, Account toAccount, decimal amount)
        {
            _fromAccount = fromAccount;
            _toAccount = toAccount;
            _amount = amount;
            _executed = false;
            _reversed = false;

            _withdrawTransaction = new WithdrawTransaction(fromAccount, amount);
            _depositTransaction = new DepositTransaction(toAccount, amount);
        }

        public void Execute()
        {
            if (_executed)
                throw new InvalidOperationException("This transfer transaction has already been attempted.");

            _executed = true; // Mark as attempted before executing to prevent retries in case of failure

            //attempt to try to withdraw before depositing

            try
            {
                _withdrawTransaction.Execute(); // Will throw an exception if the withdrawal fails
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Withdrawal part of the transfer failed: " + ex.Message);
                
            }
            if (!_withdrawTransaction.Success)
            {
                return;
            }
            try
            {
                _depositTransaction.Execute(); // Will throw an exception if the deposit fails
            }
            catch (Exception ex)
            {
                // Roll back the withdrawal if the deposit fails.
                _withdrawTransaction.Rollback();
                throw new InvalidOperationException("Deposit part of the transfer failed: " + ex.Message);
            }
        }

   

        public void Rollback()
            {
            if (!_executed || _reversed)
                throw new InvalidOperationException("The transfer transaction cannot be reversed.");
            bool rollbackSuccess = false;

            try
            {
                _depositTransaction.Rollback();
                rollbackSuccess = true;
            }
            catch (Exception ex)
            {   
                Console.WriteLine($"Failed to rollback the deposit: {ex.Message}");
                return;
            }
          
            if (rollbackSuccess) //only if the deposit rollback was successful
            {
                try
                {
                    _withdrawTransaction.Rollback();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to rollback the withdrawal: {ex.Message}");
                    return;
                }
            }

            _reversed = true;
            Console.WriteLine($"Transfer transaction has been successfully reversed: {_amount:C2} transferred from {_fromAccount.Name} back to {_toAccount.Name}.");
        }

           
        public void Print()
        {
            // First, check if the transaction was executed at all
            if (!_executed)
            {
                Console.WriteLine("The transfer transaction was not executed.");
                return;
            }

            // Then, provide detailed feedback based on the outcome
            if (_executed)
            {
                Console.WriteLine($"Successfully transferred {_amount:C2} from {_fromAccount.Name} to {_toAccount.Name}.");
            }
            else
            {
                Console.WriteLine("Transfer transaction failed.");
                if (_withdrawTransaction.Executed && !_withdrawTransaction.Success)
                {
                    Console.WriteLine("Failure in withdrawal part of the transaction:");
                    _withdrawTransaction.Print(); // This will provide specific failure reason from the withdrawal transaction
                }
                if (_depositTransaction.Executed && !_depositTransaction.Success)
                {
                    Console.WriteLine("Failure in deposit part of the transaction, or it was not attempted due to previous withdrawal failure.");
                    // If withdrawal was successful but deposit failed, print deposit transaction details
                    if (_withdrawTransaction.Success)
                    {
                        _depositTransaction.Print();
                    }
                }
            }
        }

        // Properties to access the private fields
        public bool Executed { get { return _executed; } }
        public bool Success { get { return _withdrawTransaction.Success && _depositTransaction.Success; } }
        public bool Reversed { get { return _reversed; } }
    }
}