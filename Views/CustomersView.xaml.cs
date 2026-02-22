using System.Windows;
using System.Windows.Controls;
using FinancialSystem.ViewModels; // Ensure this is here

namespace FinancialSystem.Views
{
    public partial class CustomersView : UserControl
    {
        public CustomersView()
        {
            InitializeComponent();
        }

        private void OpenCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            // Get the ViewModel attached to this View
            if (this.DataContext is CustomersViewModel viewModel)
            {
                // Ensure a customer is actually selected
                if (viewModel.SelectedCustomer == null)
                {
                    MessageBox.Show("Будь ласка, оберіть клієнта зі списку перед створенням рахунку.",
                                    "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Open the window and pass the selected customer
                var dialog = new CreateAccountWindow(viewModel.SelectedCustomer);

                // ShowDialog() pauses the code here until the child window is closed
                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    // Optional: If you want to refresh the accounts list after saving, 
                    // you might trigger a refresh method on your viewmodel here.
                }
            }
        }
    }
}