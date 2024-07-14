﻿using System;
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
using System.Data.SqlClient;

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
        private int EnteredAmount { get; set; }
        private string FocusedTBoxName { get; set; }
        private InvoiceObject billItem { get; set; }

        private List<string> SimButtons { get; set; }

        // Connection for the Database
        private readonly string connectionString = "Server=DESKTOP-27KC7PD\\SQLEXPRESS;Database=UserManagementDB;Trusted_Connection=True;";

        public LoginWindow()
        {
            InitializeComponent();
            AddComboBoxItems();     // Adding Users into UserList_ComboBox
            ModifyHeader(Visibility.Collapsed, HorizontalAlignment.Left);   // Removing User options from Header
            SimButtons = new List<string>() { };
            getAllSims();

            //SimButtons = new List<string>()
            //{
            //    "Jazz", "Ufone", "Zong", "Telenor"
            //};

            foreach (string s in SimButtons)
            {
                Button b = new Button()
                {
                    Content = (SimButtons.IndexOf(s) + 1).ToString() + " " + s,
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
            if (InvoiceEntryButton.IsEnabled && e.Key == Key.Enter)
            {
                MessageBox.Show($"{billItem.Name}, {billItem.Price}, {billItem.Margin}, {billItem.Cost}");
            }

            if (IsAnyTextBoxFocused())
            {
                return;
            }

            if (CustomerPage.Visibility == Visibility.Visible)
            {
                if (keyToButtonTagMap.ContainsKey(e.Key))
                {
                    int tagToDisable = keyToButtonTagMap[e.Key];

                    Panel panel = (Panel)FindName(KeyboardFocusedPanelName);
                    DisableMenuButton(tagToDisable, panel);      // Disable the button with the corresponding tag
                }
                else if (e.Key == Key.OemPlus)
                {
                    var v = this.FindElementsWithNameContains("AMOUNT");

                    foreach (var element in v)
                    {
                        // Do something with the elements found
                        // For example:
                        if (element is TextBox textBox)
                        {
                            if (textBox.Text == "") textBox.Text = "0";
                            textBox.Text = (int.Parse(textBox.Text) + 5).ToString();
                        }
                    }


                }
                else if (e.Key == Key.OemMinus)
                {
                    var v = this.FindElementsWithNameContains("AMOUNT");

                    foreach (var element in v)
                    {
                        // Do something with the elements found
                        // For example:
                        if (element is TextBox textBox)
                        {
                            if (textBox.Text == "") textBox.Text = "0";
                            if (int.Parse(textBox.Text) < 5) return;
                            textBox.Text = (int.Parse(textBox.Text) - 5).ToString();
                        }
                    }
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

        public void getAllSims()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = "SELECT * FROM SIMs";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (reader.GetDecimal(2) > 99)
                {
                    SimButtons.Add(reader.GetString(1));
                }
            }
            connection.Close();
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
                billItem = new InvoiceObject();
                Invoice.Visibility = Visibility.Visible;
                LoginButton.IsEnabled = false;
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
                    billItem.Name = content.Substring(2);

                    if (int.TryParse(LOAD_AMOUNT_TBOX.Text, out int result) && result > 0)
                        InvoiceEntryButton.IsEnabled = true;
                    else
                        InvoiceEntryButton.IsEnabled = false;

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

                if (billItem != null)
                {
                    billItem.Name = null;
                    billItem.Price = 0;
                    billItem.Cost = 0;
                    billItem.Margin = 0;

                    var v = this.FindElementsWithNameContains("AMOUNT");

                    foreach (var element in v)
                    {
                        // Do something with the elements found
                        // For example:
                        if (element is TextBox textBox)
                        {
                            textBox.Text = "0";
                        }
                    }

                    var v1 = this.FindElementsWithNameContains("MARGIN");

                    foreach (var element in v1)
                    {
                        // Do something with the elements found
                        // For example:
                        if (element is TextBox textBox)
                        {
                            textBox.Text = "0";
                        }
                    }

                    var v2 = this.FindElementsWithNameContains("COST");

                    foreach (var element in v2)
                    {
                        // Do something with the elements found
                        // For example:
                        if (element is TextBox textBox)
                        {
                            textBox.Text = "0";
                        }
                    }
                }

                // Visibility Options
                Invoice.Visibility = Visibility.Collapsed;
                PurchasePage.Visibility = Visibility.Visible;
                panel1.Visibility = Visibility.Visible;

                // Updating Previous Menu Panel
                PreviousMenuPanel = panel1;
            }
            else return;
        }

        private void AmountButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            LOAD_AMOUNT_TBOX.Text = button.Content.ToString();
        }

        
        private void minusButton_Click(object sender, RoutedEventArgs e)
        {
            //if (loadAmountTextBox.Text == "") loadAmountTextBox.Text = "0";
            if (int.TryParse(LOAD_AMOUNT_TBOX.Text, out int load))
                if (load > 4)
                {
                    LOAD_AMOUNT_TBOX.Text = (load - 5).ToString();
                }
        }

        private void plusButton_Click(object sender, RoutedEventArgs e)
        {
            if (LOAD_AMOUNT_TBOX.Text == "") LOAD_AMOUNT_TBOX.Text = "0";
            LOAD_AMOUNT_TBOX.Text = (int.Parse(LOAD_AMOUNT_TBOX.Text) + 5).ToString();
        }

        private void NUMERIC_TBOX_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "" || billItem is null)
            {
                return;
            }
            if (tb.Name.Contains("AMOUNT"))
            {
                billItem.Price = int.Parse(tb.Text);
                if (billItem != null && billItem.Name != null && billItem.Price > 0)
                    InvoiceEntryButton.IsEnabled = true;
                else
                    InvoiceEntryButton.IsEnabled = false;
            }
            else if (tb.Name.Contains("COST"))
                billItem.Cost = int.Parse(tb.Text);
            else if (tb.Name.Contains("MARGIN"))
                billItem.Margin = int.Parse(tb.Text);
        }

        private void NUMERIC_TBOX_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void NUMERIC_TBOX_GotFocus(object sender, RoutedEventArgs e)
        {

            TextBox tb = sender as TextBox;
            FocusedTBoxName = tb.Name;
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

        private bool IsAnyTextBoxFocused()
        {
            // Check if any TextBox in the window is focused
            return this.FindVisualChildren<TextBox>().Any(tb => tb.IsFocused);
        }

        private void InvoiceEntryButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"{billItem.Name}, {billItem.Price}, {billItem.Margin}, {billItem.Cost}");
        }

        // Extension method to find all visual children of a specific type

    }
    public static class DependencyObjectExtensions
    {
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }

    public class InvoiceObject
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public int Cost { get; set; }
        public int Margin { get; set; }

        public InvoiceObject()
        {
            Name = null;
            Price = 0;
            Cost = 0;
            Margin = 0;
        }
    }

    public static class VisualTreeHelperExtensions
    {
        public static IEnumerable<FrameworkElement> FindElementsWithNameContains(this DependencyObject parent, string nameContains)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                if (child != null)
                {
                    if (child.Name.Contains(nameContains))
                        yield return child;

                    foreach (var foundChild in FindElementsWithNameContains(child, nameContains))
                        yield return foundChild;
                }
            }
        }
    }
}
