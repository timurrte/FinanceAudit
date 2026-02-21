
/// <summary>
/// Кредитний рахунок. Баланс представляє суму боргу.
/// </summary>
namespace FinancialSystem.Domain.Models;

public class CreditAccount : BankAccount
{
    public decimal CreditLimit { get; private set; }
    public decimal PenaltyRate { get; private set; }

    public CreditAccount(string accountNumber, decimal creditLimit, decimal penaltyRate)
        : base(accountNumber)
    {
        CreditLimit = creditLimit;
        PenaltyRate = penaltyRate;
    }

    public override void Withdraw(decimal amount, string description = "Використання кредитних коштів")
    {
        if (amount <= 0) throw new ArgumentException("Сума повинна бути більшою за нуль.");

        if (Balance - amount < -CreditLimit)
            throw new InvalidOperationException("Перевищено доступний кредитний ліміт.");

        Balance -= amount;
        AddTransaction(new WithdrawTransaction(amount, description));
    }

    public override void ProcessEndOfMonth()
    {
        // Якщо баланс від'ємний (користувач винен банку), нараховується відсоток на борг
        if (Balance < 0)
        {
            decimal penalty = Math.Abs(Balance) * (PenaltyRate / 100);
            // Нарахування боргу здійснюється через "Зняття" (зменшення балансу)
            Withdraw(penalty, "Нарахування відсотків за користування кредитом");
        }
    }
}
