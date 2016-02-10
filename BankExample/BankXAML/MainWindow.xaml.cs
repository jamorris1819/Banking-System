using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using BankingSystem;

namespace BankXAML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Bank.Initialise("super secret password");
        }

        private void CreateTellerButton_Click(object sender, RoutedEventArgs e)
        {
            string name = tbCreateTellerName.Text;
            string password = tbCreateTellerPassword.Password;
            List<TellerPermissions> permissions = new List<TellerPermissions>();
            string errors = string.Empty;
            if (string.IsNullOrEmpty(name))
                errors += "You must enter a name\n";
            if (string.IsNullOrEmpty(password))
                errors += "You must enter a password\n";
            if (string.IsNullOrEmpty(errors))
            {
                password = Bank.Encrypt(password);
                if (cbCreateAccounts.IsChecked.Value)
                    permissions.Add(TellerPermissions.CreateAccount);
                if (cbDeleteAccounts.IsChecked.Value)
                    permissions.Add(TellerPermissions.DeleteAccount);
                if (cbWithdrawMoney.IsChecked.Value)
                    permissions.Add(TellerPermissions.WithdrawMoney);
                if (cbDepositMoney.IsChecked.Value)
                    permissions.Add(TellerPermissions.DepositMoney);
                if (cbEditDetails.IsChecked.Value)
                    permissions.Add(TellerPermissions.EditDetails);
                Teller teller = Bank.CreateTeller(name, password, permissions);
            }
            lbCreateTellerErrors.Text = errors;
        }
    }
}
