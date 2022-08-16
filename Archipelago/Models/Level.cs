using Newtonsoft.Json;

using System.Collections.Generic;

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

            [JsonProperty("sides")]
            public Dictionary<string, Side> Sides { get; set; }
        }
}
