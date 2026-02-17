using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using LiveCharts;
using LiveCharts.Wpf;

namespace ExpenseTracker
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Expense> Expenses { get; set; }
        public SeriesCollection ExpenseSeries { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public MainViewModel()
        {
            // 1. Initialize Dummy Data
            Expenses = new ObservableCollection<Expense>
            {
                new Expense { Description = "Groceries", Amount = 150, Date = DateTime.Now.AddDays(-2), Category = "Food" },
                new Expense { Description = "Rent", Amount = 1200, Date = DateTime.Now.AddMonths(-1), Category = "Housing" },
                new Expense { Description = "Internet", Amount = 60, Date = DateTime.Now.AddMonths(-1), Category = "Utilities" },
                new Expense { Description = "Gym", Amount = 40, Date = DateTime.Now.AddMonths(-2), Category = "Health" },
                new Expense { Description = "Coffee", Amount = 5.50, Date = DateTime.Now, Category = "Food" }
            };

            // 2. Setup Charting Configuration
            ExpenseSeries = new SeriesCollection();
            YFormatter = value => value.ToString("C"); // Currency format

            UpdateChart();
        }

        public void AddExpense(Expense expense)
        {
            Expenses.Add(expense);
            UpdateChart(); // Refresh graph when data changes
        }

        private void UpdateChart()
        {
            // Group expenses by Month/Year for the graph
            var groupedData = Expenses
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    DateLabel = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    Total = g.Sum(e => e.Amount)
                })
                .ToList();

            // Clear old data and add new
            ExpenseSeries.Clear();

            ExpenseSeries.Add(new LineSeries
            {
                Title = "Total Expenses",
                Values = new ChartValues<double>(groupedData.Select(x => x.Total)),
                PointGeometrySize = 15,
                LineSmoothness = 0 // Straight lines
            });

            Labels = groupedData.Select(x => x.DateLabel).ToArray();

            OnPropertyChanged(nameof(ExpenseSeries));
            OnPropertyChanged(nameof(Labels));
        }

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}