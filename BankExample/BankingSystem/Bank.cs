using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JEncryption;

namespace BankingSystem
{
    public enum BankResult { AccountCreated, TellerInvalid, AccountExists, UnauthorisedAttempt, TransactionInvalid, TransactionComplete };
    public static class Bank
    {
        private static List<Teller> tellers;
        public static List<Account> accounts;
        private static List<Session> sessions;
        private static List<string> log;

        private static string encryptKey;

        private static decimal hardWithdrawLimit = 10000;
        private static decimal hardDepositLimit = 10000;

        public static decimal HardWithdrawLimit
        {
            get { return hardWithdrawLimit; }
        }

        public static decimal HardDepositLimit
        {
            get { return hardDepositLimit; }
        }

        #region Encryption
        // We will create our own encryption class to use the library. This means if the nature of the library changes, we can easily change it here, as opposed to everywhere we used it.
        public static string Encrypt(string message)
        {
            return Encryption.Encrypt(message);
        }

        public static string Decrypt(string message)
        {
            return Encryption.Decrypt(message);
        }
        #endregion

        public static void Initialise(string key)
        {
            tellers = new List<Teller>();
            accounts = new List<Account>();
            sessions = new List<Session>();
            log = new List<string>();
            encryptKey = key;
            Encryption.Initialise(key);
        }

        public static Teller CreateTeller(string name, string password)
        {
            Teller teller = new Teller(tellers.Count, name, password, new List<TellerPermissions> { TellerPermissions.CreateAccount, TellerPermissions.DepositMoney, TellerPermissions.ViewDetails, TellerPermissions.EditDetails });
            tellers.Add(teller);
            Log("Teller with ID " + teller.ID + " created");
            return teller;
        }

        /// <summary>
        /// Searches the sessions to see if there is one between the two
        /// </summary>
        /// <param name="teller">The teller</param>
        /// <param name="account">The account</param>
        /// <returns>Returns the session if found. If not, returns null</returns>
        public static Session FindSession(Teller teller)
        {
            foreach (Session session in sessions)
            {
                if (session.Teller == teller)
                    return session;
            }
            return null;
        }

        public static void Log(string message)
        {
            log.Add(Encrypt(message));
        }

        /// <summary>
        /// Returns a unique ID for a user account
        /// </summary>
        /// <returns></returns>
        public static int GetNewAccountID()
        {
            return accounts.Count;
        }

        public static void CreateSession(Teller teller, Account account)
        {
            Session session = new Session(teller, account);
            sessions.Add(session);
        }

        public static BankResult CreateNewAccount(Teller teller, Account account)
        {
            if (!tellers.Contains(teller))
                return BankResult.TellerInvalid;
            if (accounts.Contains(account))
                return BankResult.AccountExists;
            account.Log(Encrypt("Account created by " + teller.Name + " (" + teller.ID + ")"));
            teller.Log(Encrypt("Created account for ID " + account.ID));
            Log("Teller ID " + teller.ID + " created an account for account ID " + account.ID);
            accounts.Add(account);
            return BankResult.AccountCreated;
        }

        public static BankResult PayAccount(Account account, decimal amount)
        {
            // This is a very important command, so we add our own security. We send our current tick count, and the account will verify this was recent, and if it was, it will pay in.
            return account.PayInFunds(amount, DateTime.Now.Ticks);
        }
    }
}
