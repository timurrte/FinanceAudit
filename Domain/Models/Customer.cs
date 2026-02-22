using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Клас, що представляє клієнта банку.
/// </summary>
namespace FinancialSystem.Domain.Models;

public class Customer : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IdentityNumber { get; set; }
    public string Address { get; set; }

    public virtual ICollection<BankAccount> Accounts { get; set; }

    public Customer()
    {
        Accounts = new List<BankAccount>();
    }

    public string FullName => $"{FirstName} {LastName}";
}