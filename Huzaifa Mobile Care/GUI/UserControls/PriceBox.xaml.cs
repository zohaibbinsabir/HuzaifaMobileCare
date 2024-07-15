using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Huzaifa_Mobile_Care.GUI.UserControls
{
    /// <summary>
    /// Interaction logic for PriceBox.xaml
    /// </summary>
    public partial class PriceBox : UserControl
    {
        public PriceBox()
        {
            InitializeComponent();
        }
        private void minusButton_Click(object sender, RoutedEventArgs e)
        {
            //if (loadAmountTextBox.Text == "") loadAmountTextBox.Text = "0";
            if (int.TryParse(LOAD_AMOUNT_TBOX.NumericTBOX.Text, out int load))
                if (load > 4)
                {
                    LOAD_AMOUNT_TBOX.NumericTBOX.Text = (load - 5).ToString();
                }
        }

        private void plusButton_Click(object sender, RoutedEventArgs e)
        {
            if (LOAD_AMOUNT_TBOX.NumericTBOX.Text == "") LOAD_AMOUNT_TBOX.NumericTBOX.Text = "0";
            LOAD_AMOUNT_TBOX.NumericTBOX.Text = (int.Parse(LOAD_AMOUNT_TBOX.NumericTBOX.Text) + 5).ToString();
        }
    }
}
