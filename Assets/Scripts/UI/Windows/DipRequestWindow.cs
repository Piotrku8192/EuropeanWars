using EuropeanWars.Core.Country;
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

        public void Init(bool isNotification = false) {
            this.isNotification = isNotification;
            senderCrest.SetCountry(c1);
            receiverCrest.SetCountry(c2);
            //TODO: Fill title and description with translated content.
        }

        public void Accept() {
            onAccept?.Invoke();
            Destroy(gameObject);
        }

        public void Delice() {
            onDelice?.Invoke();
            Destroy(gameObject);
        }
    }
}
