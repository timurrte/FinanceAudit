using System.Windows;
using FinancialSystem.Domain.Models;

namespace FinancialSystem.Views
{
    public partial class CreateCustomerWindow : Window
    {
        // Властивість для зберігання створеного клієнта
        public Customer CreatedCustomer { get; private set; }

        public CreateCustomerWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Базова перевірка, чи не порожні поля
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(AddressTextBox.Text))
            {
                MessageBox.Show("Будь ласка, заповніть всі обов'язкові поля (Ім'я, Прізвище, Адреса).",
                                "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Створюємо нового клієнта з введених даних
            CreatedCustomer = new Customer
            {
                FirstName = FirstNameTextBox.Text.Trim(),
                LastName = LastNameTextBox.Text.Trim(),
                IdentityNumber = IdentityNumberTextBox.Text.Trim(),
                Address = AddressTextBox.Text.Trim() // Тепер база даних не видасть помилку NOT NULL!
            };

            this.DialogResult = true; // Сигнал, що все пройшло успішно
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}