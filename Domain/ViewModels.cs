using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using FinancialSystem.Domain.Models;
using FinancialSystem.DataAccess;

namespace FinancialSystem.ViewModels
{
    // 1. БАЗОВИЙ КЛАС VIEWMODEL
    // Забезпечує механізм сповіщення інтерфейсу про зміну даних.
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Використання CallerMemberName дозволяє не писати ім'я властивості вручну
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Допоміжний клас для прив'язки кнопок (Commands) до методів у ViewModel
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

    // 2. VIEWMODEL ДЛЯ УПРАВЛІННЯ КЛІЄНТАМИ
    public class CustomersViewModel : ViewModelBase
    {
        private ObservableCollection<Customer> _customers;
        private Customer _selectedCustomer;

        // ObservableCollection автоматично оновлює DataGrid при додаванні/видаленні елементів 
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
            // У реальному додатку тут би відкривалося діалогове вікно.
            // Для прикладу створюємо тестового клієнта.
            var newCustomer = new Customer
            {
                FirstName = "Новий",
                LastName = "Клієнт",
                IdentityNumber = "1234567890"
            };

            using (var context = new FinancialDbContext())
            {
                context.Customers.Add(newCustomer);
                context.SaveChanges();
            }

            Customers.Add(newCustomer);
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

    // 3. VIEWMODEL ДЛЯ УПРАВЛІННЯ РАХУНКАМИ ТА ТРАНЗАКЦІЯМИ
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

        // Властивість, яка прив'язується до текстового поля суми в графічному інтерфейсі
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
                var accountsFromDb = context.BankAccounts.ToList();
                Accounts = new ObservableCollection<BankAccount>(accountsFromDb);
            }
        }

        private bool CanExecuteTransaction(object parameter)
        {
            // Транзакція можлива лише якщо обрано рахунок і сума більша за нуль
            return SelectedAccount != null && TransactionAmount > 0;
        }

        private void Deposit(object parameter)
        {
            try
            {
                // Виклик інкапсульованого методу бізнес-логіки
                SelectedAccount.Deposit(TransactionAmount);
                SaveChangesToDatabase();

                TransactionAmount = 0; // Очищення поля після успішної операції
                OnPropertyChanged(nameof(SelectedAccount)); // Оновлення інтерфейсу
            }
            catch (Exception ex)
            {
                // Обробка помилок (наприклад, виведення MessageBox)
            }
        }

        private void Withdraw(object parameter)
        {
            try
            {
                // Завдяки поліморфізму, тут викликається специфічний метод Withdraw 
                // для конкретного типу рахунку (Savings, Checking тощо)
                SelectedAccount.Withdraw(TransactionAmount);
                SaveChangesToDatabase();

                TransactionAmount = 0;
                OnPropertyChanged(nameof(SelectedAccount));
            }
            catch (InvalidOperationException ex)
            {
                // Перехоплення помилок овердрафту або нестачі коштів
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