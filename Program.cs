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

            // Run the application, telling it to start with your new XAML window
            app.Run(new MainWindow());
        }
    }
}