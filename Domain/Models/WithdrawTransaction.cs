
/// <summary>
/// Транзакція списання коштів (Спадкоємець 2).
/// </summary>
namespace FinancialSystem.Domain.Models;

public class WithdrawTransaction : Transaction
{
    public WithdrawTransaction(decimal amount, string description)
        : base(amount, description) { }

    public override void Execute(BankAccount account)
    {
        account.Withdraw(this.Amount, this.Description);
    }
}