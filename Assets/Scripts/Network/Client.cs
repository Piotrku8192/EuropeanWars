using Lidgren.Network;
using System.Net;
using UnityEngine;

namespace EuropeanWars.Network {
    public class Client : MonoBehaviour
    {
        public static Client Singleton { get; private set; }
        public NetClient c;
        public string serverAddress;
        public int serverPort = 7777;
        public bool isHost;

        private bool created = false;

        public void Awake() {
            Singleton = this;

            if (!created) {
                DontDestroyOnLoad(gameObject);
                created = true;
            }
        }

        public void OnDestroy() {
            c.Shutdown("Bye");
        }

        public void Start() {
            isHost = Server.Singleton != null;

            NetPeerConfiguration config = new NetPeerConfiguration("ew");
            config.Port = Random.Range(2048, 30000);
            c = new NetClient(config);
            c.Start();
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
            NetOutgoingMessage message = c.CreateMessage();
            message.Write(0);
            message.Write("Here is space for player authoryzation info.");
            c.Connect(point, message);
        }

        public void Update() {
            NetIncomingMessage message;
            while (c.ReadMessage(out message)) {
                ushort command = message.ReadUInt16();
                ClientCommands.InvokeCommandById(command, message);
                c.Recycle(message);
            }
        }
    }
}
