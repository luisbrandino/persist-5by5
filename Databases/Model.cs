using System.Reflection;

namespace Databases
{
    public class TableAttribute : Attribute
    {
        public string TableName { get; set; }

        public TableAttribute(string tableName) 
        {
            TableName = tableName;
        }
    }

    public class CollectionAttribute : Attribute
    {
        public string CollectionName { get; set; }

        public CollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
    
    /**
     *  Para esse projeto, estou assumindo que todas primary keys são auto increment
     */
    public class PrimaryKeyAttribute : Attribute { }

    public class ColumnAttribute : Attribute
    {
        public string ColumnName { get; set; }

        public ColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }

    public abstract class Model
    {
        public string Table()
        {
            string? tableName = this.GetType().GetCustomAttribute<TableAttribute>()?.TableName;

            if (tableName == null)
                throw new InvalidOperationException($"Classe concreta de '{typeof(Model)}' precisa ter 'TableAttribute'");

            return tableName;
        }

        public string Collection()
        {
            string? collectionName = this.GetType().GetCustomAttribute<CollectionAttribute>()?.CollectionName;

            if (collectionName == null)
                throw new InvalidOperationException($"Classe concreta de '{typeof(Model)}' precisa ter 'CollectionAttribute'");

            return collectionName;
        }

        public bool IsPrimaryKey(PropertyInfo property) => Attribute.IsDefined(property, typeof(PrimaryKeyAttribute));

        public static string GetPrimaryKeyName<T>() where T : Model
        {
            string defaultPrimaryKeyName = "id";

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            foreach (var property in properties)
            {
                var primaryKeyAttribute = property.GetCustomAttribute<PrimaryKeyAttribute>();

                if (primaryKeyAttribute != null)
                    return property.GetCustomAttribute<ColumnAttribute>()?.ColumnName ?? property.Name;
            }

            return defaultPrimaryKeyName;
        }
    }
}
