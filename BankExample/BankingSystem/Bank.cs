using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JEncryption;

namespace BankingSystem
{
    public delegate void ChangedEventHandler(object sender, EventArgs e);
    public enum BankResult { AccountCreated, TellerInvalid, AccountExists, UnauthorisedAttempt, TransactionInvalid, TransactionComplete };
    public static class Bank
    {
        public static event ChangedEventHandler LogChanged;

        private static Dictionary<int, Teller> tellers;
        private static Dictionary<int, Account> accounts;
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

        private static void EventLogChanged(EventArgs e)
        {
            if (LogChanged != null)
                LogChanged(null, e);
        }

        public static void Initialise(string key)
        {
            tellers = new Dictionary<int, Teller>();
            accounts = new Dictionary<int, Account>();
            sessions = new List<Session>();
            log = new List<string>();
            encryptKey = key;
            Encryption.Initialise(key);
        }

        public static Teller CreateTeller(string name, string password, List<TellerPermissions> permissions)
        {
            Teller teller = new Teller(tellers.Count, name, password, permissions);
            tellers.Add(teller.ID, teller);
            Log("Teller (" + teller.Name + ") with ID " + teller.ID + " created");
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
            EventLogChanged(EventArgs.Empty);
        }

        public static string GetLastLog()
        {
            return log[log.Count - 1];
        }

        /// <summary>
        /// Returns a unique ID for a user account
        /// </summary>
        /// <returns></returns>
        public static int GetNewAccountID()
        {
            return accounts.Count;
        }

        /// <summary>
        /// Returns a unique ID for a teller account
        /// </summary>
        /// <returns></returns>
        public static int GetNewTellerID()
        {
            return tellers.Count;
        }

        /// <summary>
        /// Creates a session between a teller and an account
        /// </summary>
        /// <param name="teller">Teller</param>
        /// <param name="account">Account</param>
        public static void CreateSession(Teller teller, Account account)
        {
            Session session = new Session(teller, account);
            sessions.Add(session);
        }

        /// <summary>
        /// Creates a session between a teller and an account
        /// </summary>
        /// <param name="teller">Teller</param>
        /// <param name="account">Account</param>
        public static void CreateSession(Teller teller, int id)
        {
            CreateSession(teller, FindAccount(id));
        }
        
        /// <summary>
        /// Finds an account with the specified ID
        /// </summary>
        /// <param name="id">ID of sought account</param>
        /// <returns></returns>
        public static Account FindAccount(int id)
        {
            return accounts[id];
        }

        /// <summary>
        /// Creates a new account without using a session
        /// </summary>
        /// <param name="teller">Teller</param>
        /// <param name="account">Account</param>
        /// <returns></returns>
        public static BankResult CreateNewAccount(Teller teller, Account account)
        {
            if (!tellers.ContainsKey(teller.ID))
                return BankResult.TellerInvalid;
            if (accounts.ContainsValue(account))
                return BankResult.AccountExists;
            account.Log(Encrypt("Account created by " + teller.Name + " (" + teller.ID + ")"));
            teller.Log(Encrypt("Created account for ID " + account.ID));
            Log("Teller ID " + teller.ID + " created an account for account ID " + account.ID);
            accounts.Add(account.ID, account);
            return BankResult.AccountCreated;
        }

        public static BankResult PayAccount(Account account, decimal amount)
        {
            // This is a very important command, so we add our own security. We send our current tick count, and the account will verify this was recent, and if it was, it will pay in.
            return account.PayInFunds(amount, DateTime.Now);
        }

        public static BankResult WithdrawFunds(Account account, decimal amount)
        {
            return account.WithdrawFunds(amount, DateTime.Now);
        }
    }
}
