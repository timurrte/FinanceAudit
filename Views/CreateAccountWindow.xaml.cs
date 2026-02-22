using System;
using System.Windows;
using FinancialSystem.Domain.Models;
using FinancialSystem.DataAccess;

namespace FinancialSystem.Views
{
    public partial class CreateAccountWindow : Window
    {
        private readonly Customer _selectedCustomer;

        // Modify the constructor to require a Customer
        public CreateAccountWindow(Customer customer)
        {
            InitializeComponent();
            _selectedCustomer = customer;

            // Optional: Update title to show who this is for
            this.Title = $"Новий рахунок для: {_selectedCustomer.FirstName} {_selectedCustomer.LastName}";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(BalanceTextBox.Text, out decimal initialBalance))
            {
                using (var context = new FinancialDbContext())
                {
                    BankAccount newAccount = null;

                    // 1. Generate a mock Account Number (e.g., UA + 10 random digits)
                    // In a real app, this might come from a sequence in the database
                    string newAccountNumber = $"UA{new Random().Next(1000000000, int.MaxValue)}";

                    if (AccountTypeComboBox.SelectedIndex == 0)
                    {
                        // SavingsAccount requires: string accountNumber, decimal balance
                        newAccount = new SavingsAccount(newAccountNumber, initialBalance);
                    }
                    else if (AccountTypeComboBox.SelectedIndex == 1)
                    {
                        // CheckingAccount requires: string accountNumber, decimal balance, decimal overdraftLimit
                        decimal defaultOverdraft = 1000;
                        newAccount = new CheckingAccount(newAccountNumber, initialBalance, defaultOverdraft);
                    }
                    else if (AccountTypeComboBox.SelectedIndex == 2)
                    {
                        // CheckingAccount requires: string accountNumber, decimal balance, decimal overdraftLimit
                        decimal creditLimit = 5000;
                        decimal penaltyRate = 0.05m;
                        newAccount = new CreditAccount(newAccountNumber, creditLimit, penaltyRate);
                    }

                    newAccount.CustomerId = _selectedCustomer.Id; 

                    context.BankAccounts.Add(newAccount);
                    context.SaveChanges();
                }

                MessageBox.Show("Рахунок успішно створено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Будь ласка, введіть коректну суму.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}