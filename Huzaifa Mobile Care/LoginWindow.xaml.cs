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
using Huzaifa_Mobile_Care.DL;
using Huzaifa_Mobile_Care.BL;
using System.Text.RegularExpressions;

namespace Huzaifa_Mobile_Care
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static string ActiveUser { get; set; }
        string connectionString = "Server=DESKTOP-27KC7PD\\SQLEXPRESS;Database=UserManagementDB;Trusted_Connection=True;";



        public LoginWindow()
        {
            InitializeComponent();
            AddComboBoxItems(); // Adding Users into UserList_ComboBox
            ModifyHeader(Visibility.Collapsed, HorizontalAlignment.Left); // Removing User options from Header
        }

        /* Header Function */
        private void ModifyHeader(Visibility UserHeaderButtonsVisibility, HorizontalAlignment BrandNameHorizontalAlignment)
        {
            HeaderUserTextButtons.Visibility = UserHeaderButtonsVisibility;
            HeaderUserIconButtons.Visibility = UserHeaderButtonsVisibility;
            BrandName.HorizontalAlignment = BrandNameHorizontalAlignment;
        }


        /* Global Shortcuts */
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // UserList_ComboBox Shortcut
            if (e.Key == Key.Down)
            {
                if (!UserList_ComboBox.IsDropDownOpen)
                {
                    e.Handled = true;   // it is used to over-write any default shortcut
                    UserList_ComboBox.IsDropDownOpen = true;
                    UserList_ComboBox.Focus();  // it will now have keyboard focus, all default keyboard shortcuts of combobox will be active

                    // if nothing is selected then by opening list, first item will be focused by default.
                    if (UserList_ComboBox.SelectedItem == null)
                    {
                        ComboBoxItem firstItem = (ComboBoxItem)UserList_ComboBox.ItemContainerGenerator.ContainerFromIndex(0);
                        firstItem.Focus();
                    }
                }
            }

            // LoginButton Shortcut
            else if (e.Key == Key.Enter && LoginButton.IsEnabled && !UserList_ComboBox.IsDropDownOpen)
            {
                LoginButton_Click(LoginButton, null);
            }
        }


        /* User List Combo Box */
        private void UserList_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserList_ComboBox.SelectedItem != null)
            {
                PinBox.IsEnabled = true;
                PinBox.Focus();
            }
        }

        private void UserList_ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // if UserList_ComboBox is in closed state, then down arrow key will open it. (Shortcut)
            if (e.Key == Key.Down)
            {
                if (!UserList_ComboBox.IsDropDownOpen)
                {
                    e.Handled = true;
                    UserList_ComboBox.IsDropDownOpen = true;
                }
            }
        }

        private void AddComboBoxItems()
        {
            UserCRUD.getAllUsersFromDB(connectionString);

            foreach (string user in UserCRUD.users)
            {
                // Check if the user already exists in the ComboBox
                if (!UserList_ComboBox.Items.Contains(user))
                {
                    UserList_ComboBox.Items.Add(user);
                }
            }

        }


        /* Pin Box */
        private void PinBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            LoginButton.IsEnabled = PinBox.Password.Length == 4;
        }

        private void PinBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var passwordBox = sender as PasswordBox;

            // Check if input is numeric and the password length is less than 4
            if (!IsNumeric(e.Text) || passwordBox.Password.Length >= 4)
            {
                e.Handled = true; // Cancel the input if not numeric or exceeds length
            }
        }

        private void PinBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Allow only Backspace and Delete
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                e.Handled = false; // Allow backspace and delete
            }
            else if (!IsNumeric(e.Key.ToString()) && !e.Key.ToString().StartsWith("D") && !e.Key.ToString().StartsWith("NumPad"))
            {
                e.Handled = true; // Disallow any other key inputs
            }
        }

        private bool IsNumeric(string text)
        {
            // Helper method to check if the text is numeric
            return Regex.IsMatch(text, "^[0-9]+$");
        }


        /* Login Button */
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ActiveUser = UserList_ComboBox.SelectedItem.ToString();
            User user = UserCRUD.SignIn(UserList_ComboBox.SelectedItem.ToString(), PinBox.Password, connectionString);

            if (user != null)
            {
                MessageBox.Show("Logged In");

                // Setting Up Header
                ModifyHeader(Visibility.Visible, HorizontalAlignment.Center);

                // Collapsing LoginPage
                LoginPage.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Wrong Password");
            }
        }


        /* Application Buttons */
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Setting Up Header
            ModifyHeader(Visibility.Collapsed, HorizontalAlignment.Left);

            // Collapsing LoginPage
            LoginPage.Visibility = Visibility.Visible;

            PinBox.Clear();
            PinBox.IsEnabled = false;
            UserList_ComboBox.SelectedIndex = -1;
        }
    }
}
