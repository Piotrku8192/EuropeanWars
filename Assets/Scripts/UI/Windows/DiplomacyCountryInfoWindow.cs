using Boo.Lang;
using EuropeanWars.Core;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy_Old;
using EuropeanWars.Core.Language;
using EuropeanWars.Network;
using Lidgren.Network;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class DiplomacyCountryInfoWindow : MonoBehaviour {
        public Image crest;
        public Image religion;
        public Image king;
        public Text countryName;

        public DipActionButton[] dipActionButtons;
        public DeclareWarWindow declareWarWindow;

        public CountryInfo country;

        public void UpdateLanguage() {
            foreach (var item in dipActionButtons) {
                Text t = item.GetComponentInChildren<Text>();
                switch (item.action) {
                    case DiplomacyAction.CreateAlliance:
                        t.text = LanguageDictionary.language["MakeAlliance"];
                        break;
                    case DiplomacyAction.DeleteAlliance:
                        t.text = LanguageDictionary.language["DeleteAlliance"];
                        break;
                    case DiplomacyAction.CreateMilitaryAccess:
                        t.text = LanguageDictionary.language["MillitaryAccess"];
                        break;
                    case DiplomacyAction.DeleteMilitaryAccess:
                        t.text = LanguageDictionary.language["DeleteMillitaryAccess"];
                        break;
                    case DiplomacyAction.DeclareWar:
                        t.text = LanguageDictionary.language["DeclareWar"];
                        break;
                    default:
                        break;
                }
            }
        }

        public void UpdateWindow(CountryInfo country) {
            declareWarWindow.ResetAndDisable();
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
                    button.gameObject.SetActive(false);
                    continue;
                }

                switch (item.action) {
                    case DiplomacyAction.DeclareWar:
                        button.gameObject.SetActive(!GameInfo.PlayerCountry.IsInWarAgainstCountry(country));
                        break;
                    case DiplomacyAction.CreateAlliance:
                        button.gameObject.SetActive(Alliance.CanCreate(GameInfo.PlayerCountry, country));
                        break;
                    case DiplomacyAction.DeleteAlliance:
                        button.gameObject.SetActive(Alliance.CanDelete(GameInfo.PlayerCountry, country));
                        break;
                    case DiplomacyAction.CreateMilitaryAccess:
                        button.gameObject.SetActive(MilitaryAccess.CanCreate(GameInfo.PlayerCountry, country));
                        break;
                    case DiplomacyAction.DeleteMilitaryAccess:
                        button.gameObject.SetActive(MilitaryAccess.CanDelete(GameInfo.PlayerCountry, country));
                        break;
                    default:
                        break;
                }
            }
        }

        public void PlayAction(DiplomacyAction action) {
            switch (action) {
                case DiplomacyAction.DeclareWar:
                    NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                    msg.Write((ushort)1035);
                    msg.Write(GameInfo.PlayerCountry.id);
                    msg.Write(country.id);
                    msg.Write(declareWarWindow.selectedReason.warReasonId);
                    Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                    break;
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
