using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class DipRequestWindow : MonoBehaviour {
        public Image senderCrest;
        public Image receiverCrest;
        public Text title;
        public Text description;
        public Text acceptText;
        public Text deliceText;

        public DiplomaticRelation relation;

        public void Init() {
            senderCrest.sprite = relation.countries[0].crest;
            receiverCrest.sprite = relation.countries[1].crest;
            //TODO: Fill title and description with translated content.
        }

        public void Accept() {
            DiplomacyManager.CreateRelation(relation);
            Destroy(gameObject);
        }

        public void Delice() {
            Destroy(gameObject);
        }
    }
}
