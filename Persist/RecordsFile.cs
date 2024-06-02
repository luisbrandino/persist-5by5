using Newtonsoft.Json;
using Models;

namespace Persist
{
    public class RecordsFile
    {
        public string Path { get; private set; }

        public RecordsFile(string path)
        {
            Path = path;
        }

        public List<Record> Read()
        {
            StreamReader reader = new(Path);

            return JsonConvert.DeserializeObject<RadarData>(reader.ReadToEnd())?.Records ?? new();
        }
    }
}
