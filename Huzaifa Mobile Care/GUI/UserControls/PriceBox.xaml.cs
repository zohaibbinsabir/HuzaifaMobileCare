using System.Windows;
using System.Windows.Controls;

namespace Huzaifa_Mobile_Care.GUI.UserControls
{
    public partial class PriceBox : UserControl
    {
        public PriceBox()
        {
            InitializeComponent();
        }
        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(PBox_TBox.NumericTBox.Text, out int load))
                if (load > 4)
                {
                    PBox_TBox.NumericTBox.Text = (load - 5).ToString();
                }
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (PBox_TBox.Text == "") PBox_TBox.Text = "0";
            PBox_TBox.Text = (int.Parse(PBox_TBox.Text) + 5).ToString();
        }
    }
}
