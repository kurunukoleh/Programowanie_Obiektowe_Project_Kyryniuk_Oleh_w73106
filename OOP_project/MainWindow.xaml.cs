using Microsoft.Data.Sqlite;
using OOP_project;
using OOP_project.Data;
using OOP_project.Models;
using System.Data;
using System.Linq;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace Programowanie_Obiektowe_Project_Kyryniuk_Oleh_w73106
{
    public partial class LibraryWindow : Window
    {
        private readonly Database _database;
        private readonly RootSystem _rootSystem;
        private readonly AffixSystem _affixSystem;

        private readonly PhoneticSystem _phonetic_system;

        private readonly WordManager _wordManager;

        public LibraryWindow(Database database, Window? owner = null)
        {
            InitializeComponent();

            _database = database;
            _rootSystem = new RootSystem(_database);
            _affixSystem = new AffixSystem(_database);
            _phonetic_system = new PhoneticSystem(_database);
            _wordManager = new WordManager(
                _database, _affixSystem, _rootSystem);

            if (owner != null && owner.IsLoaded)
                Owner = owner;

            Loaded += LibraryWindow_Loaded;
        }

        public LibraryWindow() : this(new Database())
        {
        }



        private void LibraryWindow_Loaded(object sender, RoutedEventArgs e)
        {
            bool alphabetIsEmpty =
                _database.GetRowCount("AlphabetLibrary") == 0;

            bool affixesAreEmpty =
                _database.GetRowCount("Affixes") == 0;

            if (alphabetIsEmpty || affixesAreEmpty)
            {
                var setupWindow = new InitialSetupWindow
                {
                    Owner = this
                };

                if (setupWindow.ShowDialog() != true)
                {
                    Close();
                    return;
                }

                _phonetic_system.GenerateAlphabet(
                    setupWindow.AlphabetCount, 0);

                for (int i = 0; i < setupWindow.AffixCount; i++)
                {
                    _affixSystem.GenerateAffix();
                }
            }

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
            LettersGrid.ItemsSource = _phonetic_system.ReadSymbolsToDelete()
;
        }


        private void LoadForms(int rootId)
        {
            var forms = _wordManager.ReturnWordFormView(rootId);

            MessageBox.Show(
                $"Root ID: {rootId}\n" +
                $"Liczba form: {forms.Count}");

            FormsGrid.ItemsSource = null;
            FormsGrid.ItemsSource = forms;
        }

        //private void LoadSecondForms(string text)
        //{
        //    var forms = _wordManager.ReturnWordFormViewText(text);

        //    MessageBox.Show(
        //        $"Tekst: [{text}]\n" +
        //        $"Liczba form: {forms.Count}");

        //    FormsGrid.ItemsSource = null;
        //    FormsGrid.ItemsSource = forms;
        //}

        private void LoadDictionary()
        {
            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                "SELECT * FROM Dictionary;",
                connection);

            using var reader = command.ExecuteReader();

            var table = new DataTable();
            table.Load(reader);

            WordsGrid.ItemsSource = table.DefaultView;
        }

        private void RootsGrid_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
                {
                    if (RootsGrid.SelectedItem is not DataRowView selected)
                        return;

                    RootTextBox.Text = selected["Symbol"]?.ToString() ?? "";
                    RootTranslateBox.Text = selected["Translation"]?.ToString() ?? "";
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
                MessageBox.Show("Wprowadź literę.");
                return;
            }

            try
            {
                _phonetic_system.AddSymbol(symbol, category);

                LetterTextBox.Clear();


                LoadAlphabet();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd");
            }


        }
        private void DeleteLetter_Click(object sender, RoutedEventArgs e)
        {
            if (LettersGrid.SelectedItem is not AlphabetLetter selected)
            {
                MessageBox.Show("Wybierz literę do usunięcia.");
                return;
            }

            try
            {
                _phonetic_system.DeleteLetters(selected.Id);
                LoadAlphabet();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd");
            }
        }

        private void AddForm_Click(
            object sender,
            RoutedEventArgs e)
                {
                    if (FormsGrid.SelectedItem is not WordFormView selected)
                    {
                        MessageBox.Show("Wybierz formę do zapisania.");
                        return;
                    }

                    string translation = FormTranslateBox.Text.Trim();

                    if (string.IsNullOrWhiteSpace(translation))
                    {
                        MessageBox.Show("Wprowadź tłumaczenie słowa.");
                        return;
                    }

                    try
                    {
                
                        int? rootId = null;

                        if (RootsGrid.SelectedItem is DataRowView root)
                            rootId = Convert.ToInt32(root["Id"]);

                        _wordManager.SaveWordToDictionary(
                            selected.Word,
                            translation,
                            rootId);

                        MessageBox.Show("Word saved to dictionary.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Błąd");
                    }
                }




        private void DeleteForm_Click(object sender, RoutedEventArgs e)
        {
            if (FormsGrid.SelectedItem is not WordFormView selected)
            {
                MessageBox.Show("Wybierz słowo.");
                return;
            }

            var forms = FormsGrid.ItemsSource as List<WordFormView>;

            if (forms == null)
                return;

            forms.Remove(selected);
            FormsGrid.Items.Refresh();
        }


        private void AddWord_Click(object sender, RoutedEventArgs e)
        {
            string word = WordTextBox.Text.Trim();
            string translation = WordTranslateBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(word) ||
                string.IsNullOrWhiteSpace(translation))
            {
                MessageBox.Show("Wprowadź słowo i tłumaczenie.");
                return;
            }

            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                @"INSERT INTO Dictionary (Word, Translation, RootId)
          VALUES (@Word, @Translation, @RootId);",
                connection);

            command.Parameters.AddWithValue("@Word", word);
            command.Parameters.AddWithValue("@Translation", translation);
            command.Parameters.AddWithValue("@RootId", DBNull.Value);

            command.ExecuteNonQuery();

            MessageBox.Show("Słowo zostało dodane.");
        }

        private void DeleteWord_Click(object sender, RoutedEventArgs e)
        {
            if (WordsGrid.SelectedItem is not DataRowView selected)
            {
                MessageBox.Show("Wybierz słowo do usunięcia.");
                return;
            }

            int id = Convert.ToInt32(selected["Id"]);

            var result = MessageBox.Show(
                "Czy na pewno chcesz usunąć to słowo?",
                "Potwierdzenie",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            using var connection = _database.GetConnection();
            connection.Open();

            using var command = new SqliteCommand(
                "DELETE FROM Dictionary WHERE Id = @Id;",
                connection);

            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();

            MessageBox.Show("Słowo zostało usunięte.");

            LoadDictionary();
        }

        private void GenerateFormsFromForm_Click(
        object sender, RoutedEventArgs e)
            {
                if (FormsGrid.SelectedItem is not WordFormView selected)
                {
                    MessageBox.Show("Wybierz słowo.");
                    return;
                }

                FormsGrid.ItemsSource = _wordManager.ReturnWordFormViewText(
                    selected.Word,
                    selected.GrammaticalRole,
                    selected.RootTranslation);
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

        private void UpdateRootTranslation_Click(
            object sender,
            RoutedEventArgs e)
                {
                    if (RootsGrid.SelectedItem is not DataRowView selected)
                    {
                        MessageBox.Show("Wybierz korzeń.");
                        return;
                    }

                    string translation = RootTranslateBox.Text.Trim();

                    if (string.IsNullOrWhiteSpace(translation))
                    {
                        MessageBox.Show("Wprowadź tłumaczenie.");
                        return;
                    }

                    int rootId = Convert.ToInt32(selected["Id"]);

                    try
                    {
                        _rootSystem.UpdateRootTranslation(rootId, translation);

                        LoadRoots();

                        MessageBox.Show("Translation updated.");
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