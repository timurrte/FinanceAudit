using FinancialSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;

/// <summary>
/// Базовий абстрактний клас сутності. Реалізує інтерфейс IEntity.
/// </summary>
namespace FinancialSystem.Domain.Models;

public abstract class BaseEntity : IEntity
{
    public int Id { get; set; }
}