using Databases.Drivers;
using Databases;
using Models;

namespace Repositories
{
    public class RecordRepository
    {
        private IDatabaseDriver _driver;

        public RecordRepository(IDatabaseDriver driver)
        {
            _driver = driver;
        }

        public List<Record> FindAll()
        {
            return _driver.FindAll<Record>();
        }

        public void InsertMany(List<Record> records)
        {
            _driver.InsertMany(records.Cast<Model>().ToList());
        }

        public async Task InsertManyInBatchesAsync(List<Record> records, int batchSize = 0)
        {
            if (batchSize == 0)
                batchSize = records.Count / 10;

            int batches = (int) (records.Count / batchSize);

            var tasks = new List<Task>();

            for (int i = 0; i <= batches; i++)
            {
                var batch = records.Skip(i * batchSize).Take(batchSize).Cast<Model>().ToList();
                tasks.Add(Task.Run(() => _driver.InsertMany(batch)));
            }

            await Task.WhenAll(tasks);
        }

    }
}
