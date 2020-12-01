using EuropeanWars.Network;
using Lidgren.Network;
using UnityEngine;

namespace EuropeanWars.UI.Windows {
    public class DeleteBuildingWindow : MonoBehaviour {
        public int provinceId;
        public int slotId;

        public void Yes() {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)512);
            msg.Write(0);
            msg.Write(provinceId);
            msg.Write(slotId);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
            Destroy(gameObject);
        }

        public void No() {
            Destroy(gameObject);
        }
    }
}
