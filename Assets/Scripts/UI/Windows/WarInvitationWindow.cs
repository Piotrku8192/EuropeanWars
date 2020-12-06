using EuropeanWars.Core;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Language;
using EuropeanWars.Core.War;
using EuropeanWars.Network;
using Lidgren.Network;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.UI.Windows {
    public class WarInvitationWindow : MonoBehaviour {
        public CountryButton senderCrest;
        public CountryButton receiverCrest;
        public Text title;
        public Text description;
        public Text acceptText;
        public Text deliceText;

        private WarInfo war;
        private CountryInfo inviter;
        private bool isAttacker;

        public void Init(WarInfo war, CountryInfo inviter, bool isAttacker) {
            senderCrest.SetCountry(inviter);
            receiverCrest.SetCountry(GameInfo.PlayerCountry);
            this.war = war;
            this.inviter = inviter;
            this.isAttacker = isAttacker;
            title.text = LanguageDictionary.language["War"];
            description.text = string.Format(LanguageDictionary.language["WarInvitation"], inviter.Name);
            acceptText.text = LanguageDictionary.language["Accept"];
            deliceText.text = LanguageDictionary.language["Delice"];
        }

        public void Accept() {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)1033);
            msg.Write(war.id);
            msg.Write(GameInfo.PlayerCountry.id);
            msg.Write(isAttacker);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);

            Destroy(gameObject);
        }

        public void Delice() {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)1034);
            msg.Write(inviter.id);
            msg.Write(GameInfo.PlayerCountry.id);
            msg.Write(isAttacker);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);

            Destroy(gameObject);
        }
    }
}
