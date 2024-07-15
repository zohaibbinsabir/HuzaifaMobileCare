using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Huzaifa_Mobile_Care.GUI.UserControls
{
    public partial class NumericTextBox : UserControl
    {
        LoginWindow MainWindow;
        public NumericTextBox()
        {
            // Getting Instance of the Existing Window Page
            MainWindow = Application.Current.MainWindow as LoginWindow;
            InitializeComponent();
            NumericTBox.Text = "0";
        }

        public string Text
        {
            get { return NumericTBox.Text; }
            set { NumericTBox.Text = value; }
        }

        private void NumericTBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null || textBox.Text == "") return;

            var tag = textBox.Tag?.ToString();

            switch (tag)
            {
                case "Amount":
                    MainWindow.billItem.Price = int.Parse(textBox.Text);
                    if (MainWindow.billItem != null && MainWindow.billItem.Name != null && MainWindow.billItem.Price > 0)
                        MainWindow.InvoiceEntryButton.IsEnabled = true;
                    else
                        MainWindow.InvoiceEntryButton.IsEnabled = false;
                    break;
                case "Cost":
                    MainWindow.billItem.Cost = int.Parse(textBox.Text);
                    MainWindow.billItem.Margin = int.Parse(MainWindow.billItem.Price.ToString()) - int.Parse(MainWindow.billItem.Cost.ToString());
                    MainWindow.SetMarginValue(MainWindow.billItem.Margin.ToString());
                    break;
                case "Margin":
                    MainWindow.billItem.Margin = int.Parse(textBox.Text);
                    break;
                default:
                    // Handle default case or error
                    break;
            }
        }

        private void NumericTBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void NumericTBox_GotFocus(object sender, RoutedEventArgs e)
        {

            TextBox tb = sender as TextBox;
            MainWindow.FocusedTBoxName = tb.Name;
            // Clear the "0" value when the TextBox gets focus
            if (tb.Text == "0")
            {
                tb.Text = string.Empty;
            }
        }

        private void NumericTBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Regular expression to allow only numeric input
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NumericTBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            // Restore the "0" value if the TextBox is empty when it loses focus
            if (string.IsNullOrEmpty(tb.Text))
            {
                tb.Text = "0";
            }
        }
    }
}
