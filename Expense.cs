using System;

namespace ExpenseTracker
{
    public class Expense
    {
        public string Description { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }

        // Helper for formatting in the UI
        public string DisplayDate => Date.ToString("MMM dd, yyyy");
    }
}