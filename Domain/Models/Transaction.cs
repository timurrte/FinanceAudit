using FinancialSystem.Domain.Interfaces;

/// <summary>
/// Базовий абстрактний клас для всіх фінансових транзакцій.
/// Реалізує інтерфейс ITransactionProcessor.
/// </summary>
namespace FinancialSystem.Domain.Models;

public abstract class Transaction : BaseEntity, ITransactionProcessor
{
    public int AccountId { get; set; }
    public virtual BankAccount Account { get; set; }

    public decimal Amount { get; protected set; }
    public DateTime Date { get; protected set; }
    public string Description { get; protected set; }

    protected Transaction(decimal amount, string description)
    {
        Amount = amount;
        Description = description;
        Date = DateTime.UtcNow;
    }

    public abstract void Execute(BankAccount account);
}