using EuropeanWars.Core;
using EuropeanWars.Core.Country;
using EuropeanWars.Network;
using Lidgren.Network;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Lobby {
    public class LobbyManager : MonoBehaviour {
        public static LobbyManager Singleton { get; private set; }

        public Image crest;
        public Text countryName;
        public Image readyIcon;

        public Button readyButton;
        public Button playButton;

        private CountryInfo selectedCountry;
        private bool isReady = false;

        public void Awake() {
            Singleton = this;
        }

        public void Start() {
            readyIcon.color = Color.red;

        }

        public void SelectCountry(CountryInfo c) {
            if (isReady) {
                return;
            }
            readyButton.interactable = !c.isPlayer;
            selectedCountry = c;
            countryName.text = c.Name;
            crest.sprite = c.Crest;
        }

        public void Ready() {
            if (selectedCountry != null) {
                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)128);
                msg.Write(selectedCountry.id);
                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void SetReady(int countryId, bool b) {
            isReady = b;
            readyIcon.color = b ? Color.green : Color.red;
            GameInfo.SetPlayerCountry(GameInfo.countries[countryId]);
            readyButton.interactable = true;
        }

        public void UpdateReadyButton(int countryId) {
            if (selectedCountry != null && selectedCountry.id == countryId) {
                readyButton.interactable = !selectedCountry.isPlayer;
            }
        }

        public void Play() {
            NetOutgoingMessage msg = Server.Singleton.s.CreateMessage();
            msg.Write((ushort)130);
            Server.Singleton.s.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
