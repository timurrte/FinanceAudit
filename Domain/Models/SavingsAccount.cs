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

    public override void ProcessEndOfMonth()
    {
        if (Balance > 0)
        {
            decimal interest = Balance * (InterestRate / 100);
            Deposit(interest, "Капіталізація щомісячних відсотків");
        }
    }
}