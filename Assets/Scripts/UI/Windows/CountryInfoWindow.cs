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

        public Button makeVassal;
        public Button deleteVassal;

        public Button makeMarchy;
        public Button deleteMarchy;
        public Button annexVassal;

        public CountryInfo country;

        public void UpdateLanguage() {
            declareWarWindow.declareButton.GetComponentInChildren<Text>().text = LanguageDictionary.language["DeclareWar"];
            declareWarButton.GetComponent<DescriptionText>().text = LanguageDictionary.language["DeclareWar"];
            peaceWarButton.GetComponent<DescriptionText>().text = LanguageDictionary.language["PeaceDeal"];

            foreach (var item in dipActionButtons) {
                item.GetComponent<DescriptionText>().text = 
                    (item.targetState ? LanguageDictionary.language["CreateRelation"] : LanguageDictionary.language["DeleteRelation"]) +
                    " " + LanguageDictionary.language[item.action.ToString()];
            }
        }

        public void UpdateWindow(CountryInfo country) {
            declareWarWindow.ResetAndDisable();
            this.country = country;
            crest.sprite = country.Crest;
            //religion.sprite = country.religion.image;
            //king.sprite = country.king.image; TODO: uncomment this.
            countryName.text = country.Name;

            UpdateWarActionButtons();
            UpdateDiplomaticRelations();
            UpdateVassalActionButtons();
        }

        public void UpdateWarActionButtons() {
            declareWarButton.interactable = GameInfo.PlayerCountry.sovereign && !GameInfo.PlayerCountry.IsInWarAgainstCountry(country) 
                && country != GameInfo.PlayerCountry && GameInfo.PlayerCountry.relations[country].truceInMonths == 0;
            peaceWarButton.interactable = GameInfo.PlayerCountry.IsInWarAgainstCountry(country) && country != GameInfo.PlayerCountry;
        }
        public void UpdateDiplomaticRelations() {
            if (country == null) {
                return;
            }

            foreach (var item in dipActionButtons) {
                Button button = item.GetComponent<Button>();
                if (!GameInfo.PlayerCountry.sovereign || !country.sovereign || country == GameInfo.PlayerCountry
                    || country.IsInWarAgainstCountry(GameInfo.PlayerCountry)) {
                    button.interactable = false;
                    continue;
                }

                button.interactable = GameInfo.PlayerCountry.relations[country].CanChangeRelationStateTo(item.action, item.targetState);
            }
        }
        public void UpdateVassalActionButtons() {
            makeVassal.interactable = GameInfo.PlayerCountry.CanMakeVassal(country);
            deleteVassal.interactable = GameInfo.PlayerCountry.vassals.Contains(country);

            makeMarchy.interactable = country.suzerain == GameInfo.PlayerCountry && !country.isMarchy;
            deleteMarchy.interactable = country.suzerain == GameInfo.PlayerCountry && country.isMarchy;
            annexVassal.interactable = country.suzerain == GameInfo.PlayerCountry && country.CanAnnexVassal(country);
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

        public void MakeVassal() {
            GameInfo.PlayerCountry.MakeVassal(country);
            UpdateVassalActionButtons();
        }

        public void RemoveVassal() {
            GameInfo.PlayerCountry.RemoveVassal(country);
            UpdateVassalActionButtons();
        }

        public void MakeMarchy() {
            GameInfo.PlayerCountry.MakeMarchy(country);
            UpdateVassalActionButtons();
        }
        public void RemoveMarchy() {
            GameInfo.PlayerCountry.RemoveMarchy(country);
            UpdateVassalActionButtons();
        }

        public void AnnexVassal() {
            GameInfo.PlayerCountry.AnnexVassal(country);
            UpdateVassalActionButtons();
        }
    }
}
