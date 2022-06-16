using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Packets;
using Celeste.Mod.Archipelago.Models;
using Newtonsoft.Json;

namespace Celeste.Mod.Archipelago
{
    public class ArchipelagoModule : EverestModule
    {
        public static ArchipelagoModule Instance;

        public override Type SettingsType => typeof(ArchipelagoModuleSettings);
        public static ArchipelagoModuleSettings Settings => (ArchipelagoModuleSettings)Instance._Settings;

        private ArchipelagoSession currentAPSession;
        private int StrawberryCount = 0;
        private LevelPack levelPack;


        // If you need to store save data:
        //public override Type SaveDataType => typeof(ExampleModuleSaveData);
        //public static ExampleModuleSaveData SaveData => (ExampleModuleSaveData)Instance._SaveData;

        public ArchipelagoModule()
        {
            Instance = this;
            
        }

        public override void Load()
        {
            this.LoadLevelPack();

            On.Celeste.Strawberry.CollectRoutine += Strawberry_CollectRoutine;
            //On.Celeste.Cassette.CollectRoutine += Cassette_CollectRoutine;
            On.Celeste.Session.ctor += onSessionStart;
            On.Celeste.Level.RegisterAreaComplete += Level_RegisterAreaComplete;
        }

        public override void Unload()
        {
            On.Celeste.Strawberry.CollectRoutine -= Strawberry_CollectRoutine;
            //On.Celeste.Cassette.CollectRoutine -= Cassette_CollectRoutine;
            On.Celeste.Session.ctor -= onSessionStart;
            On.Celeste.Level.RegisterAreaComplete -= Level_RegisterAreaComplete;

            if (currentAPSession != null && currentAPSession.Socket.Connected)
            {
                currentAPSession.Socket.Disconnect();
            }
        }

        private void LoadLevelPack()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Celeste.Mod.Archipelago.CelesteLocations.json";

            var resources = assembly.GetManifestResourceNames();
            Logger.Log("Resources", JsonConvert.SerializeObject(resources));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string locationText = reader.ReadToEnd();
                this.levelPack = JsonConvert.DeserializeObject<LevelPack>(locationText);
            }
        }

        /*private System.Collections.IEnumerator Cassette_CollectRoutine(On.Celeste.Cassette.orig_CollectRoutine orig, Cassette self, Player player)
        {
            Session session = self.SceneAs<Level>().Session;
            Logger.Log("Cassette collected", self.Tag.ToString());
            if (!CassetteMap.ContainsKey(session.Area.ChapterIndex))
            {
                Logger.Log("Cassette Location Not Mapped", session.Area.ChapterIndex.ToString());
            }
            else
            {
                long location = CassetteMap[session.Area.ChapterIndex];
                Logger.Log("Location Complete", location.ToString());
            }

            return orig(self, player);
        }*/

        private void Level_RegisterAreaComplete(On.Celeste.Level.orig_RegisterAreaComplete orig, Level self)
        {
            // Currently complete upon ForsakenCity completion
            // TODO: Only send completion once
            if (self.Session.Area.ChapterIndex == 2 && self.Session.Area.Mode == AreaMode.Normal)
            {
                Logger.Log("Finished level", self.Session.Area.GetSID());

                this.currentAPSession.Socket.SendPacket(new StatusUpdatePacket { Status = ArchipelagoClientState.ClientGoal });
            }

            // TODO: announce level completion? Store completed levels.

            orig(self);
        }

        private System.Collections.IEnumerator Strawberry_CollectRoutine(On.Celeste.Strawberry.orig_CollectRoutine orig, Strawberry self, int collectIndex)
        {
            Logger.Log("strawberry", self.ID.ToString());
            Logger.Log("collectIndex", collectIndex.ToString());

            Session session = self.SceneAs<Level>().Session;
            Logger.Log("Level", session.Area.GetSID());
            Logger.Log("Chapter", session.Area.ChapterIndex.ToString());
            Logger.Log("Count Safe", SaveData.Instance.TotalStrawberries_Safe.ToString());
            Logger.Log("Count", SaveData.Instance.TotalStrawberries.ToString());

            var level = this.levelPack.Levels.FirstOrDefault(lvl => lvl.Id == session.Area.ChapterIndex);
            if (level == null)
            {
                Logger.Log("Level not mapped", session.Area.ChapterIndex.ToString());
            }
            else
            {
                var location =
                    level.Strawberries.FirstOrDefault(berry => berry.Id == self.ID.ID && berry.Room == self.ID.Level);

                if (location == null)
                {
                    Logger.Log("AP Location Not Mapped", self.ID.ToString());
                }
                else
                {
                    Logger.Log("Location Complete", location.ToString());

                    this.currentAPSession.Locations.CompleteLocationChecks(location.ArchipelagoId);
                }
            }

            

            return orig(self, collectIndex);

        }

        public void onSessionStart(On.Celeste.Session.orig_ctor orig, Session self)
        {
            orig(self);

            if (currentAPSession == null)
            {
                var urlParts = Settings.Server.Split(':');
                if (urlParts.Length < 2)
                {
                    throw new Exception("Archipelago server settings are not set");
                }

                currentAPSession = ArchipelagoSessionFactory.CreateSession(urlParts[0], Convert.ToInt32(urlParts[1]));

                currentAPSession.TryConnectAndLogin("Celeste", Settings.UserName, new Version(0, 3, 2), ItemsHandlingFlags.AllItems);

                currentAPSession.Items.ItemReceived += onReceiveItem;

                // Set default storage values
                //currentAPSession.DataStorage["StrawberryCount"].Initialize(0);
            }
        }

        private void onReceiveItem(ReceivedItemsHelper receivedItemsHelper)
        {
            var itemReceivedName = receivedItemsHelper.PeekItemName();
            Logger.Log("AP Item received", itemReceivedName);

            if (itemReceivedName == "Strawberry")
            {
                this.StrawberryCount++;
                Logger.Log("Strawberry Count", this.StrawberryCount.ToString());
                // TODO: Update the data storage value
            }

            receivedItemsHelper.DequeueItem();
        }
    }
}
