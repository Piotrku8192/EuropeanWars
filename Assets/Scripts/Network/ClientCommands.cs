using EuropeanWars.Core;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Time;
using EuropeanWars.UI;
using EuropeanWars.UI.Lobby;
using Lidgren.Network;
using System.Reflection;

namespace EuropeanWars.Network {
    public static class ClientCommands {
        public static void InvokeCommandById(ushort id, NetIncomingMessage message) {
            foreach (var item in typeof(ClientCommands).GetRuntimeMethods()) {
                var att = item.GetCustomAttribute<CommandAttribute>();
                if (att != null && att.id == id) {
                    item.Invoke(null, new object[1] { message });
                }
            }
        }

        #region Connection (0-127)

        [Command(1)]
        public static void UpdateReadyPlayers(NetIncomingMessage message) {
            int count = message.ReadInt32();
            for (int i = 0; i < count; i++) {
                GameInfo.countries[message.ReadInt32()].isPlayer = true;
            }
        }

        #endregion

        #region Lobby (128-255)

        [Command(128)]
        public static void UpdateReadyPlayer(NetIncomingMessage message) {
            int countryId = message.ReadInt32();
            bool b = message.ReadBoolean();
            GameInfo.countries[countryId].isPlayer = b;
            LobbyManager.Singleton.UpdateReadyButton(countryId);
        }

        [Command(129)]
        public static void SetPlayerReady(NetIncomingMessage message) {
            int countryId = message.ReadInt32();
            bool b = message.ReadBoolean();
            LobbyManager.Singleton.SetReady(countryId, b);
        }

        [Command(130)]
        public static void Play(NetIncomingMessage message) {
            UIManager.Singleton.lobby.SetActive(false);
            UIManager.Singleton.ui.SetActive(true);
            UIManager.Singleton.playerCountryCrest.sprite = GameInfo.PlayerCountry.crest;
            GameInfo.gameStarted = true;
        }

        #endregion

        #region Time (256-511)

        [Command(256)]
        public static void CountDay(NetIncomingMessage message) {
            TimeManager.CountDay();
        }

        [Command(257)]
        public static void SetSpeed(NetIncomingMessage message) {
            Core.Time.Timer.Singleton.ClientSetSpeed(message.ReadInt32());
        }

        [Command(258)]
        public static void SetPause(NetIncomingMessage message) {
            Core.Time.Timer.Singleton.ClientPause();
        }

        #endregion

        [Command(512)]
        public static void BuildBuilding(NetIncomingMessage message) {
            int building = message.ReadInt32();
            int province = message.ReadInt32();
            int slot = message.ReadInt32();
            BuildingInfo b = GameInfo.buildings[building];
            ProvinceInfo p = GameInfo.provinces[province];
            p.BuildBuilding(b, slot);
        }

        [Command(513)]
        public static void UpgradeProvince(NetIncomingMessage message) {
            int province = message.ReadInt32();
            ProvinceInfo p = GameInfo.provinces[province];
            p.UpgradeProvince();
        }

        [Command(514)]
        public static void DevastateProvince(NetIncomingMessage message) {
            int province = message.ReadInt32();
            ProvinceInfo p = GameInfo.provinces[province];
            p.DevastateProvince();
        }

        [Command(515)]
        public static void TakeLoans(NetIncomingMessage message) {
            int country = message.ReadInt32();
            int count = message.ReadInt32();

            GameInfo.countries[country].TakeLoans(count);
        }

        [Command(516)]
        public static void PayOffLoans(NetIncomingMessage message) {
            int country = message.ReadInt32();
            int count = message.ReadInt32();

            GameInfo.countries[country].PayOffLoans(count);
        }

        [Command(517)]
        public static void Bankruptcy(NetIncomingMessage message) {
            int country = message.ReadInt32();
            GameInfo.countries[country].Bankruptcy();
        }
    }
}
