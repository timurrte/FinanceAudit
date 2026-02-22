using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Клас, що представляє клієнта банку.
/// Демонструє відношення Агрегації з BankAccount (1 до багатьох).
/// </summary>
namespace FinancialSystem.Domain.Models;

public class Customer : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IdentityNumber { get; set; }
    public string Address { get; set; }

    // Навігаційна властивість для Entity Framework Core.
    // Virtual дозволяє використання механізму Lazy Loading (ліниве завантаження).
    public virtual ICollection<BankAccount> Accounts { get; set; }

    public Customer()
    {
        Accounts = new List<BankAccount>();
    }

    public string FullName => $"{FirstName} {LastName}";
}