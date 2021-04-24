using EuropeanWars.Core;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Language;
using EuropeanWars.Core.Persons;
using EuropeanWars.Core.War;
using EuropeanWars.Network;
using Lidgren.Network;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class CountryInfoWindow : MonoBehaviour {
        public Image crest;
        public Image religion;
        public Image ruler;
        public Text countryName;
        public RelationPointsImage relationPoints;

        public DiplomaticRelationButton[] dipActionButtons;
        public Button declareWarButton;
        public Button peaceWarButton;

        public DeclareWarWindow declareWarWindow;

        public Button makeVassal;
        public Button deleteVassal;

        public Button makeMarchy;
        public Button deleteMarchy;
        public Button annexVassal;

        public PersonButton diplomatButton;
        public PersonButton spyButton;

        public ChoosePersonWindow choosePersonWindow;

        public Text armySize;

        public GameObject spyNetworkUI;
        public Text spyNetwork;
        public Image spyNetworkProgress;
        public Image[] spyNetworkLevels;

        public CountryInfo country;
        private CountryRelation relation;

        public void UpdateLanguage() {
            declareWarWindow.declareButton.GetComponentInChildren<Text>().text = LanguageDictionary.language["DeclareWar"];
            declareWarButton.GetComponent<DescriptionText>().text = LanguageDictionary.language["DeclareWar"];
            peaceWarButton.GetComponent<DescriptionText>().text = LanguageDictionary.language["PeaceDeal"];

            foreach (var item in dipActionButtons) {
                item.GetComponent<DescriptionText>().text = 
                    (item.targetState ? LanguageDictionary.language["CreateRelation"] : LanguageDictionary.language["DeleteRelation"]) +
                    " " + LanguageDictionary.language[item.action.ToString()];
            }

            makeVassal.GetComponent<DescriptionText>().text = LanguageDictionary.language["MakeVassal"];
            deleteVassal.GetComponent<DescriptionText>().text = LanguageDictionary.language["DeleteVassal"];
            makeMarchy.GetComponent<DescriptionText>().text = LanguageDictionary.language["MakeMarchy"];
            deleteMarchy.GetComponent<DescriptionText>().text = LanguageDictionary.language["DeleteMarchy"];
            annexVassal.GetComponent<DescriptionText>().text = LanguageDictionary.language["AnnexVassal"];
        }

        public void UpdateWindow(CountryInfo country) {
            declareWarWindow.ResetAndDisable();
            this.country = country;
            crest.sprite = country.Crest;
            religion.sprite = country.religion.image;
            //king.sprite = country.king.image; TODO: uncomment this.
            countryName.text = country.Name;
            choosePersonWindow.gameObject.SetActive(false);

            if (GameInfo.PlayerCountry.relations.ContainsKey(country)) {
                relation = GameInfo.PlayerCountry.relations[country];
                relationPoints.gameObject.SetActive(true);
                relationPoints.SetRelation(GameInfo.PlayerCountry.relations[country]);

                diplomatButton.gameObject.SetActive(true);
                spyButton.gameObject.SetActive(true);
                diplomatButton.SetPerson(GameInfo.PlayerCountry.GetDiplomatInRelation(relation));
                spyButton.SetPerson(GameInfo.PlayerCountry.GetSpyInRelation(relation));

                int spyNet = GameInfo.PlayerCountry.spyNetworks[country];
                spyNetwork.text = spyNet.ToString();
                spyNetworkProgress.fillAmount = spyNet / 100.0f;

                for (int i = 0; i < spyNetworkLevels.Length; i++) {
                    spyNetworkLevels[i].color = spyNet >= (i + 1) * (1.0f / spyNetworkLevels.Length) * 100 ? new Color(255, 176, 0) : Color.gray;
                    spyNetworkLevels[i].GetComponent<DescriptionText>().text = LanguageDictionary.language["SpyNetworkLevelDescription-" + i.ToString()];
                }
                spyNetworkUI.SetActive(true);

                armySize.gameObject.SetActive(GameInfo.PlayerCountry.spyNetworks[country] >= 60);
                armySize.text = country.armies.Sum(t => t.Size).ToString();
            }
            else {
                relationPoints.gameObject.SetActive(false);
                diplomatButton.gameObject.SetActive(false);
                spyButton.gameObject.SetActive(false);
                spyNetworkUI.SetActive(false);
                armySize.gameObject.SetActive(false);
            }

            UpdateWarActionButtons();
            UpdateDiplomaticRelations();
            UpdateVassalActionButtons();
        }

        public void UpdateWarActionButtons() {
            declareWarButton.interactable = GameInfo.PlayerCountry.sovereign && !GameInfo.PlayerCountry.IsInWarAgainstCountry(country) 
                && country != GameInfo.PlayerCountry && GameInfo.PlayerCountry.relations[country].truceInMonths == 0;
            WarInfo w = country.GetWarAgainstCountry(GameInfo.PlayerCountry);
            peaceWarButton.interactable = GameInfo.PlayerCountry.IsInWarAgainstCountry(country) && country != GameInfo.PlayerCountry
                && PeaceDeal.CanMakePeaceDeal(country.GetWarAgainstCountry(GameInfo.PlayerCountry), GameInfo.PlayerCountry.wars[w], country.wars[w]);
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

        public void ChangeDiplomat() {
            choosePersonWindow.Initialize(new ChoosePersonWindow.ChoosePerson(SetDiplomat),
                GameInfo.PlayerCountry.diplomats.Where(t => t.CurrentlyImprovingRelation != GameInfo.PlayerCountry.relations[country]).ToArray());
            choosePersonWindow.gameObject.SetActive(true);
        }
        public void ChangeSpy() {
            choosePersonWindow.Initialize(new ChoosePersonWindow.ChoosePerson(SetSpy), 
                GameInfo.PlayerCountry.spies.Where(t => t.CurrentlyBuildingSpyNetwork != GameInfo.PlayerCountry.relations[country]).ToArray());
            choosePersonWindow.gameObject.SetActive(true);
        }
        public void DiscardDiplomat() {
            Diplomat diplomat = (Diplomat)diplomatButton.Person;
            if (diplomat != null) {
                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)1042);
                msg.Write(-1);
                msg.Write(-1);
                msg.Write(diplomat.id);
                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
            }
            diplomatButton.SetPerson(null);
        }
        public void DiscardSpy() {
            Spy spy = (Spy)spyButton.Person;
            if (spy != null) {
                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)1043);
                msg.Write(-1);
                msg.Write(-1);
                msg.Write(spy.id);
                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
            }
            spyButton.SetPerson(null);
        }
        public void SetDiplomat(Person person) {
            Diplomat diplomat = (Diplomat)person;
            if (diplomat.country == GameInfo.PlayerCountry) {
                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)1042);
                msg.Write(person.country.id);
                msg.Write(country.id);
                msg.Write(person.id);
                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                diplomatButton.SetPerson(diplomat);
            }
        }
        public void SetSpy(Person person) {
            Spy spy = (Spy)person;
            if (spy.country == GameInfo.PlayerCountry) {
                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)1043);
                msg.Write(person.country.id);
                msg.Write(country.id);
                msg.Write(person.id);
                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                spyButton.SetPerson(spy);
            }
        }
    }
}
