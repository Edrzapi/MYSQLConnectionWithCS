using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DatabaseConnectionTutorial
{
    public static class DatabaseConfig
    {
        public static MySqlConnection GetConnection(string databaseName)
        {
            string connStr = $"Server=localhost;Port=3306;Database={databaseName};UserID=root;Password=root;";

            var connection = new MySqlConnection(connStr);
            connection.Open();
            return connection;
        }

    }
    }