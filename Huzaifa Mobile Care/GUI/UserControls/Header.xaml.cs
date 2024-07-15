using System.Windows;
using System.Windows.Controls;

namespace Huzaifa_Mobile_Care.GUI.UserControls
{
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
            // Getting instance of the existing window page to perform operations
            LoginWindow MainWindow = Application.Current.MainWindow as LoginWindow;
            
            // Changing header to hide user options
            ModifyHeader(Visibility.Collapsed, HorizontalAlignment.Left);

            // Changing visibility to show Login Page only
            MainWindow.LoginPage.Visibility = Visibility.Visible;
            if (MainWindow.PreviousMenuPanel != null)
            {
                MainWindow.PreviousMenuPanel.Visibility = Visibility.Collapsed;
            }
            MainWindow.PurchasePage.Visibility = Visibility.Collapsed;
            MainWindow.CustomerPage.Visibility = Visibility.Collapsed;

            // Getting Serial Numbers for "Menu" Panel
            MainWindow.GetSerialNumbers((Panel)MainWindow.FindName("Menu"));
            MainWindow.ResetPanelButtons(MainWindow.KeyboardFocusedPanel);

            // Resetting Current Bill Item and Menu Page Text Boxes
            MainWindow.ResetBillItem();
        }

        public void ModifyHeader(Visibility UserHeaderButtonsVisibility, HorizontalAlignment BrandNameHorizontalAlignment)
        {
            HeaderTextButtons_USER.Visibility = UserHeaderButtonsVisibility;
            HeaderIconButtons_USER.Visibility = UserHeaderButtonsVisibility;
            BrandName.HorizontalAlignment = BrandNameHorizontalAlignment;
        }
    }
}
