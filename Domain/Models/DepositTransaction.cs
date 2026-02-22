/// <summary>
/// Транзакція зарахування коштів
/// </summary>
namespace FinancialSystem.Domain.Models;

public class DepositTransaction : Transaction
{
    public DepositTransaction(decimal amount, string description)
        : base(amount, description) { }

    public override void Execute(BankAccount account)
    {
        account.Deposit(this.Amount, this.Description);
    }
}