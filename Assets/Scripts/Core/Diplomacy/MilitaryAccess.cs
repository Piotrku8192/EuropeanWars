using EuropeanWars.Core.Country;
using EuropeanWars.Network;
using EuropeanWars.UI.Windows;
using Lidgren.Network;
using System.Linq;
using UnityEngine.UI;

namespace EuropeanWars.Core.Diplomacy {
    public class MilitaryAccess : DiplomaticRelation {
        public static bool messageSent;

        protected override void OnStart() {
            base.OnStart();
            name = "Sojusz";
        }

        public static bool CanCreate(CountryInfo sender, CountryInfo receiver) =>
            !sender.militaryAccesses.ContainsKey(receiver); //TODO: Add if sender.relations[receiver] >= some value.

        public static bool CanDelete(CountryInfo sender, CountryInfo receiver) =>
            sender.militaryAccesses.ContainsKey(receiver);

        public static void AccessRequest(CountryInfo sender, CountryInfo receiver) {
            if (!CanCreate(sender, receiver) || messageSent) {
                return;
            }

            if (receiver.isPlayer) {
                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)1028);
                msg.Write(sender.id);
                msg.Write(receiver.id);
                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                messageSent = true;
            }
            else {
                AccessRequestClient(sender, receiver);
            }
        }
        public static void AcceptAccess(MilitaryAccess access) {
            if (access.countries.Where(t => t.isPlayer).Any()) {
                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)1029);
                msg.Write(access.countries[0].id);
                msg.Write(access.countries[1].id);
                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                messageSent = true;
            }
            else {
                CreateAccessClient(access);
            }
        }
        public static void DeliceAccess(MilitaryAccess access) {
            if (access.countries.Where(t => t.isPlayer).Any()) {
                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)1030);
                msg.Write(access.countries[0].id);
                msg.Write(access.countries[1].id);
                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                messageSent = true;
            }
        }
        public static void DeleteAccess(MilitaryAccess access) {
            if (!CanDelete(access.countries[0], access.countries[1]) || messageSent) {
                return;
            }

            if (access.countries.Where(t => t.isPlayer).Any()) {
                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)1031);
                msg.Write(access.countries[0].id);
                msg.Write(access.countries[1].id);
                msg.Write(GameInfo.PlayerCountry.id);
                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                messageSent = true;
            }
            else {
                DeleteAccessClient(access);
            }
        }
        public static void AccessRequestClient(CountryInfo sender, CountryInfo receiver) {
            MilitaryAccess a = new MilitaryAccess();
            a.countries.Add(sender);
            a.countries.Add(receiver);

            if (!receiver.isPlayer) {
                //TODO: Add bot request mechanic.
            }
            else if (receiver == GameInfo.PlayerCountry) {
                //TODO: Implement translation
                var win = DiplomacyWindow.Singleton.SpawnRequest(a);
                win.title.text = "Propozycja prawa przemarszu!";
                win.description.text = "Nasi bliscy sąsiedzi proponują nam prawo przemarszu!";
                win.acceptText.text = "Zaakceptuj";
                win.deliceText.text = "Odrzuć";
            }
        }
        public static void CreateAccessClient(MilitaryAccess access) {
            if (access.countries.Count == 2) {
                DiplomacyManager.militaryAccesses.Add(access);
                access.countries[0].militaryAccesses.Add(access.countries[1], access);
                access.countries[1].militaryAccesses.Add(access.countries[0], access);

                if (access.countries[0] == GameInfo.PlayerCountry) {
                    //TODO: Implement translation
                    var win = DiplomacyWindow.Singleton.SpawnRequest(access, true);
                    win.title.text = "Prawo przemarszu!";
                    win.description.text = "Państwo, któremu zaproponowaliśmy sojusz zaakceptowało naszą propozycję!";
                    win.acceptText.text = "Ok";
                    win.deliceText.GetComponentInParent<Button>().gameObject.SetActive(false);

                    DiplomacyWindow.Singleton.countryWindow.UpdateDipActions();
                }

                messageSent = false;
            }
        }
        public static void DeleteAccessClient(MilitaryAccess access) {
            foreach (var item in access.countries) {
                item.alliances.Remove(item.militaryAccesses.Where(t => t.Value == access).FirstOrDefault().Key);
            }
            DiplomacyManager.militaryAccesses.Remove(access);

            DiplomacyWindow.Singleton.countryWindow.UpdateDipActions();
            messageSent = false;
        }
        public static void DeleteAccessClient(MilitaryAccess access, int s) {
            if (access.countries.Contains(GameInfo.PlayerCountry)) {
                if (GameInfo.PlayerCountry.id != s) {
                    //TODO: Implement translation
                    var win = DiplomacyWindow.Singleton.SpawnRequest(access, true);
                    win.title.text = "Koniec prawa przemarszu";
                    win.description.text = "Nasz sojusznik zerwał z nami prawo przemarszu!";
                    win.acceptText.text = "Ok";
                    win.deliceText.GetComponentInParent<Button>().gameObject.SetActive(false);
                }
            }

            foreach (var item in access.countries) {
                item.militaryAccesses.Remove(item.militaryAccesses.Where(t => t.Value == access).FirstOrDefault().Key);
            }
            DiplomacyManager.militaryAccesses.Remove(access);

            DiplomacyWindow.Singleton.countryWindow.UpdateDipActions();
            messageSent = false;
        }
    }
}
