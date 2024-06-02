using Models;
using Services;

namespace Controllers
{
    public class RecordController
    {
        private RecordService _service;

        public RecordController(RecordService service)
        {
            _service = service;
        }

        public List<Record> FindAll()
        {
            return _service.FindAll();
        }

        public void InsertMany(List<Record> records)
        {
            _service.InsertMany(records);
        }

        public async Task InsertManyInBatchesAsync(List<Record> records, int batchSize = 0)
        {
            await _service.InsertManyInBatchesAsync(records, batchSize);
        }

    }
}
