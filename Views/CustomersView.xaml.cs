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
            if (this.DataContext is CustomersViewModel viewModel)
            {
                if (viewModel.SelectedCustomer == null)
                {
                    MessageBox.Show("Будь ласка, оберіть клієнта зі списку перед створенням рахунку.",
                                    "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var dialog = new CreateAccountWindow(viewModel.SelectedCustomer);

                bool? result = dialog.ShowDialog();
            }
        }
    }
}