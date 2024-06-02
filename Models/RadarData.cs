using Databases;
using Newtonsoft.Json;

namespace Models
{
    public class RadarData : Model
    {

        [JsonProperty("radar")]
        public List<Record> Records;

    }
}
