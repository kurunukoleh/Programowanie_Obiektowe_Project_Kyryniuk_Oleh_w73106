using Microsoft.Data.Sqlite;
using OOP_project;
using OOP_project.Data;
using OOP_project.Models;
using OOP_project.Systems;
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

        private readonly DictionarySystem _dictionary_system;

        public LibraryWindow(Database database, Window? owner = null)
        {
            InitializeComponent();

            _database = database;
            _rootSystem = new RootSystem(_database);
            _affixSystem = new AffixSystem(_database);
            _phonetic_system = new PhoneticSystem(_database);
            _wordManager = new WordManager(
                _database, _affixSystem, _rootSystem);
            _dictionary_system = new DictionarySystem(_database);

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
            LoadDictionary();
        }



       

        private void LoadRoots()
        {
            RootsGrid.ItemsSource = _rootSystem.readtable();
        }

        private void LoadAffixes()
        {
            AffixesGrid.ItemsSource = _affixSystem.readtable();
        }

        private void LoadAlphabet()
        {
            LettersGrid.ItemsSource = _phonetic_system.readtable();
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

        

        private void LoadDictionary()
        {
            

            WordsGrid.ItemsSource = _dictionary_system.readtable();
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
                _rootSystem.insert(new RootModel(0, root, translation));

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
            if (RootsGrid.SelectedItem is not RootModel selectedRoot)
            {
                MessageBox.Show("Wybierz korzeń do usunięcia.");
                return;
            }

            _rootSystem.delete(selectedRoot.Id);

            LoadRoots();
        }



        private void AddAffix_Click(object sender, RoutedEventArgs e)
        {
            string affix = AffixTextBox.Text.Trim();
            string partOfSpeechId = PartOfSpeechIdBox.Text.Trim();
            string grammaticalRoleCode = GrammaticalRoleCodeBox.Text.Trim();
            string affixTypeId = AffixTypeIdBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(affix) ||
                string.IsNullOrWhiteSpace(partOfSpeechId) ||
                string.IsNullOrWhiteSpace(grammaticalRoleCode) ||
                string.IsNullOrWhiteSpace(affixTypeId))
            {
                MessageBox.Show("Wypełnij wszystkie pola.");
                return;
            }

            if (!int.TryParse(partOfSpeechId, out int posId) ||
                !int.TryParse(affixTypeId, out int typeId))
            {
                MessageBox.Show("Podaj prawidłowe identyfikatory.");
                return;
            }

            try
            {
                var newAffix = new AffixModel(
                    0,
                    affix,
                    posId,
                    grammaticalRoleCode,
                    typeId,
                    "" // Nazwa typu zostanie pobrana podczas odczytu z bazy
                );

                _affixSystem.insert(newAffix);

                AffixTextBox.Clear();
                PartOfSpeechIdBox.Clear();
                GrammaticalRoleCodeBox.Clear();
                AffixTypeIdBox.Clear();

                LoadAffixes();

                MessageBox.Show("Afiks został dodany.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd");
            }
        }


        private void DeleteAffix_Click(object sender, RoutedEventArgs e)
        {
            if (AffixesGrid.SelectedItem is not AffixModel selectedAffix)
            {
                MessageBox.Show("Wybierz afiks do usunięcia.");
                return;
            }

            _affixSystem.delete(selectedAffix.Id);

            LoadAffixes();
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
                _phonetic_system.insert(new AlphabetLetter(0, symbol, category));

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
                _phonetic_system.delete(selected.Id);
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
                    if (FormsGrid.SelectedItem is not WordModel selected)
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

                    LoadDictionary();
        }




        private void DeleteForm_Click(object sender, RoutedEventArgs e)
        {
            if (FormsGrid.SelectedItem is not WordModel selected)
            {
                MessageBox.Show("Wybierz słowo.");
                return;
            }

            var forms = FormsGrid.ItemsSource as List<WordModel>;

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

            var newWord = new WordDictionaryModel(
                0,
                word,
                translation,
                null
            );

            _dictionary_system.insert(newWord);

            MessageBox.Show("Słowo zostało dodane.");

            LoadDictionary();
        }

        private void DeleteWord_Click(object sender, RoutedEventArgs e)
        {
            if (WordsGrid.SelectedItem is not WordDictionaryModel selected)
            {
                MessageBox.Show("Wybierz słowo do usunięcia.");
                return;
            }

            int id = selected.Id;

            var result = MessageBox.Show(
                "Czy na pewno chcesz usunąć to słowo?",
                "Potwierdzenie",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            _dictionary_system.delete(id);

            MessageBox.Show("Słowo zostało usunięte.");

            LoadDictionary();
        }

        private void GenerateFormsFromForm_Click(
        object sender, RoutedEventArgs e)
            {
                if (FormsGrid.SelectedItem is not WordModel selected)
                {
                    MessageBox.Show("Wybierz słowo.");
                    return;
                }

                FormsGrid.ItemsSource = _wordManager.ReturnWordFormView(
                    selected.Word,
                    selected.GrammaticalRole,
                    selected.RootTranslation);
            }

        private void GenerateForms_Click(object sender, RoutedEventArgs e)
        {
            if (RootsGrid.SelectedItem is not RootModel selected)
            {
                MessageBox.Show("Wybierz korzeń.");
                return;
            }

            int rootId = selected.Id;

            try
            {
                LoadForms(rootId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Błąd");
            }
        }

        private void UpdateRootTranslation_Click(object sender, RoutedEventArgs e)
        {
            if (RootsGrid.SelectedItem is not RootModel selected)
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

            try
            {
                _rootSystem.update(
                    new RootModel(selected.Id, selected.Symbol, translation));

                LoadRoots();

                MessageBox.Show("Translation updated.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Błąd");
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