using System.Windows.Controls;

namespace Huzaifa_Mobile_Care.GUI.UserControls
{
    public partial class SimpleAmountBox : UserControl
    {
        public SimpleAmountBox()
        {
            InitializeComponent();
        }
        public string SimpleAmountBoxName
        {
            get { return SimpleABox_TBlock.Text.ToString(); }
            set 
            {
                SimpleABox_TBlock.Text = value;
                SimpleABox_NumericTBox.Tag = value;
            }
        }
    }
}
