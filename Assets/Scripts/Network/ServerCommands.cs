using EuropeanWars.Core;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Province;
using EuropeanWars.UI.Lobby;
using Lidgren.Network;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

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
            Core.Time.Timer.Singleton.speed = 0;

            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)258);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }

        #endregion

        [Command(512)]
        public static void BuildBuildingRequest(NetIncomingMessage message) {
            int building = message.ReadInt32();
            int province = message.ReadInt32();
            int slot = message.ReadInt32();
            BuildingInfo b = GameInfo.buildings[building];
            ProvinceInfo p = GameInfo.provinces[province];

            if (p.Country.id == Server.Singleton.clients[message.SenderConnection].countryId && b.CanBuildInProvince(p)) {
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

            if (p.Country.id == Server.Singleton.clients[message.SenderConnection].countryId) {
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

            if (p.Country.id == Server.Singleton.clients[message.SenderConnection].countryId) {
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
    }
}
