using EuropeanWars.Core;
using EuropeanWars.Core.Army;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Province;
using EuropeanWars.Network;
using Lidgren.Network;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class ProvinceWindow : MonoBehaviour
    {
        public static ProvinceWindow Singleton { get; private set; }

        public GameObject provinceWindow;

        public Text provinceName;
        public CountryButton countryCrest;
        public Image religion;
        public Image terrainImage;

        public Text tax;
        public Text buildingsIncome;
        public Text tradeIncome;
        public Text culture;
        public Text garnison;
        public Transform claimersContent;
        public List<CountryButton> claimators = new List<CountryButton>();

        public Button upgradeButton;
        public Button devastateButton;
        public Button fabricateClaimButton;

        public BuildingButton[] buildings;
        public BuildingBuilder builder;
        public DeleteBuildingWindow deleteBuildingWindowPrefab;

        public GameObject occupationWindow;
        public Text occupationProgress;
        public Image occupationProgressBar;
        public Image occupantCrest;
        public Text occupantArmySize;
        public Text garnisonSize;

        public ProvinceInfo province;

        public int selectedBuildingSlot;

        public void Awake() {
            Singleton = this;
        }

        public void UpdateWindow(ProvinceInfo province, bool activateWindow = false) {
            if (ArmyInfo.selectedArmies.Count > 0) {
                GameInfo.UnselectProvince();
                return;
            }

            if (provinceWindow.activeInHierarchy || activateWindow) {
                UIManager.Singleton.CloseAllWindowsProvince();
                provinceWindow.SetActive(true);
                this.province = province;
                provinceName.text = province.name;
                countryCrest.SetCountry(province.Country);
                religion.sprite = province.religion.image;
                //TODO: Add: terrainImage = ...
                tax.text = province.taxation.ToString();
                buildingsIncome.text = province.buildingsIncome.ToString();
                tradeIncome.text = province.tradeIncome.ToString();
                culture.text = province.culture.name;
                garnison.text = province.fogOfWar ? "?" : province.garnison.Sum(t => t.Value).ToString();

                foreach (var item in claimators) {
                    Destroy(item.gameObject);
                }
                claimators.Clear();
                foreach (var item in province.claimators) {
                    CountryButton clmtr = Instantiate(countryCrest, claimersContent);
                    clmtr.SetCountry(item);
                    claimators.Add(clmtr);
                }

                for (int i = 0; i < 10; i++) {
                    buildings[i].UpdateButton(province.buildings[i]);
                }
                if (activateWindow) {
                    builder.SetWindowActive(false);
                }

                bool b = GameInfo.PlayerCountry == province.Country;
                upgradeButton.interactable = b;
                devastateButton.interactable = b;
                fabricateClaimButton.interactable = province.isInteractive && !province.claimators.Contains(GameInfo.PlayerCountry) && !GameInfo.PlayerCountry.toClaim.ContainsKey(province)
                    && province.neighbours.Where(t => t.NationalCountry == GameInfo.PlayerCountry).Any();

                if (!province.fogOfWar && province.OccupationCounter.Army != null) {
                    occupationWindow.SetActive(true);
                }
            }
        }

        public void Update() {
            if (province != null) {
                if (!province.fogOfWar && province.OccupationCounter.Army != null) {
                    occupationProgress.text = Mathf.RoundToInt(province.OccupationCounter.Progress) + "%";
                    occupationProgressBar.fillAmount = province.OccupationCounter.Progress * 0.01f;
                    occupantCrest.sprite = province.OccupationCounter.Army.Country.crest;
                    garnisonSize.text = province.garnison.Sum(t => t.Value).ToString();
                    occupantArmySize.text = province.OccupationCounter.Army.Size.ToString();
                }
                else {
                    occupationWindow.SetActive(false);
                }
            }
        }

        public void SelectBuildingSlot(int id) {
            if (province.Country == GameInfo.PlayerCountry) {
                selectedBuildingSlot = id;
                builder.SetWindowActive(true);
            }
        }

        public void BuildBuildingInSlot(BuildingInfo building) {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)512);
            msg.Write(building.id);
            msg.Write(province.id);
            msg.Write(selectedBuildingSlot);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);

            builder.SetWindowActive(false);
        }

        public void UpgradeProvince() {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)513);
            msg.Write(province.id);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        public void DevastateProvince() {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)514);
            msg.Write(province.id);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        public void FabricateClaim() {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)1032);
            msg.Write(province.id);
            msg.Write(GameInfo.PlayerCountry.id);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
