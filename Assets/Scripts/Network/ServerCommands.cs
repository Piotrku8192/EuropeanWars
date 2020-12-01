using EuropeanWars.Core;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Province;
using EuropeanWars.UI.Lobby;
using Lidgren.Network;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EuropeanWars.Network {
    public static class ServerCommands {
        public static void InvokeCommandById(ushort id, NetIncomingMessage message) {
            foreach (var item in typeof(ServerCommands).GetRuntimeMethods()) {
                var att = item.GetCustomAttribute<CommandAttribute>();
                if (att != null && att.id == id) {
                    item.Invoke(null, new object[1] { message });
                }
            }
        }

        #region Connection (0-127)

        /// <summary>
        /// Command, which check if client can play on this server.
        /// </summary>
        /// <param name="message"></param>
        [Command(0)]
        public static void AuthorizeClient(NetIncomingMessage message) {
            ClientInfo client = new ClientInfo();
            client.connection = message.SenderConnection;
            client.countryId = 0;
            client.nick = Server.Singleton.clients.Count.ToString();
            Server.Singleton.clients.Add(client.connection, client);
        }
        [Command(1)]
        public static void SendPlayersCountries(NetIncomingMessage message) {
            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)1);
            int count = 0;
            List<int> ctrs = new List<int>();
            foreach (var item in GameInfo.countries) {
                if (item.Value.isPlayer) {
                    ctrs.Add(item.Key);
                    count++;
                }
            }
            msg.Write(count);
            foreach (var item in ctrs) {
                msg.Write(item);
            }
            Server.Singleton.s.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
        }

        #endregion

        #region Lobby (128-255)

        /// <summary>
        /// Sets player country to new if nobody already selected it.
        /// </summary>
        /// <param name="message"></param>
        [Command(128)]
        public static void SetPlayerReady(NetIncomingMessage message) {
            int countryId = message.ReadInt32();
            bool b = Server.Singleton.clients.Where(t => t.Value.countryId == countryId
                && t.Value.isReady && t.Key != message.SenderConnection).Any();

            if (!b) {
                Server.Singleton.clients[message.SenderConnection].countryId = countryId;
                Server.Singleton.clients[message.SenderConnection].isReady = !Server.Singleton.clients[message.SenderConnection].isReady;

                NetOutgoingMessage brodcastMsg = Server.Singleton.s.CreateMessage();
                brodcastMsg.Write((ushort)128);
                brodcastMsg.Write(countryId);
                brodcastMsg.Write(Server.Singleton.clients[message.SenderConnection].isReady);
                Server.Singleton.s.SendToAll(brodcastMsg, NetDeliveryMethod.ReliableOrdered);

                NetOutgoingMessage clientMsg = Server.Singleton.s.CreateMessage();
                clientMsg.Write((ushort)129);
                clientMsg.Write(countryId);
                clientMsg.Write(Server.Singleton.clients[message.SenderConnection].isReady);
                Server.Singleton.s.SendMessage(clientMsg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);

                LobbyManager.Singleton.playButton.interactable = !Server.Singleton.clients.Where(t => !t.Value.isReady).Any();
            }
        }

        #endregion

        #region Time (256-511)

        [Command(256)]
        public static void Pause(NetIncomingMessage message) {
            if (Core.Time.Timer.Singleton.speed == 0 && Core.Time.Timer.Singleton.lastSpeed > 0) {
                Core.Time.Timer.Singleton.ServerSetSpeed(Core.Time.Timer.Singleton.lastSpeed);
            }
            else {
                Core.Time.Timer.Singleton.lastSpeed = Core.Time.Timer.Singleton.speed;
                Core.Time.Timer.Singleton.speed = 0;

                NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
                msg.Write((ushort)258);
                Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
            }
        }

        #endregion

        #region Economy (512-1023)
        [Command(512)]
        public static void BuildBuildingRequest(NetIncomingMessage message) {
            int building = message.ReadInt32();
            int province = message.ReadInt32();
            int slot = message.ReadInt32();
            BuildingInfo b = GameInfo.buildings[building];
            ProvinceInfo p = GameInfo.provinces[province];

            if ((p.Country.id == Server.Singleton.clients[message.SenderConnection].countryId || !p.Country.isPlayer) && b.CanBuildInProvince(p)) {
                NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
                msg.Write((ushort)512);
                msg.Write(building);
                msg.Write(province);
                msg.Write(slot);
                Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
            }
        }

        [Command(513)]
        public static void UpgradeProvinceRequest(NetIncomingMessage message) {
            int province = message.ReadInt32();
            ProvinceInfo p = GameInfo.provinces[province];

            if (p.Country.id == Server.Singleton.clients[message.SenderConnection].countryId || !p.Country.isPlayer) {
                NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
                msg.Write((ushort)513);
                msg.Write(province);
                Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
            }
        }

        [Command(514)]
        public static void DevastateProvinceRequest(NetIncomingMessage message) {
            int province = message.ReadInt32();
            ProvinceInfo p = GameInfo.provinces[province];

            if (p.Country.id == Server.Singleton.clients[message.SenderConnection].countryId || !p.Country.isPlayer) {
                NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
                msg.Write((ushort)514);
                msg.Write(province);
                Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
            }
        }

        [Command(515)]
        public static void TakeLoans(NetIncomingMessage message) {
            int country = message.ReadInt32();
            int count = message.ReadInt32();

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)515);
            msg.Write(country);
            msg.Write(count);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }

        [Command(516)]
        public static void PayOffLoans(NetIncomingMessage message) {
            int country = message.ReadInt32();
            int count = message.ReadInt32();

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)516);
            msg.Write(country);
            msg.Write(count);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }

        [Command(517)]
        public static void Bankruptcy(NetIncomingMessage message) {
            int country = message.ReadInt32();

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)517);
            msg.Write(country);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }
        #endregion

        #region Diplomacy (1024-2047)

        #region Claims
        [Command(1032)]
        public static void FabricateClaim(NetIncomingMessage message) {
            int province = message.ReadInt32();
            int country = message.ReadInt32();

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)1032);
            msg.Write(province);
            msg.Write(country);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }
        #endregion

        #region War
        [Command(1033)]
        public static void AcceptWarInvitation(NetIncomingMessage message) {
            int war = message.ReadInt32();
            int country = message.ReadInt32();
            bool isAttacker = message.ReadBoolean();

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)1033);
            msg.Write(war);
            msg.Write(country);
            msg.Write(isAttacker);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }

        [Command(1034)]
        public static void DeliceWarInvitation(NetIncomingMessage message) {
            int inviter = message.ReadInt32();
            int country = message.ReadInt32();

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)1034);
            msg.Write(inviter);
            msg.Write(country);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }

        [Command(1035)]
        public static void DeclareWar(NetIncomingMessage message) {
            int attacker = message.ReadInt32();
            int defender = message.ReadInt32();
            int warReason = message.ReadInt32();

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)1035);
            msg.Write(attacker);
            msg.Write(defender);
            msg.Write(warReason);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }

        [Command(1036)]
        public static void SendPeaceDeal(NetIncomingMessage message) {
            byte[] b = message.ReadBytes(message.LengthBytes - 2);

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)1036);
            msg.Write(b);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }

        [Command(1037)]
        public static void PeaceDealRequest(NetIncomingMessage message) {
            int receiver = message.ReadInt32();
            byte[] b = message.ReadBytes(message.LengthBytes - 6);

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)1037);
            msg.Write(b);
            Server.Singleton.s.SendMessage(msg, Server.Singleton.clients.Where(t =>
            t.Value.countryId == receiver).FirstOrDefault().Key, NetDeliveryMethod.ReliableOrdered);
        }

        [Command(1038)]
        public static void SendDelicePeaceDeal(NetIncomingMessage message) {
            int sender = message.ReadInt32();
            int receiver = message.ReadInt32();

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)1038);
            msg.Write(sender);
            msg.Write(receiver);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }
        #endregion

        [Command(1039)]
        public static void ChangeRelationState(NetIncomingMessage message) {
            int sender = message.ReadInt32();
            int receiver = message.ReadInt32();
            int relation = message.ReadInt32();

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)1039);
            msg.Write(sender);
            msg.Write(receiver);
            msg.Write(relation);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }

        [Command(1040)]
        public static void ChangeRelationStateRequest(NetIncomingMessage message) {
            int sender = message.ReadInt32();
            int receiver = message.ReadInt32();
            int relation = message.ReadInt32();

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)1040);
            msg.Write(sender);
            msg.Write(receiver);
            msg.Write(relation);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }

        [Command(1041)]
        public static void ChangeRelationStateDelice(NetIncomingMessage message) {
            int sender = message.ReadInt32();
            int receiver = message.ReadInt32();
            int relation = message.ReadInt32();

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)1041);
            msg.Write(sender);
            msg.Write(receiver);
            msg.Write(relation);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }

        #endregion

        #region Army (2048-3071)
        [Command(2048)]
        public static void RecruitUnit(NetIncomingMessage message) {
            int unitInfo = message.ReadInt32();
            int country = message.ReadInt32();
            int province = message.ReadInt32();
            int count = message.ReadInt32();

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)2048);
            msg.Write(unitInfo);
            msg.Write(country);
            msg.Write(province);
            msg.Write(count);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }

        [Command(2049)]
        public static void GenerateArmyRoute(NetIncomingMessage message) {
            int id = message.ReadInt32();
            int target = message.ReadInt32();

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)2049);
            msg.Write(id);
            msg.Write(target);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }

        [Command(2050)]
        public static void AddUnit(NetIncomingMessage message) {
            int army = message.ReadInt32();
            int unit = message.ReadInt32();
            int count = message.ReadInt32();
            int maxCount = message.ReadInt32();

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)2050);
            msg.Write(army);
            msg.Write(unit);
            msg.Write(count);
            msg.Write(maxCount);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }

        [Command(2051)]
        public static void RemoveUnit(NetIncomingMessage message) {
            int army = message.ReadInt32();
            int unit = message.ReadInt32();
            int count = message.ReadInt32();

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)2051);
            msg.Write(army);
            msg.Write(unit);
            msg.Write(count);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }

        [Command(2052)]
        public static void DeleteArmy(NetIncomingMessage message) {
            int id = message.ReadInt32();

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)2052);
            msg.Write(id);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }

        #endregion
    }
}
