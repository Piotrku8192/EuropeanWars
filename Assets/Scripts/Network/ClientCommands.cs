using EuropeanWars.Core;
using EuropeanWars.Core.Pathfinding;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Time;
using EuropeanWars.UI;
using EuropeanWars.UI.Lobby;
using EuropeanWars.UI.Windows;
using Lidgren.Network;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;
using EuropeanWars.Core.Army;
using EuropeanWars.Core.War;
using EuropeanWars.Audio;

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
            foreach (var item in GameInfo.provinces) {
                item.Value.RefreshFogOfWar();
            }
            GameInfo.UnselectProvince();

            MusicManager.Singleton.audioSource.Stop();
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

        #region Economy (512-1023)
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
        #endregion

        #region Diplomacy (1024-2047)

        #region Alliance
        [Command(1024)]
        public static void AllianceRequest(NetIncomingMessage message) {
            int sender = message.ReadInt32();
            int receiver = message.ReadInt32();

            Alliance.AllianceRequestClient(GameInfo.countries[sender], GameInfo.countries[receiver]);
        }

        [Command(1025)]
        public static void AcceptAlliance(NetIncomingMessage message) {
            int sender = message.ReadInt32();
            int receiver = message.ReadInt32();

            Alliance alliance = new Alliance();
            alliance.countries.Add(GameInfo.countries[sender]);
            alliance.countries.Add(GameInfo.countries[receiver]);

            Alliance.CreateAllianceClient(alliance);
        }

        [Command(1026)]
        public static void DeliceAlliance(NetIncomingMessage message) {
            int sender = message.ReadInt32();
            int receiver = message.ReadInt32();

            //TODO: Implement translation
            var win = DiplomacyWindow.Singleton.SpawnRequest(new DiplomaticRelation() {
                countries = new List<Core.Country.CountryInfo>() {
                    GameInfo.countries[sender],
                    GameInfo.countries[receiver]
                }
            }, true);
            win.title.text = "Odrzucono sojusz";
            win.description.text = "Państwo, któremu zaproponowaliśmy sojusz odrzuciło naszą propozycję!";
            win.acceptText.text = "Ok";
            win.deliceText.GetComponentInParent<Button>().gameObject.SetActive(false);

            Alliance.messageSent = false;
        }

        [Command(1027)]
        public static void DeleteAlliance(NetIncomingMessage message) {
            int sender = message.ReadInt32();
            int receiver = message.ReadInt32();
            int s = message.ReadInt32();

            Alliance alliance = DiplomacyManager.alliances.Where(t => t.countries.Contains(GameInfo.countries[sender])
            && t.countries.Contains(GameInfo.countries[receiver])).FirstOrDefault();

            Alliance.DeleteAllianceClient(alliance, s);
        }
        #endregion

        #region MilitaryAccess
        [Command(1028)]
        public static void AccessRequest(NetIncomingMessage message) {
            int sender = message.ReadInt32();
            int receiver = message.ReadInt32();

            MilitaryAccess.AccessRequestClient(GameInfo.countries[sender], GameInfo.countries[receiver]);
        }

        [Command(1029)]
        public static void AcceptAccess(NetIncomingMessage message) {
            int sender = message.ReadInt32();
            int receiver = message.ReadInt32();

            MilitaryAccess access = new MilitaryAccess();
            access.countries.Add(GameInfo.countries[sender]);
            access.countries.Add(GameInfo.countries[receiver]);

            MilitaryAccess.CreateAccessClient(access);
        }

        [Command(1030)]
        public static void DeliceAccess(NetIncomingMessage message) {
            int sender = message.ReadInt32();
            int receiver = message.ReadInt32();

            //TODO: Implement translation
            var win = DiplomacyWindow.Singleton.SpawnRequest(new DiplomaticRelation() {
                countries = new List<Core.Country.CountryInfo>() {
                    GameInfo.countries[sender],
                    GameInfo.countries[receiver]
                }
            }, true);
            win.title.text = "Odrzucono prawo przemarszu";
            win.description.text = "Państwo, któremu zaproponowaliśmy prawo przemarszu odrzuciło naszą propozycję!";
            win.acceptText.text = "Ok";
            win.deliceText.GetComponentInParent<Button>().gameObject.SetActive(false);

            MilitaryAccess.messageSent = false;
        }

        [Command(1031)]
        public static void DeleteAccess(NetIncomingMessage message) {
            int sender = message.ReadInt32();
            int receiver = message.ReadInt32();
            int s = message.ReadInt32();

            MilitaryAccess access = DiplomacyManager.militaryAccesses.Where(t => t.countries.Contains(GameInfo.countries[sender])
            && t.countries.Contains(GameInfo.countries[receiver])).FirstOrDefault();

            MilitaryAccess.DeleteAccessClient(access, s);
        }
        #endregion

        #region Claims
        [Command(1032)]
        public static void FabricateClaim(NetIncomingMessage message) {
            int province = message.ReadInt32();
            int country = message.ReadInt32();

            GameInfo.countries[country].EnqueFabricateClaim(GameInfo.provinces[province]);
        }
        #endregion

        #region War
        [Command(1033)]
        public static void AcceptWarInvitation(NetIncomingMessage message) {
            int war = message.ReadInt32();
            int country = message.ReadInt32();
            bool isAttacker = message.ReadBoolean();

            DiplomacyManager.wars[war].JoinWar(GameInfo.countries[country], isAttacker);
        }

        [Command(1034)]
        public static void DeliceWarInvitation(NetIncomingMessage message) {
            int inviter = message.ReadInt32();
            int country = message.ReadInt32();
            CountryInfo i = GameInfo.countries[inviter];
            CountryInfo c = GameInfo.countries[country];

            if (i.alliances.ContainsKey(c)) {
                Alliance.DeleteAllianceClient(i.alliances[c], inviter);
            }
        }

        [Command(1035)]
        public static void DeclareWar(NetIncomingMessage message) {
            int attacker = message.ReadInt32();
            int defender = message.ReadInt32();
            int warReason = message.ReadInt32();
            CountryInfo a = GameInfo.countries[attacker];
            CountryInfo d = GameInfo.countries[defender];

            if (a == GameInfo.PlayerCountry) {
                DiplomacyWindow.Singleton.UpdateWindow();
            }

            if (!a.IsInWarAgainstCountry(d)) {
                WarReasonFactory factory = new WarReasonFactory(a, d);
                WarReason w = factory.GetReasons()[warReason];
                DiplomacyManager.DeclareWar(w, a, d);

                if (d == GameInfo.PlayerCountry) {
                    //TODO: Implement translation
                    DipRequestWindow win = DiplomacyWindow.Singleton.SpawnRequest(new DiplomaticRelation() {
                        countries = new List<CountryInfo>() { a, d } }, true);
                    win.title.text = "Wojna!";
                    win.description.text = "Nasz niedaleki sąsiad wypowiedział nam wojnę!";
                    win.acceptText.text = "Ok";
                    win.deliceText.text = "Ok";
                }
            }
        }

        [Command(1036)]
        public static void SendPeaceDeal(NetIncomingMessage message) {
            int war = message.ReadInt32();
            int sender = message.ReadInt32();
            int receiver = message.ReadInt32();
            int gainedGold = message.ReadInt32();

            int senderCount = message.ReadInt32();
            List<int> senderElements = new List<int>();
            for (int i = 0; i < senderCount; i++) {
                senderElements.Add(message.ReadInt32());
            }

            int receiverCount = message.ReadInt32();
            List<int> receiverElements = new List<int>();
            for (int i = 0; i < receiverCount; i++) {
                receiverElements.Add(message.ReadInt32());
            }

            WarInfo w = DiplomacyManager.wars[war];
            PeaceDeal deal = new PeaceDeal(w, GameInfo.countries[sender].wars[w], GameInfo.countries[receiver].wars[w]);
            deal.ChangeGold(gainedGold / 10);
            deal.selectedSenderElements.AddRange(senderElements);
            deal.selectedReceiverElements.AddRange(receiverElements);
            deal.Execute();
        }

        [Command(1037)]
        public static void PeaceDealRequest(NetIncomingMessage message) {
            int war = message.ReadInt32();
            int sender = message.ReadInt32();
            int receiver = message.ReadInt32();
            int gainedGold = message.ReadInt32();

            int senderCount = message.ReadInt32();
            List<int> senderElements = new List<int>();
            for (int i = 0; i < senderCount; i++) {
                senderElements.Add(message.ReadInt32());
            }

            int receiverCount = message.ReadInt32();
            List<int> receiverElements = new List<int>();
            for (int i = 0; i < receiverCount; i++) {
                receiverElements.Add(message.ReadInt32());
            }

            WarInfo w = DiplomacyManager.wars[war];
            PeaceDeal deal = new PeaceDeal(w, GameInfo.countries[sender].wars[w], GameInfo.countries[receiver].wars[w]);
            deal.ChangeGold(gainedGold / 10);
            deal.selectedSenderElements.AddRange(senderElements);
            deal.selectedReceiverElements.AddRange(receiverElements);
            deal.ProcessRequest();
        }

        [Command(1038)]
        public static void SendDelicePeaceDeal(NetIncomingMessage message) {
            int sender = message.ReadInt32();
            int receiver = message.ReadInt32();

            if (sender == GameInfo.PlayerCountry.id) {
                DipRequestWindow window = DiplomacyWindow.Singleton.SpawnRequest(new DiplomaticRelation() {
                    countries = new List<CountryInfo>() {
                        GameInfo.countries[sender], GameInfo.countries[receiver]
                    }
                },
                true);
                //TODO: translations!!!!
                window.title.text = "Odrzucono propozycję pokoju!";
                window.description.text = $"Państwo {GameInfo.countries[receiver].name} odrzuciło naszą propozycję pokoju.";
                window.acceptText.text = "Ok";
                window.deliceText.gameObject.SetActive(false);
            }
        }
        #endregion

        #endregion

        #region Army (2048-3071)
        [Command(2048)]
        public static void RecruitUnit(NetIncomingMessage message) {
            UnitInfo unitInfo = GameInfo.units[message.ReadInt32()];
            CountryInfo country = GameInfo.countries[message.ReadInt32()];
            ProvinceInfo province = GameInfo.provinces[message.ReadInt32()];
            int count = message.ReadInt32();

            country.EnqueueUnitToRecruit(unitInfo, province, count);
        }

        [Command(2049)]
        public static void GenerateArmyRoute(NetIncomingMessage message) {
            int id = message.ReadInt32();
            int target = message.ReadInt32();

            GameInfo.armies[id].GenerateRoute(GameInfo.provinces[target]);
        }

        [Command(2050)]
        public static void AddUnit(NetIncomingMessage message) {
            int army = message.ReadInt32();
            int unit = message.ReadInt32();
            int count = message.ReadInt32();
            int maxCount = message.ReadInt32();

            GameInfo.armies[army].AddUnit(GameInfo.units[unit], count, maxCount);
        }


        [Command(2051)]
        public static void RemoveUnit(NetIncomingMessage message) {
            int army = message.ReadInt32();
            int unit = message.ReadInt32();
            int count = message.ReadInt32();

            GameInfo.armies[army].RemoveUnit(GameInfo.units[unit], count);
        }

        [Command(2052)]
        public static void DeleteArmy(NetIncomingMessage message) {
            int id = message.ReadInt32();
            if (GameInfo.armies.ContainsKey(id)) {
                GameInfo.armies[id].DeleteLocal();
            }
        }
        #endregion
    }
}
