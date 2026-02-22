/// <summary>
/// Розрахунковий рахунок.
/// </summary>

namespace FinancialSystem.Domain.Models;

public class CheckingAccount : BankAccount
{
    public decimal OverdraftLimit { get; private set; }
    public decimal MonthlyFee { get; private set; }

    public CheckingAccount(string accountNumber, decimal overdraftLimit, decimal monthlyFee)
        : base(accountNumber)
    {
        OverdraftLimit = overdraftLimit;
        MonthlyFee = monthlyFee;
    }

    /// <summary>
    /// Перевизначення методу зняття коштів.
    /// Замість перевірки на нуль, перевіряє ліміт овердрафту.
    /// </summary>
    public override void Withdraw(decimal amount, string description = "Зняття з поточного рахунку")
    {
        if (amount <= 0)
            throw new ArgumentException("Сума зняття повинна бути більшою за нуль.");

        // Допускаємо від'ємний баланс, але не більше ніж встановлений ліміт
        if (Balance - amount < -OverdraftLimit)
            throw new InvalidOperationException($"Операцію відхилено: Перевищено ліміт овердрафту ({OverdraftLimit}).");

        Balance -= amount;
        AddTransaction(new WithdrawTransaction(amount, description));
    }

    public override void ProcessEndOfMonth()
    {
        Withdraw(MonthlyFee, "Щомісячна комісія за обслуговування рахунку");
    }
}