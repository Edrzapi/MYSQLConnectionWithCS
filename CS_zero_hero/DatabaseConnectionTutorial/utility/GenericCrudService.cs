using System.Reflection;
using System.Xml.Linq;
using DatabaseConnectionTutorial.Utility;
using MySql.Data.MySqlClient;

namespace DatabaseConnectionTutorial.Services
{
    public class GenericCrudService<T> : IService<T>
        where T : class, new()
    {
        private readonly IConnectionFactory _factory;
        private readonly string _tableName;
        private readonly PropertyInfo _idProp;
        private readonly List<PropertyInfo> _props;  // all non-ID props
        public GenericCrudService(IConnectionFactory factory)
        {
            _factory = factory;

            var type = typeof(T);
            var name = type.Name;   // ← get the class name (“User”)

            // auto-pluralize ("User" → "Users", "Classs" will stay "Classs" so you may want
            // to tighten this if you have weird plurals in your table names..
            // This is just the convention I tend to use. By all means remove it and
            // keep you POCOs simple.
            _tableName = name.EndsWith("s", StringComparison.OrdinalIgnoreCase)
                ? name
                : name + "s"; 

            _idProp = type.GetProperty("Id")
                ?? throw new InvalidOperationException(
                       $"{type.Name} must have an int Id property");

            _props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                         .Where(p => p != _idProp)
                         .ToList();
        }


        private MySqlConnection OpenConn() => _factory.CreateConnection();

        public void Create(T item)
        {
            var cols = string.Join(", ", _props.Select(p => p.Name));
            var pars = string.Join(", ", _props.Select(p => "@" + p.Name));
            var sql = $"INSERT INTO `{_tableName}` ({cols}) VALUES ({pars});";

            using var conn = OpenConn();
            using var cmd = new MySqlCommand(sql, conn);

            foreach (var p in _props)
                cmd.Parameters.AddWithValue("@" + p.Name,
                                            p.GetValue(item) ?? DBNull.Value);

            cmd.ExecuteNonQuery();

            // assign the new auto-incremented Id back onto the item
            var newId = Convert.ToInt32(cmd.LastInsertedId);
            _idProp.SetValue(item, newId);
        }

        public T Read(int id)
        {
            var sql = $"SELECT * FROM `{_tableName}` WHERE {_idProp.Name} = @id LIMIT 1;";
            using var conn = OpenConn();
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var rdr = cmd.ExecuteReader();
            if (!rdr.Read()) return null;
            return Map(rdr);
        }

        public IEnumerable<T> Read()
        {
            var list = new List<T>();
            var sql = $"SELECT * FROM `{_tableName}`;";
            using var conn = OpenConn();
            using var cmd = new MySqlCommand(sql, conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
                list.Add(Map(rdr));
            return list;
        }

        public void Update(T item)
        {
            var setClause = string.Join(", ",
                _props.Select(p => $"{p.Name} = @{p.Name}"));
            var sql = $"UPDATE `{_tableName}` SET {setClause} WHERE {_idProp.Name} = @id;";

            using var conn = OpenConn();
            using var cmd = new MySqlCommand(sql, conn);

            foreach (var p in _props)
                cmd.Parameters.AddWithValue("@" + p.Name,
                                            p.GetValue(item) ?? DBNull.Value);

            var idVal = _idProp.GetValue(item);
            cmd.Parameters.AddWithValue("@id", idVal);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            var sql = $"DELETE FROM `{_tableName}` WHERE {_idProp.Name} = @id;";
            using var conn = OpenConn();
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        private T Map(MySqlDataReader rdr)
        {
            var obj = new T();
            foreach (var p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var val = rdr[p.Name];
                if (val is DBNull) continue;
                p.SetValue(obj, Convert.ChangeType(val, p.PropertyType));
            }
            return obj;
        }
    }
}
