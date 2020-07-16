using EuropeanWars.Core.Diplomacy;
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

        public DiplomaticRelation relation;
        public bool isNotification;

        public delegate void OnAccept();
        public delegate void OnDelice();
        public OnAccept onAccept;
        public OnDelice onDelice;

        public void Init(bool isNotification = false) {
            this.isNotification = isNotification;
            senderCrest.SetCountry(relation.countries[0]);
            receiverCrest.SetCountry(relation.countries[1]);
            //TODO: Fill title and description with translated content.
        }

        public void Accept() {
            if (!isNotification) {
                DiplomacyManager.AcceptRelation(relation);
            }
            onAccept?.Invoke();
            Destroy(gameObject);
        }

        public void Delice() {
            if (!isNotification) {
                DiplomacyManager.DeliceRelation(relation);
            }
            onDelice?.Invoke();
            Destroy(gameObject);
        }
    }
}
