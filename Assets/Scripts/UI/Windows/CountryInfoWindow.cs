using EuropeanWars.Core;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Language;
using EuropeanWars.Core.War;
using EuropeanWars.Network;
using Lidgren.Network;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class CountryInfoWindow : MonoBehaviour {
        public Image crest;
        public Image religion;
        public Image king;
        public Text countryName;

        public DiplomaticRelationButton[] dipActionButtons;
        public Button declareWarButton;
        public Button peaceWarButton;

        public DeclareWarWindow declareWarWindow;

        public CountryInfo country;

        public void UpdateLanguage() {
            declareWarWindow.declareButton.GetComponentInChildren<Text>().text = LanguageDictionary.language["DeclareWar"];
        }

        public void UpdateWindow(CountryInfo country) {
            declareWarWindow.ResetAndDisable();
            this.country = country;
            crest.sprite = country.crest;
            //religion.sprite = country.religion.image;
            //king.sprite = country.king.image; TODO: uncomment this.
            countryName.text = country.name;

            UpdateWarActionButtons();
            UpdateDiplomaticRelations();
        }

        public void UpdateWarActionButtons() {
            declareWarButton.interactable = !GameInfo.PlayerCountry.IsInWarAgainstCountry(country) 
                && country != GameInfo.PlayerCountry && GameInfo.PlayerCountry.relations[country].truceInMonths == 0;
            peaceWarButton.interactable = GameInfo.PlayerCountry.IsInWarAgainstCountry(country) && country != GameInfo.PlayerCountry;
        }

        public void UpdateDiplomaticRelations() {
            if (country == null) {
                return;
            }

            foreach (var item in dipActionButtons) {
                Button button = item.GetComponent<Button>();
                if (country == GameInfo.PlayerCountry || country.IsInWarAgainstCountry(GameInfo.PlayerCountry)) {
                    button.interactable = false;
                    continue;
                }

                button.interactable = GameInfo.PlayerCountry.relations[country].CanChangeRelationStateTo(item.action, item.targetState);
            }
        }

        public void TryChangeRelationState(DiplomaticRelation relation, bool targetState) {
            if (GameInfo.PlayerCountry != country && GameInfo.PlayerCountry.relations[country].CanChangeRelationStateTo(relation, targetState)) {
                GameInfo.PlayerCountry.relations[country].TryChangeRelationState(relation, GameInfo.PlayerCountry, country);
            }
        }

        public void DeclareWar() {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)1035);
            msg.Write(GameInfo.PlayerCountry.id);
            msg.Write(country.id);
            msg.Write(declareWarWindow.selectedReason.warReasonId);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);

            declareWarWindow.ResetAndDisable();
        }

        public void PeaceWar() {
            if (GameInfo.PlayerCountry.IsInWarAgainstCountry(country) && country != GameInfo.PlayerCountry) {
                WarInfo war = GameInfo.PlayerCountry.GetWarAgainstCountry(country);
                PeaceDealWindow.Singleton.CreatePeaceDeal(
                    war, GameInfo.PlayerCountry.wars[war], country.wars[war]);
            }
        }
    }
}
