using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Celeste.Mod.Archipelago.Models
{
    internal class LevelPack
    {
        [JsonProperty("levelPackName")]
        public string LevelPackName { get; set; }

        [JsonProperty("levelPackVersion")]
        public string LevelPackVersion { get; set; }

        [JsonProperty("packOrder")]
        public int PackOrder { get; set; }

        [JsonProperty("levels")]
        public Level[] Levels { get; set; }
    }
}
