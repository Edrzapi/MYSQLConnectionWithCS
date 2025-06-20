using MySql.Data.MySqlClient;

namespace DatabaseConnectionTutorial.Utility
{
    // Define the factory interface
    public interface IConnectionFactory
    {
        MySqlConnection CreateConnection();
    }

    // Implement 
    public class DatabaseConfig : IConnectionFactory
    {
        private readonly string _connectionString;

        public DatabaseConfig(string databaseName)
        {
            _connectionString =
                $"Server=localhost;Port=3306;Database={databaseName};UserID=root;Password=root;";
        }
        // Same as before
        public MySqlConnection CreateConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
