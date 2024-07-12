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
using System.Collections.ObjectModel;

namespace Huzaifa_Mobile_Care
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Ctrl M O and Ctrl M P
    /// </summary>
    public partial class LoginWindow : Window
    {
        private Panel PreviousMenuPanel { get; set; }         // To collapse the visibility of the previous Menu Page (LOAD,CASH,BILL,SERVICES,ACCESSORY)
        private string KeyboardFocusedPanelName { get; set; }        // For switching of Numpad Shortcut Keys for New Page
        private string ActiveUser { get; set; }     // For getting the active user name
        private string SelectedButton { get; set; }

        private List<string> SimButtons { get; set; }

        // Connection for the Database
        private readonly string connectionString = "Server=DESKTOP-27KC7PD\\SQLEXPRESS;Database=UserManagementDB;Trusted_Connection=True;";

        public LoginWindow()
        {
            InitializeComponent();
            AddComboBoxItems();     // Adding Users into UserList_ComboBox
            ModifyHeader(Visibility.Collapsed, HorizontalAlignment.Left);   // Removing User options from Header

            SimButtons = new List<string>()
            {
                "Jazz", "Ufone", "Zong", "Telenor"
            };

            foreach(string s in SimButtons)
            {
                Button b = new Button()
                {
                    Content = (SimButtons.IndexOf(s)+1).ToString() + " " + s,
                    Style = (Style)FindResource("SimButton")
                };
                LOAD_BUTTONS.Children.Add(b);
            }
            
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
            if (CustomerPage.Visibility == Visibility.Visible)
            {
                if (keyToButtonTagMap.ContainsKey(e.Key))
                {
                    int tagToDisable = keyToButtonTagMap[e.Key];

                    Panel panel = (Panel)FindName(KeyboardFocusedPanelName);
                    DisableMenuButton(tagToDisable, panel);      // Disable the button with the corresponding tag
                }
                return;
            }

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
                // Setting Up Header
                ModifyHeader(Visibility.Visible, HorizontalAlignment.Center);

                // Visibility Options
                LoginPage.Visibility = Visibility.Collapsed;
                KeyboardFocusedPanelName = Menu.Name.ToString();
                CustomerPage.Visibility = Visibility.Visible;
                Invoice.Visibility = Visibility.Visible;
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

            // VisibilityOptions
            LoginPage.Visibility = Visibility.Visible;
            PreviousMenuPanel.Visibility = Visibility.Collapsed;
            PurchasePage.Visibility = Visibility.Collapsed;
            CustomerPage.Visibility = Visibility.Collapsed;

            Panel panel = (Panel)FindName("Menu");
            GetSerialNumbers(panel);
            Panel panel2 = (Panel)FindName(KeyboardFocusedPanelName);
            DisableAllChildrenButtons(panel2);
            

            PinBox.Clear();
            PinBox.IsEnabled = false;
            UserList_ComboBox.SelectedIndex = -1;
        }

        private readonly Dictionary<Key, int> keyToButtonTagMap = new Dictionary<Key, int>
        {
            { Key.NumPad1, 1 },
            { Key.D1, 1 },
            { Key.D2, 2 },
            { Key.NumPad2, 2 },
            { Key.D3, 3 },
            { Key.NumPad3, 3 },
            { Key.D4, 4 },
            { Key.NumPad4, 4 },
            { Key.D5, 5 },
            { Key.NumPad5,5 },
            { Key.D6, 6 },
            { Key.NumPad6, 6 },
            { Key.D7, 7},
            { Key.NumPad7, 7 },
            { Key.D8, 8 },
            { Key.NumPad8, 8},
            { Key.D9, 9 },
            { Key.NumPad9, 9 }
            // Add more mappings as needed
        };

        private Panel GetButtonParent(Button button)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(button);
            return parent as Panel;
        }

        private void DisableMenuButton(int Serial, Panel container)
        {
            if (Serial > container.Children.Count) return;
            foreach (Button btn in container.Children.OfType<Button>())
            {
                if (GetSerialNumber(btn.Content.ToString(), container) == Serial)
                {
                    string content = btn.Content?.ToString() ?? string.Empty;
                    SelectedButton = content.Substring(2);
                    EnableFunctionality(btn, container);
                    btn.IsEnabled = false;
                }
                else
                {
                    btn.IsEnabled = true;
                }
            }
        }

        private void SerialButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Panel panel = GetButtonParent(btn);

            if (KeyboardFocusedPanelName.Contains("_"))
            {
                Panel p = (Panel)FindName(KeyboardFocusedPanelName);
                DisableAllChildrenButtons(p);
            }

            KeyboardFocusedPanelName = panel.Name;
            DisableMenuButton(GetSerialNumber(btn.Content.ToString(), panel), panel);
            EnableFunctionality(btn, panel);
        }

        private void DisableAllChildrenButtons(Panel panel)
        {
            if (panel is null) return;
            foreach (Button button in panel.Children)
                button.IsEnabled = true;
        }

        private int GetSerialNumber(string content, Panel panel)
        {
            if (string.IsNullOrEmpty(content))
                return 0; // Handle empty or null strings

            // Iterate through the panel's children
            for (int i = 0; i < panel.Children.Count; i++)
            {
                if (panel.Children[i] is Button button && button.Content?.ToString() == content)
                {
                    return i + 1; // Return the 1-based index
                }
            }
            return 0; // Return 0 if no matching button is found
        }

        private void GetSerialNumbers(Panel panel)
        {
            foreach (Button button in panel.Children)
            {
                button.IsEnabled = true;
                button.Content = GetSerialNumber(button.Content.ToString(), Menu) + " " + button.Content.ToString();
            }
        }

        private void RemoveSerialNumbers(Panel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is Button button)
                {
                    string content = button.Content?.ToString() ?? string.Empty;
                    if (content.Length > 2 && int.TryParse(content[0].ToString(), out _))
                    {
                        button.Content = content.Substring(2);
                    }
                }
            }
        }

        private void EnableFunctionality(Button button, Panel panel)
        {
            if (panel.Name == "Menu")
            {
                RemoveSerialNumbers(panel);
                string name = button.Content.ToString();
                Panel panel1 = (Panel)FindName(name);

                // If Conditions
                if (PreviousMenuPanel != null) PreviousMenuPanel.Visibility = Visibility.Collapsed;
                if (panel1 is null) return;

                if (panel1.Name == "LOAD")
                    KeyboardFocusedPanelName = panel1.Name + "_BUTTONS";

                // Visibility Options
                Invoice.Visibility = Visibility.Collapsed;
                PurchasePage.Visibility = Visibility.Visible;
                panel1.Visibility = Visibility.Visible;
                
                // Updating Previous Menu Panel
                PreviousMenuPanel = panel1;
            }
            else return;
        }
    }
}
