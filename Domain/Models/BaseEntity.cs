using FinancialSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

/// <summary>
/// Базовий абстрактний клас сутності. Реалізує інтерфейс IEntity.
/// </summary>
namespace FinancialSystem.Domain.Models;

public abstract class BaseEntity : IEntity
{
    public int Id { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}