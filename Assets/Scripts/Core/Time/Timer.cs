using EuropeanWars.Network;
using Lidgren.Network;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.Core.Time {
    public class Timer : MonoBehaviour
    {
        public static Timer Singleton { get; private set; }
        public Text date;
        public Button[] buttons = new Button[6];
        public int speed = 0;

        private float time;

        public void Awake() {
            Singleton = this;
        }

        public void Update() {
            if (Server.Singleton == null) {
                return;
            }

            if (speed > 0) {
                if (time >= 1) {
                    NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
                    msg.Write((ushort)256);
                    Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);

                    time = 0;
                }

                time += UnityEngine.Time.deltaTime * speed;
            }
        }

        public void PauseRequest() {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)256);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        public void ClientPause() {
            buttons[0].image.color = Color.cyan;
            for (int i = 1; i < buttons.Length; i++) {
                buttons[i].image.color = Color.white;
            }
        }

        public void ServerSetSpeed(int s) {
            if (Server.Singleton != null) {
                speed = s;
                NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
                msg.Write((ushort)257);
                msg.Write(s);
                Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void ClientSetSpeed(int speed) {
            buttons[0].image.color = Color.white;
            for (int i = 1; i < buttons.Length; i++) {
                buttons[i].image.color = i > speed ? Color.white : Color.green;
            }
        }
    }
}
