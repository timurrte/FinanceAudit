using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialSystem.Domain.Interfaces
{
    /// <summary>
    /// Базовий інтерфейс для всіх сутностей бази даних.
    /// Забезпечує наявність первинного ключа.
    /// </summary>
    internal interface IEntity
    {
        int Id { get; }
    }
}
