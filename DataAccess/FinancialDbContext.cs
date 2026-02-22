namespace FinancialSystem.DataAccess
{
    using FinancialSystem.Domain.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Контекст бази даних Entity Framework Core.
    /// </summary>
    public class FinancialDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public FinancialDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Використання SQLite як локальної, безсерверної бази даних
            optionsBuilder.UseSqlite("Data Source=financial.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BankAccount>()
               .HasDiscriminator<string>("AccountType")
               .HasValue<SavingsAccount>("Savings")
               .HasValue<CheckingAccount>("Checking")
               .HasValue<CreditAccount>("Credit");

            modelBuilder.Entity<Transaction>()
               .HasDiscriminator<string>("TransactionType")
               .HasValue<DepositTransaction>("Deposit")
               .HasValue<WithdrawTransaction>("Withdraw");

            modelBuilder.Entity<Customer>()
               .Property(c => c.FirstName).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<Customer>()
               .Property(c => c.LastName).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<BankAccount>()
               .HasMany(b => b.Transactions)
               .WithOne(t => t.Account)
               .HasForeignKey(t => t.AccountId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}