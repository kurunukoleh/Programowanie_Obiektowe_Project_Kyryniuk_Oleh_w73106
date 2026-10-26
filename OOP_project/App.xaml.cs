using System;
using System.Windows;
using OOP_project.Data;

namespace Programowanie_Obiektowe_Project_Kyryniuk_Oleh_w73106
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var database = new Database();
                database.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Database initialization error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Shutdown();
            }
        }
    }
}