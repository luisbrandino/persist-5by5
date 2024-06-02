namespace Databases.Drivers
{
    public interface IDatabaseDriver
    {
        void Insert(Model model);

        void InsertMany(List<Model> models);

        T? Find<T>(int id) where T : Model, new();

        List<T> FindAll<T>() where T : Model, new();

    }
}
