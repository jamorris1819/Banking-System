using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BankingSystem;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            string passKey = "fdghfhgf";
            Bank.Initialise(passKey);

            Teller teller = Bank.CreateTeller("Jacob", "temp");
            TellerResult result = teller.CreateAccount("Emily", "pass", "", "", "");
            if (result == TellerResult.Success)
            {
                Console.WriteLine("Account was created");
                teller.CreateSession(0);
                Console.WriteLine(Bank.FindAccount(0).Balance);
                result = teller.PayInFunds(100);
                Console.WriteLine(result);
                Console.WriteLine(Bank.FindAccount(0).Balance);
                result = teller.WithdrawFunds(60);
                Console.WriteLine(result);
                Console.WriteLine(Bank.FindAccount(0).Balance);
            }
            else
            {
                Console.WriteLine("Error creating account");
            }
            Console.ReadKey();
        }
    }
}
