using Newtonsoft.Json;

namespace Celeste.Mod.Archipelago.Models
{
    public class Side
    {
        [JsonProperty("strawberries")]
        public Strawberry[] Strawberries { get; set; }

        [JsonProperty("cassettes")]
        public Cassette[] Cassettes { get; set; }

        [JsonProperty("crystalHearts")]
        public CrystalHeart[] CrystalHearts { get; set; }
    }
}
