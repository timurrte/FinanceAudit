using System.Transactions;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Абстрактний базовий клас для всіх банківських рахунків.
/// Не може бути створений безпосередньо. Реалізує інкапсуляцію фінансових даних.
/// </summary>

namespace FinancialSystem.Domain.Models;

    public abstract class BankAccount : BaseEntity
{
    public string AccountNumber { get; set; }

    // Зв'язок із власником (Customer).
    public int CustomerId { get; set; }
    public virtual Customer Customer { get; set; }

    /// <summary>
    /// ІНКАПСУЛЯЦІЯ: Баланс може бути прочитаний ким завгодно, 
    /// але змінений лише зсередини цього класу або його спадкоємців.
    /// </summary>
    private decimal _balance;
    public decimal Balance
    {
        get => _balance;
        protected set
        {
            _balance = value;
            OnPropertyChanged(); // Tells the UI to update the DataGrid!
        }
    }

    // Композиція: Рахунок володіє своїми транзакціями. При видаленні рахунку
    // видаляються всі його транзакції.[23]
    public virtual ICollection<Transaction> Transactions { get; set; }

    protected BankAccount(string accountNumber)
    {
        AccountNumber = accountNumber;
        Balance = 0m;
        Transactions = new List<Transaction>();
    }

    /// <summary>
    /// Базовий метод поповнення рахунку. Містить логіку валідації.
    /// </summary>
    public virtual void Deposit(decimal amount, string description = "Поповнення готівкою")
    {
        if (amount <= 0)
            throw new ArgumentException("Сума поповнення повинна бути більшою за нуль.");

        Balance += amount;
        AddTransaction(new DepositTransaction(amount, description));
    }

    /// <summary>
    /// ВІРТУАЛЬНИЙ МЕТОД: Базова основа для Поліморфізму.
    /// Цей метод може бути перевизначений (overridden) у похідних класах
    /// для зміни правил зняття коштів (наприклад, для дозволу овердрафту).
    /// </summary>
    public virtual void Withdraw(decimal amount, string description = "Зняття готівки")
    {
        if (amount <= 0)
            throw new ArgumentException("Сума зняття повинна бути більшою за нуль.");

        if (Balance - amount < 0)
            throw new InvalidOperationException("Операцію відхилено: Недостатньо коштів на рахунку.");

        Balance -= amount;
        AddTransaction(new WithdrawTransaction(amount, description));
    }

    [NotMapped]
    public string AccountTypeName
    {
        get
        {
            return this.GetType().Name switch
            {
                "SavingsAccount" => "Ощадний рахунок",
                "CheckingAccount" => "Поточний рахунок",
                "CreditAccount" => "Кредитний рахунок",
                _ => "Стандартний рахунок"
            };
        }
    }

    /// <summary>
    /// Абстрактний метод для автоматичної обробки закриття фінансового місяця.
    /// Змушує всі похідні класи реалізувати власну унікальну логіку.
    /// </summary>
    public abstract void ProcessEndOfMonth();

    protected void AddTransaction(Transaction transaction)
    {
        transaction.AccountId = this.Id;
        Transactions.Add(transaction);
    }
}