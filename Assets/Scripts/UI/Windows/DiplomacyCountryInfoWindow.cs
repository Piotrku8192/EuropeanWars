using Boo.Lang;
using EuropeanWars.Core;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class DiplomacyCountryInfoWindow : MonoBehaviour {
        public Image crest;
        public Image religion;
        public Image king;
        public Text countryName;

        public DipActionButton[] dipActionButtons;

        public CountryInfo country;

        public void UpdateWindow(CountryInfo country) {
            this.country = country;
            crest.sprite = country.crest;
            religion.sprite = country.religion.image;
            //king.sprite = country.king.image; TODO: uncomment this.
            countryName.text = country.name;

            UpdateDipActions();
        }

        public void UpdateDipActions() {
            if (country == null) {
                return;
            }

            foreach (var item in dipActionButtons) {
                Button button = item.GetComponent<Button>();
                if (country == GameInfo.PlayerCountry) {
                    button.interactable = false;
                    continue;
                }

                switch (item.action) {
                    case DiplomacyAction.CreateAlliance:
                        button.interactable = Alliance.CanCreate(GameInfo.PlayerCountry, country);
                        break;
                    case DiplomacyAction.DeleteAlliance:
                        button.interactable = Alliance.CanDelete(GameInfo.PlayerCountry, country);
                        break;
                    case DiplomacyAction.CreateMilitaryAccess:
                        button.interactable = MilitaryAccess.CanCreate(GameInfo.PlayerCountry, country);
                        break;
                    case DiplomacyAction.DeleteMilitaryAccess:
                        button.interactable = MilitaryAccess.CanDelete(GameInfo.PlayerCountry, country);
                        break;
                    default:
                        break;
                }
            }
        }

        public void PlayAction(DiplomacyAction action) {
            switch (action) {
                case DiplomacyAction.CreateAlliance:
                    Alliance.AllianceRequest(GameInfo.PlayerCountry, country);
                    break;
                case DiplomacyAction.DeleteAlliance:
                    Alliance.DeleteAlliance(GameInfo.PlayerCountry.alliances[country]);
                    break;
                case DiplomacyAction.CreateMilitaryAccess:
                    MilitaryAccess.AccessRequest(GameInfo.PlayerCountry, country);
                    break;
                case DiplomacyAction.DeleteMilitaryAccess:
                    MilitaryAccess.DeleteAccess(GameInfo.PlayerCountry.militaryAccesses[country]);
                    break;
                default:
                    break;
            }
            UpdateDipActions();
        }
    }
}
