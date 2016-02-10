using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem
{
    public enum TellerPermissions { CreateAccount, DeleteAccount, DepositMoney, WithdrawMoney, EditDetails };
    public enum TellerResult { Success, NoSession, OverLimit, InvalidInput, NoPermissions, Unauthorised };
    public class Teller
    {
        private int id;
        private string name;
        private string password;
        private List<string> log;
        private List<TellerPermissions> permissions;
        private Account currentAccount;

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

        public Account CurrentAccount
        {
            get { return currentAccount; }
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

        /// <summary>
        /// Create a session
        /// </summary>
        /// <param name="account">The account to create a session with</param>
        public void CreateSession(Account account)
        {
            Bank.CreateSession(this, account);
        }

        /// <summary>
        /// Create a session
        /// </summary>
        /// <param name="id">The account's ID</param>
        public void CreateSession(int id)
        {
            Bank.CreateSession(this, id);
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
            Log(Bank.Encrypt("Started deposit for £" + amount + " into account ID " + account.ID));
            account.Log(Bank.Encrypt("Deposit started for £" + amount + " into account by teller ID " + ID));
            BankResult result = Bank.PayAccount(account, amount);
            if (result == BankResult.TransactionComplete)
                return TellerResult.Success;
            else if (result == BankResult.UnauthorisedAttempt)
                return TellerResult.Unauthorised;
            else
                return TellerResult.InvalidInput;
        }

        public TellerResult WithdrawFunds(decimal amount)
        {
            Account account = GetSession();
            if (account == null)
                return TellerResult.NoSession;
            if (amount > Bank.HardWithdrawLimit)
                return TellerResult.OverLimit;
            if (amount <= 0)
                return TellerResult.InvalidInput;
            if (!permissions.Contains(TellerPermissions.WithdrawMoney))
                return TellerResult.NoPermissions;
            Log(Bank.Encrypt("Started withdrawal for £" + amount + " for account ID " + account.ID));
            account.Log(Bank.Encrypt("Withdrawal started for £" + amount + " by teller ID " + ID));
            BankResult result = Bank.WithdrawFunds(account, amount);
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
