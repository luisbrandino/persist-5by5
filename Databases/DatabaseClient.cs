using Databases.Drivers;

namespace Databases
{
    public class DatabaseClient : IDatabaseDriver
    {
        protected IDatabaseDriver _driver;
        
        public DatabaseClient(IDatabaseDriver driver)
        {
            _driver = driver;
        }

        public T? Find<T>(int id) where T : Model, new()
        {
            return _driver.Find<T>(id);
        }

        public List<T> FindAll<T>() where T : Model, new()
        {
            return _driver.FindAll<T>();
        }

        public void Insert(Model model)
        {
            _driver.Insert(model);
        }

        public void InsertMany(List<Model> models)
        {
            _driver.InsertMany(models);
        }
    }
}
