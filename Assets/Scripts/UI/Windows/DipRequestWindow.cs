using EuropeanWars.Core.Country;
using EuropeanWars.Network;
using Lidgren.Network;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class DipRequestWindow : MonoBehaviour {
        public CountryButton senderCrest;
        public CountryButton receiverCrest;
        public Text title;
        public Text description;
        public Text acceptText;
        public Text deliceText;

        public CountryInfo c1;
        public CountryInfo c2;

        public bool isNotification;

        public delegate void OnAccept();
        public delegate void OnDelice();
        public OnAccept onAccept;
        public OnDelice onDelice;

        /// <summary>
        /// Window will send this to server when player click Accept button.
        /// </summary>
        public NetOutgoingMessage acceptMessage;
        /// <summary>
        /// Window will send this to server when player click Delice button.
        /// </summary>
        public NetOutgoingMessage deliceMessage;

        public void Init(bool isNotification = false) {
            this.isNotification = isNotification;
            senderCrest.SetCountry(c1);
            receiverCrest.SetCountry(c2);
            //TODO: Fill title and description with translated content.
        }

        public void Accept() {
            onAccept?.Invoke();
            if (acceptMessage != null) {
                Client.Singleton.c.SendMessage(acceptMessage, NetDeliveryMethod.ReliableOrdered);
            }
            Destroy(gameObject);
        }

        public void Delice() {
            onDelice?.Invoke();
            if (deliceMessage != null) {
                Client.Singleton.c.SendMessage(deliceMessage, NetDeliveryMethod.ReliableOrdered);
            }
            Destroy(gameObject);
        }
    }
}
