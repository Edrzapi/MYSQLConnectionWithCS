using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DatabaseConnectionTutorial
{
    public class UserService
    {
        private readonly string dbName;

        public UserService(string dbName)
        {
            this.dbName = dbName;
        }

        public void Create(string firstName, string lastName)
        {
            using (var conn = DatabaseConfig.GetConnection(dbName))
            {
                string query = "INSERT INTO users (firstName, lastName) VALUES (@firstName, @lastName)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@firstName", firstName);
                cmd.Parameters.AddWithValue("@lastName", lastName);
                cmd.ExecuteNonQuery();
            }
        }

        public List<(int userId, string firstName, string lastName)> ReadAll()
        {
            var results = new List<(int, string, string)>();
            using (var conn = DatabaseConfig.GetConnection(dbName))
            {
                string query = "SELECT userId, firstName, lastName FROM users";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add((
                        reader.GetInt32("userId"),
                        reader.GetString("firstName"),
                        reader.GetString("lastName")
                    ));
                }
            }
            return results;
        }

        public void Update(int userId, string firstName, string lastName)
        {
            using (var conn = DatabaseConfig.GetConnection(dbName))
            {
                string query = "UPDATE users SET firstName = @firstName, lastName = @lastName WHERE userId = @userId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@firstName", firstName);
                cmd.Parameters.AddWithValue("@lastName", lastName);
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int userId)
        {
            using (var conn = DatabaseConfig.GetConnection(dbName))
            {
                string query = "DELETE FROM users WHERE userId = @userId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}

