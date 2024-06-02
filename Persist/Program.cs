using Databases.Drivers;
using Databases;
using Persist;
using Repositories;
using Services;
using Controllers;
using Models;

DatabaseClient database = new(new SqlServerDriver("db_radar", "sa", "SqlServer2019!"));
RecordRepository repository = new(database);
RecordService service = new(repository);
RecordController controller = new(service);

List<Record> records = new RecordsFile(@"C:\dev\dados-dos-radares.json").Read().ToList();

await controller.InsertManyInBatchesAsync(records, 64);

Console.WriteLine("Registros inseridos!");