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
    /// Interaction logic for SimpleAmountBox.xaml
    /// </summary>
    public partial class SimpleAmountBox : UserControl
    {
        public SimpleAmountBox()
        {
            InitializeComponent();
        }
        public string SimpleAmountBoxName
        {
            get { return BoxName.Text.ToString(); }
            set 
            {
                BoxName.Text = value;
                LOAD_MARGIN_TBOX.Tag = value;
            }
        }
    }
}
