using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace Databases.Drivers
{
    /**
     * Wrapper do SqlClient
     */
    internal class SqlServerCommand
    {
        private string _connectionString;

        public SqlServerCommand(string databaseName, string? user = null, string? password = null)
        {
            _connectionString = @$"Server=127.0.0.1;Database={databaseName};TrustServerCertificate=True;";

            if (user != null && password != null)
                _connectionString += $"User Id={user};Password={password}";
        }

        public SqlConnection CreateNewConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);

            connection.Open();

            return connection;
        }

        public SqlCommand? Command(string query, SqlConnection connection, Dictionary<string, object>? data = null)
        {
            SqlCommand command = new SqlCommand(query, connection);

            if (data != null)
                foreach (var entry in data)
                    command.Parameters.AddWithValue(entry.Key, entry.Value);

            return command;
        }

        public DataRowCollection DataTable(string query, Dictionary<string, object>? data = null)
        {
            using (SqlConnection connection = CreateNewConnection())
                return CommandDataTable(Command(query, connection, data));
        }

        public DataRowCollection? CommandDataTable(SqlCommand command)
        {
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                DataTable result = new();

                adapter.Fill(result);

                if (result.Rows.Count <= 0)
                    return null;

                return result.Rows;
            }
        }

        public int NonQuery(string query, Dictionary<string, object>? data = null)
        {
            using (SqlConnection connection = CreateNewConnection())
                return CommandNonQuery(Command(query, connection, data));
        }

        public int CommandNonQuery(SqlCommand command)
        {
            return command.ExecuteNonQuery();
        }

        public T? CommandScalar<T>(SqlCommand command)
        {
            using (SqlConnection connection = CreateNewConnection())
            {
                object result = command.ExecuteScalar();

                if (result == null)
                    return default(T);

                return (T)result;
            }
        }

        public SqlCommand? Procedure(string name, SqlConnection connection, Dictionary<string, object> data)
        {
            SqlCommand? command = Command(name, connection, data);

            if (command == null)
                return null;

            command.CommandType = CommandType.StoredProcedure;

            return command;
        }

        public void ProcedureNonQuery(string name, Dictionary<string, object> data)
        {
            using (SqlConnection connection = CreateNewConnection())
            using (SqlCommand? procedure = Procedure(name, connection, data))
                CommandNonQuery(procedure);
        }

        public T? ProcedureScalar<T>(string name, Dictionary<string, object> data)
        {
            using (SqlConnection connection = CreateNewConnection())
            using (SqlCommand? procedure = Procedure(name, connection, data))
                return CommandScalar<T>(procedure);
        }

        public DataRowCollection? ProcedureDataTable(string name, Dictionary<string, object> data)
        {
            using (SqlConnection connection = CreateNewConnection())
            using (SqlCommand? procedure = Procedure(name, connection, data))
                return CommandDataTable(procedure);
        }
    }

    public class SqlServerDriver : IDatabaseDriver
    {
        private SqlServerCommand _command;

        public SqlServerDriver(string databaseName, string? user = null, string? password = null)
        {
            _command = new SqlServerCommand(databaseName, user, password);
        }

        public PropertyInfo? GetPropertyWithColumnAttribute<T>(string column)
        {
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<ColumnAttribute>();

                if (attribute != null && attribute.ColumnName == column)
                    return property;
            }

            return null;
        }

        public List<T> Convert<T>(DataRowCollection rows, List<string> columns) where T : Model, new()
        {
            List<T> models = new();

            foreach (DataRow item in rows)
            {
                T model = new();

                foreach (string column in columns)
                {
                    PropertyInfo? property = GetPropertyWithColumnAttribute<T>(column);
                    property?.SetValue(model, item[column]);
                }

                models.Add(model);
            }

            return models;
        }

        public T? Find<T>(int id) where T : Model, new()
        {
            List<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<string> columns = properties.Select(property => property.GetCustomAttribute<ColumnAttribute>()?.ColumnName ?? property.Name).ToList();

            string names = string.Join(", ", columns);
            string? tableName = typeof(T).GetCustomAttribute<TableAttribute>()?.TableName;

            if (tableName == null)
                return null;

            string primaryKeyName = Model.GetPrimaryKeyName<T>();

            string query = $"select {names} from {tableName} where {primaryKeyName} = {id}";

            DataRowCollection rows = _command.DataTable(query);

            return Convert<T>(rows, columns).First();
        }

        public List<T> FindAll<T>() where T : Model, new()
        {
            List<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<string> columns = properties.Select(property => property.GetCustomAttribute<ColumnAttribute>()?.ColumnName ?? property.Name).ToList();

            string names = string.Join(", ", columns);
            string? tableName = typeof(T).GetCustomAttribute<TableAttribute>()?.TableName;

            if (tableName == null)
                return new List<T>();

            string query = $"select {names} from {tableName}";

            DataRowCollection rows = _command.DataTable(query);

            return Convert<T>(rows, columns);
        }

        public void Insert(Model model)
        {
            List<PropertyInfo> properties = model.GetType().GetProperties().ToList().Where(property => !model.IsPrimaryKey(property)).ToList();
            List<string> columns = properties.Select(property => property.GetCustomAttribute<ColumnAttribute>()?.ColumnName ?? property.Name).ToList();

            string names = string.Join(", ", columns);
            string values = string.Join(", ", properties.Select(property => $"'{property.GetValue(model)}'"));

            string query = $"insert into {model.Table()} ({names}) values ({values})";

            _command.NonQuery(query);
        }

        public void InsertMany(List<Model> models)
        {
            if (models.Count == 0)
                return;

            List<PropertyInfo> properties = models.First().GetType().GetProperties().ToList().Where(property => !models.First().IsPrimaryKey(property)).ToList();
            string names = string.Join(", ", properties.Select(property => property.GetCustomAttribute<ColumnAttribute>()?.ColumnName ?? property.Name));
            string values = "";

            foreach (Model model in models)
                values += $"({string.Join(", ", properties.Select(property => $"'{property.GetValue(model)}'"))}),";

            values = values.TrimEnd(',');

            string query = $"insert into {models.First().Table()} ({names}) values {values}";

            _command.NonQuery(query);

        }
    }
}
