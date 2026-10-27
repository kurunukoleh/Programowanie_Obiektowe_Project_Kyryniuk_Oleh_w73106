
using System.Windows;

namespace OOP_project
{
    public partial class InitialSetupWindow : Window
    {
        public int AlphabetCount { get; private set; }
        public int AffixCount { get; private set; }

        public InitialSetupWindow()
        {
            InitializeComponent();
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(AlphabetCountBox.Text, out int alphabetCount) ||
                !int.TryParse(AffixCountBox.Text, out int affixCount) ||
                alphabetCount < 1 || affixCount < 1)
            {
                MessageBox.Show(
                    "Wprowadź poprawne liczby większe od zera.");
                return;
            }

            AlphabetCount = alphabetCount;
            AffixCount = affixCount;

            DialogResult = true;
        }
    }
}
