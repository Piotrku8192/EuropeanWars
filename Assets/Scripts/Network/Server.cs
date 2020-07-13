using Lidgren.Network;
using System.Collections.Generic;
using UnityEngine;

namespace EuropeanWars.Network {
    public class Server : MonoBehaviour
    {
        public static Server Singleton { get; private set; }
        public NetServer s;
        public int serverPort = 7777;

        private bool created = false;

        public Dictionary<NetConnection, ClientInfo> clients = new Dictionary<NetConnection, ClientInfo>();

        public void Awake() {
            Singleton = this;
            if (!created) {
                DontDestroyOnLoad(gameObject);
                created = true;
            }
        }

        public void OnDestroy() {
            s.Shutdown("");
        }

        public void OnApplicationQuit() {
            s.Shutdown("");
        }

        public void Start() {
            NetPeerConfiguration config = new NetPeerConfiguration("ew");
            config.Port = serverPort;
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            s = new NetServer(config);
            s.Start();
        }

        public void Update() {
            NetIncomingMessage message;
            while (s.ReadMessage(out message)) {
                ushort command = message.ReadUInt16();
                if (message.MessageType == NetIncomingMessageType.ConnectionApproval) {
                    if (command == 0) {
                        message.SenderConnection.Approve();
                    }
                    else {
                        message.SenderConnection.Deny();
                    }
                }
                ServerCommands.InvokeCommandById(command, message);
                s.Recycle(message);
            }
        }
    }
}
