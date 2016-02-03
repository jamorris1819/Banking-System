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
            Bank.Initialise("secret key");
            Teller teller = Bank.CreateTeller("Jacob", "temp");
            TellerResult result = teller.CreateAccount("Emily", "pass", "", "", "");
            teller.CreateSession(Bank.accounts[0]);
            result = teller.PayInFunds(200);
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}
