using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem
{
    public class Session
    {
        private Teller teller;
        private Account account;

        public Teller Teller
        {
            get { return teller; }
        }

        public Account Account
        {
            get { return account; }
        }

        public Session(Teller teller, Account account)
        {
            this.teller = teller;
            this.account = account;
        }
    }
}
