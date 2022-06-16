using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.Archipelago
{
    public class ArchipelagoModuleSettings : EverestModuleSettings
    {
        [SettingMaxLength(128)]
        public string Server { get; set; } = "archipelago.gg:";

        [SettingMaxLength(16)] 
        public string UserName { get; set; } = "Player";
    }
}
