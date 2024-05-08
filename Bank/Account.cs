using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Application_Extended
{

        public class Account
        {
            private decimal _balance;
            private string _name;

            public Account(string name, decimal balance)
            {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty or null.");
            }
            if (name.Length > 50) //  maximum name length of 50 characters
            {
                throw new ArgumentException("Name cannot be longer than 50 characters.");
            }
            if (balance < 0)
            {
                throw new ArgumentException("Initial balance must be positive.");
            }               
                _name = name;
                _balance = balance;
            }

        //Deposit Method Adds funds to the account increasing the _balance by the specified amount if the amount is positive.
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be positive.");
            }

            _balance += amount;
        }

        public void Withdraw(decimal amount)
        {

            if (amount <= 0)
            {
                throw new ArgumentException("Withdrawal amount must be positive.");
            }
            if (amount > _balance)
            {
                throw new InvalidOperationException("Insufficient Funds!");
            }

            _balance -= amount;
        }
        public void Print()
            {
                Console.WriteLine("Hi! Your current " + _name +"'s balance is: " + _balance.ToString("C2"));
            }

            //Property Name  - gets the name of the account

            public String Name
            {
                get { return _name; }
            }
        public decimal Balance
        {
            get { return _balance; }
        }

    }
}
