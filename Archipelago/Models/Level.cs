using Newtonsoft.Json;

namespace Celeste.Mod.Archipelago.Models
{
    public class Level
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("levelName")]
        public string LevelName { get; set; }

        [JsonProperty("formalName")]
        public string FormalName { get; set; }

        [JsonProperty("strawberries")]
        public Strawberry[] Strawberries { get; set; }

        [JsonProperty("cassettes")]
        public Cassette[] Cassettes { get; set; }

        [JsonProperty("crystalHearts")]
        public CrystalHeart[] CrystalHearts { get; set; }
    }
}
