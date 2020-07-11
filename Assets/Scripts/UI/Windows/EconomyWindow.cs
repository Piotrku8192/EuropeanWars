using EuropeanWars.Core;
using EuropeanWars.Network;
using Lidgren.Network;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class EconomyWindow : MonoBehaviour
    {
        public static EconomyWindow Singleton { get; private set; }

        public GameObject window;

        public Text taxationValue;
        public Text taxationModifier;
        public Text buildingsValue;
        public Text buildingsModifier;
        public Text tradeValue;
        public Text tradeModifier;

        public Text armyValue;
        public Text armyModifier;
        public Text defenceValue;
        public Text defenceModifier;
        public Text reparationsValue;
        public Text reparationsModifier;
        
        public Text loans;
        public Text balance;

        public void Awake() {
            Singleton = this;
        }

        public void Update() {
            if (window.activeInHierarchy) {
                UpdateWindow();
            }
        }

        public void OpenWindow() {
            UIManager.Singleton.CloseAllWindows();
            window.SetActive(true);
        }

        public void UpdateWindow() {
            taxationValue.text = GameInfo.PlayerCountry.taxationIncome.ToString();
            taxationModifier.text = (GameInfo.PlayerCountry.taxationIncomeModifier * 100).ToString() + "%";
            buildingsValue.text = GameInfo.PlayerCountry.buildingsIncome.ToString();
            buildingsModifier.text = (GameInfo.PlayerCountry.buildingsIncomeModifier * 100).ToString() + "%";
            tradeValue.text = GameInfo.PlayerCountry.tradeIncome.ToString();
            tradeModifier.text = (GameInfo.PlayerCountry.tradeIncomeModifier * 100).ToString() + "%";
            armyValue.text = GameInfo.PlayerCountry.armyMaintenance.ToString();
            armyModifier.text = (GameInfo.PlayerCountry.armyMaintenanceModifier * 100).ToString() + "%";

            loans.text = GameInfo.PlayerCountry.loans.ToString();
            balance.text = GameInfo.PlayerCountry.balance.ToString();
            balance.color = GameInfo.PlayerCountry.balance < 0 ? Color.red : Color.green;
        }

        public void TakeLoan() {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)515);
            msg.Write(GameInfo.PlayerCountry.id);
            msg.Write(1);

            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        public void PayOffLoan() {
            if (GameInfo.PlayerCountry.loans > 0) {
                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)516);
                msg.Write(GameInfo.PlayerCountry.id);
                msg.Write(1);

                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void Bankruptcy() {
            if (!GameInfo.PlayerCountry.isBankruptcy) {
                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)517);
                msg.Write(GameInfo.PlayerCountry.id);

                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
            }
        }
    }
}
