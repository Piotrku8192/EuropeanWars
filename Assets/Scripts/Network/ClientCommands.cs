﻿using EuropeanWars.Core;
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

        #endregion

        #region Army (2048-3071)
        [Command(2048)]
        public static void RecruitUnit(NetIncomingMessage message) {
            UnitInfo unitInfo = GameInfo.units[message.ReadInt32()];
            CountryInfo country = GameInfo.countries[message.ReadInt32()];
            ProvinceInfo province = GameInfo.provinces[message.ReadInt32()];
            int count = message.ReadInt32();

            country.EnqueueUnitToRecruite(unitInfo, province, count);
        }

        [Command(2049)]
        public static void GenerateArmyRoute(NetIncomingMessage message) {
            int id = message.ReadInt32();
            int target = message.ReadInt32();

            GameInfo.armies[id].GenerateRoute(GameInfo.provinces[target]);
        }
        #endregion
    }
}