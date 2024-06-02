using Models;
using Repositories;

namespace Services
{
    public class RecordService
    {
        private RecordRepository _repository;

        public RecordService(RecordRepository repository)
        {
            _repository = repository;
        }

        public List<Record> FindAll() 
        {
            return _repository.FindAll();
        }

        public void InsertMany(List<Record> records)
        {
            _repository.InsertMany(records);
        }

        public async Task InsertManyInBatchesAsync(List<Record> records, int batchSize = 0)
        {
            await _repository.InsertManyInBatchesAsync(records, batchSize);
        }

    }
}
