using Databases;
using Newtonsoft.Json;

namespace Models
{
    [Table("tb_record")]
    [Collection("record")]
    public class Record : Model
    {
        [PrimaryKey]
        [Column("id")]
        public int Id { get; set; }

        [Column("concessionaire")]
        [JsonProperty("concessionaria")]
        public string Concessionaire { get; set; }

        [Column("year_pnv_snv")]
        [JsonProperty("ano_do_pnv_snv")]
        public string YearPNVandSnv { get; set; }

        [Column("radar_type")]
        [JsonProperty("tipo_de_radar")]
        public string RadarType { get; set; }

        [Column("highway")]
        [JsonProperty("rodovia")]
        public string Highway { get; set; }

        [Column("state")]
        [JsonProperty("uf")]
        public string State { get; set; }

        [Column("km_m")]
        [JsonProperty("km_m")]
        public string KM_m { get; set; }

        [Column("county")]
        [JsonProperty("municipio")]
        public string County { get; set; }

        [Column("track_type")]
        [JsonProperty("tipo_pista")]
        public string TrackType { get; set; }

        [Column("direction")]
        [JsonProperty("sentido")]
        public string Direction { get; set; }

        [Column("situation")]
        [JsonProperty("situacao")]
        public string Situation { get; set; }

        [Column("latitude")]
        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [Column("longitude")]
        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [Column("light_speed")]
        [JsonProperty("velocidade_leve")]
        public string LightSpeed { get; set; }
    }
}
