/// <summary>
/// Ощадний рахунок. Нараховує відсотки на позитивний залишок.
/// Успадковує базову логіку від BankAccount.
/// </summary>

namespace FinancialSystem.Domain.Models;

public class SavingsAccount : BankAccount
{
    public decimal InterestRate { get; private set; }

    public SavingsAccount(string accountNumber, decimal interestRate)
        : base(accountNumber)
    {
        InterestRate = interestRate;
    }

    /// <summary>
    /// ПОЛІМОРФІЗМ: Специфічна реалізація логіки закриття місяця.
    /// </summary>
    public override void ProcessEndOfMonth()
    {
        if (Balance > 0)
        {
            decimal interest = Balance * (InterestRate / 100);
            // Використовуємо інкапсульований метод базового класу для поповнення
            Deposit(interest, "Капіталізація щомісячних відсотків");
        }
    }
}