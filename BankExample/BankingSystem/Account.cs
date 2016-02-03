using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public BankResult PayInFunds(decimal amount, long code)
        {
            long difference = DateTime.Now.Ticks - code;
            // We get 25000-3000 here, so to be safe we'll say 40000
            if (difference >= 40000)
            {
                return BankResult.UnauthorisedAttempt;
            }
            if (amount < 0 || amount > Bank.HardDepositLimit)
                return BankResult.TransactionInvalid;
            Log(Bank.Encrypt("Transaction successfully cleared"));
            balance += amount;
            return BankResult.TransactionComplete;
        }
    }
}
