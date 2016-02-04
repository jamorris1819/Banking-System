using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankingSystem
{
    public class Account
    {
        private int id;
        private string password;
        private string name;
        private string dob;
        private decimal balance;
        private string address;
        private string phoneNumber;
        private List<string> log;

        public int ID
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }

        public string DOB
        {
            get { return dob; }
        }

        public string Address
        {
            get { return address; }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
        }

        public decimal Balance
        {
            get { return balance; }
        }

        public Account(string name, string password, string dob, string address, string phoneNumber)
        {
            id = Bank.GetNewAccountID();
            balance = 0;
            log = new List<string>();
            this.name = name;
            this.password = password;
            this.dob = dob;
            this.address = address;
            this.phoneNumber = phoneNumber;
        }

        public void Log(string message)
        {
            log.Add(message);
        }

        private bool CheckAuthorised(DateTime code)
        {
            Thread.Sleep(10);
            TimeSpan difference = DateTime.Now - code;
            return difference.Milliseconds < 500;
        }

        public BankResult PayInFunds(decimal amount, DateTime code)
        {
            if (!CheckAuthorised(code))
                return BankResult.UnauthorisedAttempt;
            if (amount < 0 || amount > Bank.HardDepositLimit)
                return BankResult.TransactionInvalid;
            Log(Bank.Encrypt("Deposit successfully cleared"));
            balance += amount;
            return BankResult.TransactionComplete;
        }

        public BankResult WithdrawFunds(decimal amount, DateTime code)
        {
            if (!CheckAuthorised(code))
                return BankResult.UnauthorisedAttempt;
            if (amount < 0 || amount > Bank.HardWithdrawLimit)
            {
                return BankResult.TransactionInvalid;
            }
            Log(Bank.Encrypt("Withdrawal successfully cleared"));
            balance -= amount;
            return BankResult.TransactionComplete;
        }
    }
}
