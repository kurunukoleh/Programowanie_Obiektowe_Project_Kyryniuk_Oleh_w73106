using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Programowanie_Obiektowe_Project_Kyryniuk_Oleh_w73106
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 



    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateWord_Click(
        object sender,
        RoutedEventArgs e)
            {
                if (AffixCountComboBox.SelectedItem is not ComboBoxItem item)
                    return;

                int affixCount = int.Parse(item.Content.ToString()!);

                // Тут викликатиметься твій генератор.
                GeneratedWordText.Text = "Nowe słowo";
                WordDetailsText.Text =
                    $"Liczba afiksów: {affixCount}";
            }
    }
}