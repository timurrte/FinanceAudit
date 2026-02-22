using System;
using System.Windows;

namespace FinancialSystem
{
    using FinancialSystem.Views;
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Initialize the WPF application
            var app = new Application();
            app.Run(new MainWindow());
        }
    }
}