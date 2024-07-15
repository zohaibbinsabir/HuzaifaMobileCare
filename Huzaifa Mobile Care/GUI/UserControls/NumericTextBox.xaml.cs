using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Huzaifa_Mobile_Care.GUI.UserControls
{
    /// <summary>
    /// Interaction logic for NumericTextBox.xaml
    /// </summary>
    public partial class NumericTextBox : UserControl
    {
        LoginWindow MainWindow;
        public NumericTextBox()
        {
            // Getting Instance of the Existing Window Page
            MainWindow = Application.Current.MainWindow as LoginWindow;
            InitializeComponent();
            NumericTBOX.Text = "0";
        }

        public string Text
        {
            get { return NumericTBOX.Text; }
            set { NumericTBOX.Text = value; }
        }

        private void NUMERIC_TBOX_TextChanged(object sender, TextChangedEventArgs e)
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
                    break;
                case "Margin":
                    MainWindow.billItem.Margin = int.Parse(textBox.Text);
                    break;
                default:
                    // Handle default case or error
                    break;
            }




            //TextBox tb = sender as TextBox;
            //if (tb.Text == "" || MainWindow.billItem is null)
            //{
            //    return;
            //}
            //if (tb.Tag.ToString().Contains("AMOUNT"))
            //{
            //    MainWindow.billItem.Price = int.Parse(tb.Text);
            //    if (MainWindow.billItem != null && MainWindow.billItem.Name != null && MainWindow.billItem.Price > 0)
            //        MainWindow.InvoiceEntryButton.IsEnabled = true;
            //    else
            //        MainWindow.InvoiceEntryButton.IsEnabled = false;
            //}
            //else if (tb.Name.Contains("COST"))
            //    MainWindow.billItem.Cost = int.Parse(tb.Text);
            //else if (tb.Name.Contains("MARGIN"))
            //    MainWindow.billItem.Margin = int.Parse(tb.Text);
        }

        private void NUMERIC_TBOX_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void NUMERIC_TBOX_GotFocus(object sender, RoutedEventArgs e)
        {

            TextBox tb = sender as TextBox;
            MainWindow.FocusedTBoxName = tb.Name;
            // Clear the "0" value when the TextBox gets focus
            if (tb.Text == "0")
            {
                tb.Text = string.Empty;
            }
        }

        private void NUMERIC_TBOX_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Regular expression to allow only numeric input
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NUMERIC_TBOX_LostFocus(object sender, RoutedEventArgs e)
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
