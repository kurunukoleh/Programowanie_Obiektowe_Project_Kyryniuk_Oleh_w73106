using Microsoft.Data.Sqlite;
using OOP_project.Data;
using OOP_project.Models;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Programowanie_Obiektowe_Project_Kyryniuk_Oleh_w73106
{
    public partial class LibraryWindow : Window
    {
        private readonly Database _database;
        private readonly RootSystem _rootSystem;
        private readonly AffixSystem _affixSystem;

        private readonly PhoneticSystem _phonetic_system;

        private readonly WordManager _wordManager;

        public LibraryWindow(Database database)
        {
            InitializeComponent();

            _database = database;
            _rootSystem = new RootSystem(_database);
            _affixSystem = new AffixSystem(_database);
            _phonetic_system = new PhoneticSystem(database);
            _wordManager = new WordManager(_database, _affixSystem, _rootSystem);
            
            var alphabet = _phonetic_system.ReadAlphabet();
            var affixes = _affixSystem.ReadAffixeInformation();

            if (alphabet.Count == 0)
            {
                _phonetic_system.GenerateAlphabet(12, 0);
            }
            if (affixes.Count == 0)
            {
                for(int i = 0; i < 10; i++)
                {
                    _affixSystem.GenerateAffix();
                }
                
            }

        }



        public LibraryWindow() : this(new Database())
        {
        }

        private void LibraryWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRoots();
            LoadAlphabet();
            LoadAffixes();
        }



        private void LoadRoots()
        {
            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                "SELECT Id, Symbol, Translation FROM Roots;",
                connection);

            using var reader = command.ExecuteReader();

            var roots = new System.Data.DataTable();
            roots.Load(reader);

            RootsGrid.ItemsSource = roots.DefaultView;
        }

        private void LoadAffixes()
        {
            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                @"SELECT
              Id,
              Affix,
              PartOfSpeechId,
              GrammaticalRoleCode,
              AffixTypeId
          FROM Affixes;",
                connection);

            using var reader = command.ExecuteReader();

            var affixes = new System.Data.DataTable();
            affixes.Load(reader);

            AffixesGrid.ItemsSource = affixes.DefaultView;
        }

        private void LoadAlphabet()
        {
            var alphabet = _phonetic_system.ReadAlphabet();

            LettersGrid.ItemsSource = alphabet
                .Select(x => new
                {
                    Symbol = x.Key,
                    Category = x.Value
                })
                .ToList();
        }

        private void LoadForms(int rootId)
        {
            FormsGrid.ItemsSource = _wordManager.ReturnWordList(rootId);
        }

        private void AddRoot_Click(object sender, RoutedEventArgs e)
        {
            string root = RootTextBox.Text.Trim();
            string translation = RootTranslateBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(root))
            {
                MessageBox.Show("Wprowadź korzeń.");
                return;
            }

            try
            {
                _rootSystem.AddRoot(root, translation);

                RootTextBox.Clear();
                RootTranslateBox.Clear();

                LoadRoots();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd");
            }
        }

        private void DeleteRoot_Click(object sender, RoutedEventArgs e)
        {
            if (RootsGrid.SelectedItem is not System.Data.DataRowView selected)
            {
                MessageBox.Show("Wybierz korzeń do usunięcia.");
                return;
            }

            int rootId = Convert.ToInt32(selected["Id"]);

            try
            {
                _rootSystem.DeleteRoot(rootId);
                LoadRoots();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Nie można usunąć korzenia.\n" + ex.Message,
                    "Błąd");
            }
        }


        private void AddAffix_Click(object sender, RoutedEventArgs e)
        {

            string affix = AffixTextBox.Text.Trim();
            string partOfSpeechId = PartOfSpeechIdBox.Text.Trim();
            string grammaticalRoleCode = GrammaticalRoleCodeBox.Text.Trim();
            string affixTypeId = AffixTypeIdBox.Text.Trim();



            if (string.IsNullOrWhiteSpace(affix))
            {
                MessageBox.Show("Wprowadź afiks.");
                return;
            }

            try
            {
                _affixSystem.AddAffix(affix, partOfSpeechId, grammaticalRoleCode, affixTypeId);

                AffixTextBox.Clear();
                PartOfSpeechIdBox.Clear();
                GrammaticalRoleCodeBox.Clear();
                AffixTypeIdBox.Clear();

                LoadAffixes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd");
            }

        }

        private void DeleteAffix_Click(object sender, RoutedEventArgs e)
        {
            if (AffixesGrid.SelectedItem is not DataRowView selected)
            {
                MessageBox.Show("Wybierz afiks do usunięcia.");
                return;
            }

            int id = Convert.ToInt32(selected["Id"]);

            try
            {
                _affixSystem.DeleteAffixes(id);
                LoadAffixes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd");
            }
        }
        private void AddLetter_Click(object sender, RoutedEventArgs e) 
        {
            string symbol = LetterTextBox.Text.Trim();
            string category = LetterCategoryComboBox.Text.Trim();


            if (string.IsNullOrWhiteSpace(symbol))
            {
                MessageBox.Show("Wprowadź afiks.");
                return;
            }

            try
            {
                _phonetic_system.AddSymbol(symbol, category);

                LetterTextBox.Clear();
                

                LoadAffixes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd");
            }


        }
        private void DeleteLetter_Click(object sender, RoutedEventArgs e) 
        {

            if (LettersGrid.SelectedItem is not DataRowView selected)
            {
                MessageBox.Show("Wybierz literę do usunięcia.");
                return;
            }

            int id = Convert.ToInt32(selected["Id"]);

            try
            {
                _phonetic_system.DeleteLetters(id);
                LoadAffixes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd");
            }

        }
        private void AddForm_Click(object sender, RoutedEventArgs e) 
        {
            
        }
        private void DeleteForm_Click(object sender, RoutedEventArgs e) 
        {
        
        }
        

        private void AddWord_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void DeleteWord_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GenerateFormsFromForm_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GenerateForms_Click(object sender, RoutedEventArgs e)
        {
            if (RootsGrid.SelectedItem is not System.Data.DataRowView selected)
            {
                MessageBox.Show("Wybierz korzeń.");
                return;
            }

            int rootId = Convert.ToInt32(selected["Id"]);

            try
            {
                LoadForms(rootId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd");
            }
        }





        private void GenerateRoots_Click(object sender, RoutedEventArgs e) 
        {
            _rootSystem.GenerateRoots();
            LoadRoots();
        }
        private void LibraryTabs_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) { }
    }
}