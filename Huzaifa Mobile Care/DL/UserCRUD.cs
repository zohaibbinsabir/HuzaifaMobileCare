using Huzaifa_Mobile_Care.BL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Huzaifa_Mobile_Care.DL
{
    class UserCRUD
    {
        public static List<string> users = new List<string>();

        public static void getAllUsersFromDB(string dbConnectionString)
        {
            SqlConnection connection = new SqlConnection(dbConnectionString);
            connection.Open();
            string query = "SELECT * FROM Users";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string stateStr = reader.GetString(4); // Assuming State is stored as string in database
                User.UserState state;
                Enum.TryParse(stateStr, true, out state); // Use Enum.TryParse to convert string to enum
                if (state == User.UserState.active)
                {
                    users.Add(reader.GetString(1));
                }
            }
            connection.Close();
        }

        public static User SignIn(string name, string pin, string connectionString)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string searchQuery = String.Format("Select * from Users where Name = '{0}' and Pin = '{1}'", name, pin);
            SqlCommand command = new SqlCommand(searchQuery, connection);
            SqlDataReader data = command.ExecuteReader();
            if (data.Read())
            {
                User storedUser = GetUserFromDbData(data);
                connection.Close();
                return storedUser;
            }
            connection.Close();
            return null;
        }

        public static User GetUserFromDbData(SqlDataReader Data)
        {
            // Retrieve values from SqlDataReader
            string name = Data.GetString(1);
            string pin = Data.GetString(2);
            string roleStr = Data.GetString(3); // Assuming Role is stored as string in database
            string stateStr = Data.GetString(4); // Assuming State is stored as string in database

            // Convert string representations to enum values
            User.UserRole role;
            Enum.TryParse(roleStr, true, out role); // Use Enum.TryParse to convert string to enum

            User.UserState state;
            Enum.TryParse(stateStr, true, out state); // Use Enum.TryParse to convert string to enum

            // Create User object and add to list
            User user = new User(name, pin, role, state);
            return user;
        }
    }
}
