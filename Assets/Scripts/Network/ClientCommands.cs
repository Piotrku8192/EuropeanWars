﻿using EuropeanWars.Audio;
using EuropeanWars.Core;
using EuropeanWars.Core.AI;
using EuropeanWars.Core.Army;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Language;
using EuropeanWars.Core.Persons;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Time;
using EuropeanWars.Core.War;
using EuropeanWars.UI;
using EuropeanWars.UI.Lobby;
using EuropeanWars.UI.Windows;
using Lidgren.Network;
using System;
using System.Collections.Generic;
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
            UIManager.Singleton.playerCountryCrest.sprite = GameInfo.PlayerCountry.Crest;
            GameInfo.gameStarted = true;
            foreach (var item in GameInfo.provinces) {
                item.Value.RefreshFogOfWar();
            }
            GameInfo.UnselectProvince();

            //if (Client.Singleton.IsHost) {
                foreach (var item in GameInfo.countries) {
                    if (item.Value.id > 0) {
                        GameInfo.countryAIs.Add(item.Value, new BoomerCountryAI(item.Value));
                    }
                }
            //}

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

        [Command(518)]
        public static void LootProvince(NetIncomingMessage message) {
            int id = message.ReadInt32();
            GameInfo.provinces[id].LootProvince();
        }

        #endregion

        #region Diplomacy (1024-2047)

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
            bool isAttacker = message.ReadBoolean();

            if (i.relations[c].relations[(int)DiplomaticRelation.Alliance] && !isAttacker) {
                i.relations[c].ChangeRelationState(DiplomaticRelation.Alliance, i, c);
            }
        }

        [Command(1035)]
        public static void DeclareWar(NetIncomingMessage message) {
            int attacker = message.ReadInt32();
            int defender = message.ReadInt32();
            int warReason = message.ReadInt32();
            CountryInfo a = GameInfo.countries[attacker];
            CountryInfo d = GameInfo.countries[defender];

            WarReasonFactory factory = new WarReasonFactory(a, d);
            WarReason w = factory.GetReasons()[warReason];
            DiplomacyManager.DeclareWar(w, a, d);

            if (DiplomacyWindow.Singleton.window.activeInHierarchy 
                && (DiplomacyWindow.Singleton.countryWindow.country == a || DiplomacyWindow.Singleton.countryWindow.country == d)) {
                DiplomacyWindow.Singleton.UpdateWindow();
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

            if (DiplomacyManager.wars.ContainsKey(war)) {
                WarInfo w = DiplomacyManager.wars[war];
                PeaceDeal deal = new PeaceDeal(w, GameInfo.countries[sender].wars[w], GameInfo.countries[receiver].wars[w]);
                deal.ChangeGold(gainedGold / 10);
                deal.selectedSenderElements.AddRange(senderElements);
                deal.selectedReceiverElements.AddRange(receiverElements);
                deal.Execute();
            }
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
                DipRequestWindow window = DiplomacyWindow.Singleton.SpawnRequest(GameInfo.countries[sender], GameInfo.countries[receiver], true);
                window.title.text = LanguageDictionary.language["PeaceDeal"];
                window.description.text = string.Format(LanguageDictionary.language["PeaceDealDeliceDescription"],
                GameInfo.countries[receiver].Name);
                window.acceptText.text = "Ok";
                window.deliceText.transform.parent.gameObject.SetActive(false);
            }
        }
        #endregion

        [Command(1039)]
        public static void ChangeRelationState(NetIncomingMessage message) {
            CountryInfo sender = GameInfo.countries[message.ReadInt32()];
            CountryInfo receiver = GameInfo.countries[message.ReadInt32()];
            int relation = message.ReadInt32();

            sender.relations[receiver].ChangeRelationState((DiplomaticRelation)relation, sender, receiver);
        }

        [Command(1040)]
        public static void ChangeRelationStateRequest(NetIncomingMessage message) {
            CountryInfo sender = GameInfo.countries[message.ReadInt32()];
            CountryInfo receiver = GameInfo.countries[message.ReadInt32()];
            int relation = message.ReadInt32();

            sender.relations[receiver].ProcessRequest((Core.Diplomacy.DiplomaticRelation)relation, sender, receiver);
        }

        [Command(1041)]
        public static void ChangeRelationStateDelice(NetIncomingMessage message) {
            CountryInfo sender = GameInfo.countries[message.ReadInt32()];
            CountryInfo receiver = GameInfo.countries[message.ReadInt32()];
            int relation = message.ReadInt32();

            if (sender == GameInfo.PlayerCountry) {
                DipRequestWindow window = DiplomacyWindow.Singleton.SpawnRequest(sender, receiver, true);
                window.acceptText.text = "Ok";
                window.deliceText.transform.parent.gameObject.SetActive(false);
                window.title.text = LanguageDictionary.language[Enum.GetName(typeof(DiplomaticRelation), relation)];
                window.description.text = string.Format(
                    LanguageDictionary.language["DiplomaticRelationDeliced"], receiver.Name, window.title.text);
            }

            if (DiplomacyWindow.Singleton.window.activeInHierarchy) {
                DiplomacyWindow.Singleton.UpdateWindow();
            }
        }

        [Command(1042)]
        public static void SetDiplomatInRelation(NetIncomingMessage message) {
            int country = message.ReadInt32();
            int secondCountry = message.ReadInt32();
            int personId = message.ReadInt32();

            Diplomat d = (Diplomat)GameInfo.persons[personId];
            if (country == -1) {
                d.ImproveRelation(null);
            }
            else {
                d.ImproveRelation(GameInfo.countries[country].relations[GameInfo.countries[secondCountry]]);
            }
        }
        [Command(1043)]
        public static void SetSpyInRelation(NetIncomingMessage message) {
            int country = message.ReadInt32();
            int secondCountry = message.ReadInt32();
            int personId = message.ReadInt32();

            Spy d = (Spy)GameInfo.persons[personId];
            if (country == -1) {
                d.BuildSpyNetwork(null);
            }
            else {
                d.BuildSpyNetwork(GameInfo.countries[country].relations[GameInfo.countries[secondCountry]]);
            }
        }

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

            if (GameInfo.armies.ContainsKey(id)) {
                GameInfo.armies[id].GenerateRoute(GameInfo.provinces[target]);
            }
        }

        [Command(2050)]
        public static void AddUnit(NetIncomingMessage message) {
            int army = message.ReadInt32();
            int unit = message.ReadInt32();
            int count = message.ReadInt32();
            int maxCount = message.ReadInt32();

            if (GameInfo.armies.ContainsKey(army)) {
                GameInfo.armies[army].AddUnit(GameInfo.units[unit], count, maxCount);
            }
        }


        [Command(2051)]
        public static void RemoveUnit(NetIncomingMessage message) {
            int army = message.ReadInt32();
            int unit = message.ReadInt32();
            int count = message.ReadInt32();

            if (GameInfo.armies.ContainsKey(army)) {
                GameInfo.armies[army].RemoveUnit(GameInfo.units[unit], count);

            }
        }

        [Command(2052)]
        public static void DeleteArmy(NetIncomingMessage message) {
            int id = message.ReadInt32();
            if (GameInfo.armies.ContainsKey(id)) {
                GameInfo.armies[id].DeleteLocal();
            }
        }

        [Command(2053)]
        public static void CreateArmy(NetIncomingMessage message) {
            CountryInfo country = GameInfo.countries[message.ReadInt32()];
            ProvinceInfo province = GameInfo.provinces[message.ReadInt32()];
            UnitInfo unit = GameInfo.units[message.ReadInt32()];
            int count = message.ReadInt32();
            int maxCount = message.ReadInt32();
            bool selectAsMovingArmy = message.ReadBoolean();
            new ArmyInfo(province, country, unit, count, maxCount, selectAsMovingArmy);
        }

        [Command(2054)]
        public static void SetGeneral(NetIncomingMessage message) {
            int armyId = message.ReadInt32();
            int generalId = message.ReadInt32();

            GameInfo.armies[armyId].SetGeneral(generalId == -1 ? null : (General)GameInfo.persons[generalId]);
        }

        [Command(2055)]
        public static void RecruitMercenaries(NetIncomingMessage message) {
            int mercenariesInfo = message.ReadInt32();
            int country = message.ReadInt32();
            int province = message.ReadInt32();

            new Mercenaries(GameInfo.mercenaries[mercenariesInfo], GameInfo.countries[country], GameInfo.provinces[province]);
        }
        
        [Command(2056)]
        public static void RecruitCommonArmy(NetIncomingMessage message) {
            int country = message.ReadInt32();
            GameInfo.countries[country].RecruitCommonArmy();
        }
        #endregion
    }
}
