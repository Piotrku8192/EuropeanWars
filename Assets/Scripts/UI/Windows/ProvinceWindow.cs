using EuropeanWars.Core;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Province;
using EuropeanWars.Network;
using Lidgren.Network;
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

        public Button upgradeButton;
        public Button devastateButton;

        public BuildingButton[] buildings;
        public BuildingBuilder builder;
        public DeleteBuildingWindow deleteBuildingWindowPrefab;

        public ProvinceInfo province;

        public int selectedBuildingSlot;

        public void Awake() {
            Singleton = this;
        }

        public void UpdateWindow(ProvinceInfo province) {
            UIManager.Singleton.CloseAllWindows();
            provinceWindow.SetActive(true);
            this.province = province;
            provinceName.text = province.name;
            countryCrest.SetCountry(province.Country);
            religion.sprite = province.religion.image;
            //terrainImage = ...
            tax.text = province.taxation.ToString();
            buildingsIncome.text = province.buildingsIncome.ToString();
            tradeIncome.text = province.tradeIncome.ToString();
            culture.text = province.culture.name;
            garnison.text = province.garnison.ToString();

            for (int i = 0; i < 10; i++) {
                buildings[i].UpdateButton(province.buildings[i]);
            }
            builder.SetWindowActive(false);

            bool b = GameInfo.PlayerCountry == province.Country;
            upgradeButton.interactable = b;
            devastateButton.interactable = b;
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
    }
}
