using System.Windows;

namespace ExpenseTracker
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void AddExpense_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddExpenseWindow();
            addWindow.Owner = this; // Centers the new window over the main one

            if (addWindow.ShowDialog() == true)
            {
                // Retrieve the data from the dialog and add it to our ViewModel
                _viewModel.AddExpense(addWindow.CreatedExpense);
            }
        }
    }
}