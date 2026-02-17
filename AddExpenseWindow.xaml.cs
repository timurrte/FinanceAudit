using System;
using System.Windows;

namespace ExpenseTracker
{
    public partial class AddExpenseWindow : Window
    {
        public Expense CreatedExpense { get; private set; }

        public AddExpenseWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDesc.Text) || !double.TryParse(txtAmount.Text, out double amount))
            {
                MessageBox.Show("Please enter a valid description and amount.");
                return;
            }

            CreatedExpense = new Expense
            {
                Description = txtDesc.Text,
                Amount = amount,
                Category = cmbCategory.Text,
                Date = dpDate.SelectedDate ?? DateTime.Now
            };

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}