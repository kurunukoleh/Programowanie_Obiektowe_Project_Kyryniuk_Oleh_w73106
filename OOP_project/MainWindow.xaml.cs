using System.Collections.Generic;
using OOP_project.Data;
using OOP_project.Models;
using System.Text;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.Sqlite;

namespace Programowanie_Obiektowe_Project_Kyryniuk_Oleh_w73106
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private readonly Database _database;

        
        private List<Affix> _list_of_afficses = new();
        
        private List<Root> _list_of_roots = new();
        


        public MainWindow()
        {
            InitializeComponent();

           

            _database = new Database();

            MessageBox.Show(_database.DatabasePath);



            try
            {
                // 1. tablice i dane.
                _database.InitializeDatabase();

                // 2. alfabet.
                var phoneticSystem = new PhoneticSystem(_database);
                phoneticSystem.GenerateAlphabet(20, 1);

                // 3. affiksy.
                for (int i = 0; i < 20; i++)
                {
                    _list_of_afficses.Add(new Affix(_database));
                }

                // 4.korzeń
                for (int i = 0; i < 100; i++)
                {
                    _list_of_roots.Add(new Root(_database));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Помилка ініціалізації");
            }
        }



        private void GenerateWord_Click(
        object sender,
        RoutedEventArgs e)
            {
                try
                {
                    var database = new Database();
                    var manager = new WordManager(database);

                    List<string> forms = manager.ReturnWordList();

                    if (forms.Count == 0)
                    {
                        GeneratedWordText.Text =
                            "Nie ma korzenia";
                        return;
                    }

                    GeneratedWordText.Text =
                        string.Join(Environment.NewLine, forms);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message,
                        "Błąd generacji",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
    }
}