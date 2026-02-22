using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using FinancialSystem.Domain.Models;
using FinancialSystem.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Windows.Data;

namespace FinancialSystem.ViewModels
{
    // БАЗОВИЙ КЛАС VIEWMODEL
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
        public class MainViewModel : ViewModelBase
        {
            private readonly CustomersViewModel _customersViewModel;
            private readonly AccountsViewModel _accountsViewModel;

            private ViewModelBase _currentViewModel;

            public ViewModelBase CurrentViewModel
            {
                get => _currentViewModel;
                set { _currentViewModel = value; OnPropertyChanged(); }
            }

            public ICommand ShowCustomersCommand { get; }
            public ICommand ShowAccountsCommand { get; }

            public MainViewModel()
            {
                _customersViewModel = new CustomersViewModel();
                _accountsViewModel = new AccountsViewModel();

                ShowCustomersCommand = new RelayCommand(o => CurrentViewModel = _customersViewModel);
                ShowAccountsCommand = new RelayCommand(o => CurrentViewModel = _accountsViewModel);

                CurrentViewModel = _customersViewModel;
            }
        }

    // VIEWMODEL ДЛЯ УПРАВЛІННЯ КЛІЄНТАМИ
    public class CustomersViewModel : ViewModelBase
    {
        private ObservableCollection<Customer> _customers;
        private Customer _selectedCustomer;

        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set { _customers = value; OnPropertyChanged(); }
        }

        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set { _selectedCustomer = value; OnPropertyChanged(); }
        }

        public ICommand AddCustomerCommand { get; }
        public ICommand DeleteCustomerCommand { get; }

        public CustomersViewModel()
        {
            AddCustomerCommand = new RelayCommand(AddCustomer);
            DeleteCustomerCommand = new RelayCommand(DeleteCustomer, CanDeleteCustomer);

            LoadCustomers();
        }

        private void LoadCustomers()
        {
            using (var context = new FinancialDbContext())
            {
                var customersFromDb = context.Customers.ToList();
                Customers = new ObservableCollection<Customer>(customersFromDb);
            }
        }

        private void AddCustomer(object parameter)
        {
            var dialog = new FinancialSystem.Views.CreateCustomerWindow();

            bool? result = dialog.ShowDialog();

            if (result == true && dialog.CreatedCustomer != null)
            {
                var newCustomer = dialog.CreatedCustomer;

                using (var context = new FinancialDbContext())
                {
                    context.Customers.Add(newCustomer);
                    context.SaveChanges();
                }

                Customers.Add(newCustomer);
            }
        }

        private bool CanDeleteCustomer(object parameter)
        {
            return SelectedCustomer != null;
        }

        private void DeleteCustomer(object parameter)
        {
            if (SelectedCustomer != null)
            {
                using (var context = new FinancialDbContext())
                {
                    context.Customers.Remove(SelectedCustomer);
                    context.SaveChanges();
                }
                Customers.Remove(SelectedCustomer);
            }
        }
    }

    // VIEWMODEL ДЛЯ УПРАВЛІННЯ РАХУНКАМИ ТА ТРАНЗАКЦІЯМИ
    public class AccountsViewModel : ViewModelBase
    {
        private ObservableCollection<BankAccount> _accounts;
        private BankAccount _selectedAccount;
        private decimal _transactionAmount;

        public ObservableCollection<BankAccount> Accounts
        {
            get => _accounts;
            set { _accounts = value; OnPropertyChanged(); }
        }

        public BankAccount SelectedAccount
        {
            get => _selectedAccount;
            set { _selectedAccount = value; OnPropertyChanged(); }
        }

        public decimal TransactionAmount
        {
            get => _transactionAmount;
            set { _transactionAmount = value; OnPropertyChanged(); }
        }

        public ICommand DepositCommand { get; }
        public ICommand WithdrawCommand { get; }

        public AccountsViewModel()
        {
            DepositCommand = new RelayCommand(Deposit, CanExecuteTransaction);
            WithdrawCommand = new RelayCommand(Withdraw, CanExecuteTransaction);

            LoadAccounts();
        }

        private void LoadAccounts()
        {
            using (var context = new FinancialDbContext())
            {
                var accountsFromDb = context.BankAccounts.Include(a => a.Customer).ToList();
                Accounts = new ObservableCollection<BankAccount>(accountsFromDb);
            }
        }

        private bool CanExecuteTransaction(object parameter)
        {
            return SelectedAccount != null && TransactionAmount > 0;
        }

        private void Deposit(object parameter)
        {
            try
            {
                SelectedAccount.Deposit(TransactionAmount);
                SaveChangesToDatabase();

                TransactionAmount = 0;
                OnPropertyChanged(nameof(SelectedAccount));
                CollectionViewSource.GetDefaultView(Accounts).Refresh();
            }
            catch (Exception ex)
            {
            }
        }

        private void Withdraw(object parameter)
        {
            try
            {
                SelectedAccount.Withdraw(TransactionAmount);
                SaveChangesToDatabase();

                TransactionAmount = 0;
                OnPropertyChanged(nameof(SelectedAccount));
                CollectionViewSource.GetDefaultView(Accounts).Refresh();
            }
            catch (InvalidOperationException ex)
            {
            }
        }

        private void SaveChangesToDatabase()
        {
            using (var context = new FinancialDbContext())
            {
                context.BankAccounts.Update(SelectedAccount);
                context.SaveChanges();
            }
        }
    }
}