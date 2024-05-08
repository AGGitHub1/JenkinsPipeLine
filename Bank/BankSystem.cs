using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace Bank_Application_Extended
{

    public enum MenuOption
    {
        AddNewAccount = 1,
        Withdraw,
        Deposit,
        Print,
        SwitchAccount,
        Transfer,
        Quit
    }

    public class BankSystem
    {
        private static Bank bank = new Bank();
        private static Account currentAccount; // Track the current active account

        private static readonly decimal MaxAmountDeposit = 10000m; //to prevent OutofMemoryException
        static void Main(string[] args)
        {
            // Initialise accounts
            bank.AddAccount(new Account("Savings", 0));
            bank.AddAccount(new Account("Cheque", 500));
            bank.AddAccount(new Account("Credit", 2000));


            currentAccount = bank.GetAccount("Savings");

            bool userExited = false;

            while (!userExited)
            {

                MenuOption userChoice = ReadUserOption();
                switch (userChoice)
                {
                    case MenuOption.AddNewAccount:
                        DoAddAccount();
                        break;
                    case MenuOption.Withdraw:
                        DoWithdraw(bank);
                        break;
                    case MenuOption.Deposit:
                        DoDeposit(bank);
                        break;
                    case MenuOption.Print:
                        DoPrint(bank);
                        break;
                    case MenuOption.SwitchAccount:
                        SwitchAccount();
                        break;
                    case MenuOption.Transfer:
                        DoTransfer();
                        break;
                    case MenuOption.Quit:
                        Console.WriteLine("Goodbye!");
                        userExited = true;
                        break;
                }
            }
        }

        private static MenuOption ReadUserOption()
        {
            MenuOption choice;
            do
            {
                Console.Clear();
                Console.WriteLine($"Your Current Account is: {currentAccount.Name}");
                Console.WriteLine("===================================");
                Console.WriteLine("       Bank Options Menu");
                Console.WriteLine("===================================");
                Console.WriteLine("1. Add a new Account");
                Console.WriteLine("2. Withdraw");
                Console.WriteLine("3. Deposit");
                Console.WriteLine("4. Print Account Balance");
                Console.WriteLine("5. Switch Account");
                Console.WriteLine("6. Transfer");
                Console.WriteLine("7. Quit");
                Console.WriteLine("-----------------------------------");
                Console.Write("Select an option: ");

                if (int.TryParse(Console.ReadLine(), out int input) && input >= 1 && input <= 6)
                {
                    choice = (MenuOption)input;
                    return choice;
                }
                else
                {
                    Console.WriteLine("Invalid option, please try again.");
                }

            } while (true);
        }

        private static void DoAddAccount()
        {
            Console.WriteLine("Enter the name for the new account:");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Account name cannot be empty, please try again");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Enter the starting balance for the new account:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal startingBalance) || startingBalance < 0)
            {
                Console.WriteLine("Invalid or negative starting balance, please try again.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            try
            {
                Account newAccount = new Account(name, startingBalance);
                bank.AddAccount(newAccount);

            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }


            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void SwitchAccount()
        {
            List<string> accountNames = bank.GetAllAccountNames(); // Using the new method from Bank class

            if (accountNames.Count == 0)
            {
                Console.WriteLine("No accounts available to switch.");
                return;
            }

            Console.WriteLine("Select an account:");
            for (int i = 0; i < accountNames.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {accountNames[i]}");
            }

            Console.Write("Enter the number of the account you wish to switch to: ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int accountNumber) && accountNumber >= 1 && accountNumber <= accountNames.Count)
            {
                currentAccount = bank.GetAccount(accountNames[accountNumber - 1]);
                Console.WriteLine($"Switched to account: {currentAccount.Name}");
            }
            else
            {
                Console.WriteLine("Invalid selection, please enter a number from the list.");
            }
        }
        private static Account FindAccount(Bank bank)
        {
            // Display all account names
            List<string> accountNames = bank.GetAllAccountNames();



            if (accountNames.Count == 0)
            {
                Console.WriteLine("No accounts available.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return null;  // Exit if no accounts exist
            }
            // Ask the user to enter an account name
            Console.WriteLine("Enter the account name you wish to operate on");
            Console.WriteLine("===================================");
            Console.WriteLine("From These Available Accounts: ");
            Console.WriteLine("--->");

            foreach (var name in accountNames)
            {
                Console.WriteLine(name);
            }
            string accountName = Console.ReadLine();


            Account account = bank.GetAccount(accountName);


            if (account == null)
            {
                Console.WriteLine("No account found with the name: " + accountName);
            }
            else
            {
                Console.WriteLine("Account found: " + account.Name);
            }
            return account; // This method always returns the result, whether it's null or an actual account
        }
        private static void DoWithdraw(Bank bank)
        {
            Account account = FindAccount(bank);
            if (account == null)
            {
                Console.WriteLine("No valid account selected. Press any key to continue...");
                Console.ReadKey();
                return; // Exit the method early if no valid account is found
            }

            Console.Write($"Enter amount to withdraw (the limit is {account.Balance:C2}): ");
            string input = Console.ReadLine();

            if (!decimal.TryParse(input, out decimal amount))
            {
                Console.WriteLine("Invalid input. Please enter a valid decimal amount.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return; // Exit the method early
            }

            try
            {
                WithdrawTransaction withdraw = new WithdrawTransaction(account, amount);
                bank.ExecuteTransaction(withdraw); // Use Bank class method
                withdraw.Print();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        private static void DoDeposit(Bank bank)
        {
            Account account = FindAccount(bank);
            if (account == null)
            {
                Console.WriteLine("No valid account selected. Press any key to continue...");
                Console.ReadKey();
                return; // Exit the method early if no valid account is found
            }

            Console.Write($"Enter amount to deposit to {account.Name}: ");
            string input = Console.ReadLine();
            if (!decimal.TryParse(input, out decimal amount))
            {
                Console.WriteLine("Invalid input. Please enter a valid decimal amount.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return; // Exit the method early
            }

            if (amount > MaxAmountDeposit)
            {
                Console.WriteLine("The deposit amount exceeds the maximum allowed limit.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return; // Exit the method early
            }

            try
            {
                DepositTransaction deposit = new DepositTransaction(account, amount);
                bank.ExecuteTransaction(deposit);
                deposit.Print();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    

        private static void DoTransfer()
        {
            Console.WriteLine("Choose the account to transfer from:");
            SwitchAccount(); 
            Account fromAccount = currentAccount;

            Console.WriteLine("Choose the account to transfer to:");
            SwitchAccount();
            Account toAccount = currentAccount;

            if (fromAccount == toAccount)
            {
                Console.WriteLine("Cannot transfer to the same account. Operation cancelled.");
                return;
            }

            Console.Write("Enter amount to transfer: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
            {
                TransferTransaction transfer = new TransferTransaction(fromAccount, toAccount, amount);
                try
                {
                    bank.ExecuteTransaction(transfer);
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Transfer failed: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid amount. Please enter a positive number.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            currentAccount = fromAccount; // Reset the account selection to the original
        }


        private static void DoPrint(Bank bank)
            {
                Account account = FindAccount(bank);
                if (account == null)
                {
                    Console.WriteLine("No valid account selected. Press any key to continue...");
                    Console.ReadKey();
                    return; // Exit the method early if no valid account is found
                }
                account.Print();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

        }
    }


    
