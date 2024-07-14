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
    /// Interaction logic for Header.xaml
    /// </summary>
    public partial class Header : UserControl
    {
        public Header()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Getting Instance of the Existing Window Page
            LoginWindow MainWindow = Application.Current.MainWindow as LoginWindow;
            
            // Changing Header to Hide User Options
            ModifyHeader(Visibility.Collapsed, HorizontalAlignment.Left);

            // Showing Login Page
            MainWindow.LoginPage.Visibility = Visibility.Visible;

            // Hiding All pages
            if (MainWindow.PreviousMenuPanel != null)
            {
                MainWindow.PreviousMenuPanel.Visibility = Visibility.Collapsed;
            }
            MainWindow.PurchasePage.Visibility = Visibility.Collapsed;
            MainWindow.CustomerPage.Visibility = Visibility.Collapsed;

            // Getting Serial Numbers for "Menu" Panel
            MainWindow.GetSerialNumbers((Panel)MainWindow.FindName("Menu"));
            MainWindow.ResetPanelButtons(MainWindow.KeyboardFocusedPanel);

            // Resetting Current Bill Item and Page
            MainWindow.ResetBillItem();
        }

        public void ModifyHeader(Visibility UserHeaderButtonsVisibility, HorizontalAlignment BrandNameHorizontalAlignment)
        {
            HeaderUserTextButtons.Visibility = UserHeaderButtonsVisibility;
            HeaderUserIconButtons.Visibility = UserHeaderButtonsVisibility;
            BrandName.HorizontalAlignment = BrandNameHorizontalAlignment;
        }
    }
}
