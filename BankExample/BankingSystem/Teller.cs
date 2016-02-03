using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem
{
    public enum TellerPermissions { CreateAccount, DeleteAccount, DepositMoney, RemoveMoney, ViewDetails, EditDetails };
    public enum TellerResult { Success, NoSession, OverLimit, InvalidInput, NoPermissions };
    public class Teller
    {
        private int id;
        private string name;
        private string password;
        private List<string> log;
        private List<TellerPermissions> permissions;

        public int ID
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }

        public List<TellerPermissions> Permissions
        {
            get { return permissions; }
        }

        public Teller(int id, string name, string password, List<TellerPermissions> permissions)
        {
            this.id = id;
            this.name = name;
            this.password = password;
            this.permissions = permissions;
            log = new List<string>();
        }

        /// <summary>
        /// Find whether this Teller is in a session with the specified account
        /// </summary>
        /// <param name="account">Account to check for</param>
        /// <returns>Boolean</returns>
        private Account GetSession()
        {
            return Bank.FindSession(this).Account;
        }

        public void CreateSession(Account account)
        {
            Bank.CreateSession(this, account);
        }

        public TellerResult CreateAccount(string name, string password, string dob, string address, string phoneNumber)
        {
            if (!permissions.Contains(TellerPermissions.CreateAccount))
                return TellerResult.NoPermissions;
            Account account = new Account(name, password, dob, address, phoneNumber);
            BankResult result = Bank.CreateNewAccount(this, account);
            if (result == BankResult.AccountCreated)
                return TellerResult.Success;
            else
                return TellerResult.NoSession;
        }

        public TellerResult PayInFunds(decimal amount)
        {
            Account account = GetSession();
            if (account == null)
                return TellerResult.NoSession;
            if (amount > Bank.HardDepositLimit)
                return TellerResult.OverLimit;
            if (amount <= 0)
                return TellerResult.InvalidInput;
            if (!permissions.Contains(TellerPermissions.DepositMoney))
                return TellerResult.NoPermissions;
            Log(Bank.Encrypt("Started transaction for £" + amount + " into account ID " + account.ID));
            account.Log(Bank.Encrypt("Transation started for £" + amount + " deposited into account by teller ID " + ID));
            BankResult result = Bank.PayAccount(account, amount);
            if (result == BankResult.TransactionComplete)
                return TellerResult.Success;
            else
                return TellerResult.InvalidInput;
        }

        public void Log(string message)
        {
            log.Add(message);
        }
    }
}
