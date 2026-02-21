using System;
using System.Collections.Generic;
using System.Text;
using FinancialSystem.Domain.Models;

namespace FinancialSystem.Domain.Interfaces
{
    public interface ITransactionProcessor
    {
        void Execute(BankAccount account);
    }
}
