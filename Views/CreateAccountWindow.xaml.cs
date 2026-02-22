using FinancialSystem.DataAccess;
using FinancialSystem.Domain.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FinancialSystem.Views
{
    public partial class CreateAccountWindow : Window
    {
        private readonly Customer _selectedCustomer;

        public CreateAccountWindow(Customer customer)
        {
            InitializeComponent();
            _selectedCustomer = customer;

            this.Title = $"Новий рахунок для: {_selectedCustomer.FirstName} {_selectedCustomer.LastName}";
        }

        private void AccountTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OverdraftPanel == null || CreditPanel == null || BalancePanel == null) return;

            OverdraftPanel.Visibility = Visibility.Collapsed;
            CreditPanel.Visibility = Visibility.Collapsed;
            BalancePanel.Visibility = Visibility.Visible;

            switch (AccountTypeComboBox.SelectedIndex)
            {
                case 0: // Savings
                    break;
                case 1: // Checking
                    OverdraftPanel.Visibility = Visibility.Visible;
                    break;
                case 2: // Credit
                    CreditPanel.Visibility = Visibility.Visible;
                    BalancePanel.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new FinancialDbContext())
            {
                BankAccount newAccount = null;
                string newAccountNumber = $"UA{new Random().Next(1000000000, int.MaxValue)}";

                decimal.TryParse(BalanceTextBox.Text, out decimal initialBalance);

                try
                {
                    if (AccountTypeComboBox.SelectedIndex == 0) // Savings
                    {
                        newAccount = new SavingsAccount(newAccountNumber, initialBalance);
                    }
                    else if (AccountTypeComboBox.SelectedIndex == 1) // Checking
                    {
                        decimal overdraft = decimal.Parse(OverdraftTextBox.Text);
                        newAccount = new CheckingAccount(newAccountNumber, initialBalance, overdraft);
                    }
                    else if (AccountTypeComboBox.SelectedIndex == 2) // Credit
                    {
                        decimal limit = decimal.Parse(CreditLimitTextBox.Text);
                        decimal penalty = decimal.Parse(PenaltyRateTextBox.Text);
                        newAccount = new CreditAccount(newAccountNumber, limit, penalty);
                    }

                    newAccount.CustomerId = _selectedCustomer.Id;
                    context.BankAccounts.Add(newAccount);
                    context.SaveChanges();

                    MessageBox.Show("Рахунок успішно створено!");
                    this.DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка введення даних: {ex.Message}");
                }
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}