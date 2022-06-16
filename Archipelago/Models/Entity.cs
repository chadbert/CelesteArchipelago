using Newtonsoft.Json;

namespace Celeste.Mod.Archipelago.Models
{
    public class Entity
    {
        [JsonProperty("room")] 
        public string Room { get; set; }

        [JsonProperty("name")] 
        public string Name { get; set; }

        [JsonProperty("ID")] 
        public int Id { get; set; }

        [JsonProperty("archipelagoId")]
        public long ArchipelagoId { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Room}:{Id}) ({ArchipelagoId})";
        }
    }
}