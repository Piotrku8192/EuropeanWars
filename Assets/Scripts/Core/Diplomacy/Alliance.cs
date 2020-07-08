using EuropeanWars.Core.Country;
using EuropeanWars.Network;
using EuropeanWars.UI.Windows;
using Lidgren.Network;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.Core.Diplomacy {
    public class Alliance : DiplomaticRelation {
        public static bool messageSent;

        protected override void OnStart() {
            base.OnStart();
            name = "Sojusz";
        }

        public static bool CanCreate(CountryInfo sender, CountryInfo receiver) => 
            !sender.alliances.ContainsKey(receiver) && !sender.IsInWarAgainstCountry(receiver); //TODO: Add if sender.relations[receiver] >= some value.

        public static bool CanDelete(CountryInfo sender, CountryInfo receiver) =>
            sender.alliances.ContainsKey(receiver);

        public static void AllianceRequest(CountryInfo sender, CountryInfo receiver) {
            if (!CanCreate(sender, receiver) || messageSent) {
                return;
            }

            if (receiver.isPlayer) {
                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)1024);
                msg.Write(sender.id);
                msg.Write(receiver.id);
                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                messageSent = true;
            }
            else {
                AllianceRequestClient(sender, receiver);
            }
        }
        public static void AcceptAlliance(Alliance alliance) {
            if (alliance.countries.Where(t => t.isPlayer).Any()) {
                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)1025);
                msg.Write(alliance.countries[0].id);
                msg.Write(alliance.countries[1].id);
                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                messageSent = true;
            }
            else {
                CreateAllianceClient(alliance);
            }
        }
        public static void DeliceAlliance(Alliance alliance) {
            if (alliance.countries.Where(t => t.isPlayer).Any()) {
                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)1026);
                msg.Write(alliance.countries[0].id);
                msg.Write(alliance.countries[1].id);
                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                messageSent = true;
            }
        }
        public static void DeleteAlliance(Alliance alliance) {
            if (!CanDelete(alliance.countries[0], alliance.countries[1]) || messageSent) {
                return;
            }

            if (alliance.countries.Where(t => t.isPlayer).Any()) {
                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)1027);
                msg.Write(alliance.countries[0].id);
                msg.Write(alliance.countries[1].id);
                msg.Write(GameInfo.PlayerCountry.id);
                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
                messageSent = true;
            }
            else {
                DeleteAllianceClient(alliance);
            }
        }
        public static void AllianceRequestClient(CountryInfo sender, CountryInfo receiver) {
            Alliance a = new Alliance();
            a.countries.Add(sender);
            a.countries.Add(receiver);

            if (!receiver.isPlayer) {
                //TODO: Add bot request mechanic.
            }
            else if (receiver == GameInfo.PlayerCountry) {
                //TODO: Implement translation
                var win = DiplomacyWindow.Singleton.SpawnRequest(a);
                win.title.text = "Propozycja sojuszu!";
                win.description.text = "Nasi bliscy sąsiedzi proponują nam zawarcie sojuszu!";
                win.acceptText.text = "Zaakceptuj";
                win.deliceText.text = "Odrzuć";
            }
        }
        public static void CreateAllianceClient(Alliance alliance) {
            if (alliance.countries.Count == 2) {
                DiplomacyManager.alliances.Add(alliance);
                alliance.countries[0].alliances.Add(alliance.countries[1], alliance);
                alliance.countries[1].alliances.Add(alliance.countries[0], alliance);

                if (alliance.countries.Contains(GameInfo.PlayerCountry)) {
                    foreach (var item in alliance.countries) {
                        foreach (var p in item.provinces) {
                            p.RefreshFogOfWar();
                        }
                    }
                }

                if (alliance.countries[0] == GameInfo.PlayerCountry) {
                    //TODO: Implement translation
                    var win = DiplomacyWindow.Singleton.SpawnRequest(alliance, true);
                    win.title.text = "Zawarto sojuszu!";
                    win.description.text = "Państwo, któremu zaproponowaliśmy sojusz zaakceptowało naszą propozycję!";
                    win.acceptText.text = "Ok";
                    win.deliceText.GetComponentInParent<Button>().gameObject.SetActive(false);

                    DiplomacyWindow.Singleton.countryWindow.UpdateDipActions();
                }

                messageSent = false;
            }
        }
        public static void DeleteAllianceClient(Alliance alliance) {
            foreach (var item in alliance.countries) {
                item.alliances.Remove(item.alliances.Where(t => t.Value == alliance).FirstOrDefault().Key);
            }
            DiplomacyManager.alliances.Remove(alliance);

            DiplomacyWindow.Singleton.countryWindow.UpdateDipActions();
            messageSent = false;
        }
        public static void DeleteAllianceClient(Alliance alliance, int s) {
            if (alliance.countries.Contains(GameInfo.PlayerCountry)) {
                if (GameInfo.PlayerCountry.id != s) {
                    //TODO: Implement translation
                    var win = DiplomacyWindow.Singleton.SpawnRequest(alliance, true);
                    win.title.text = "Koniec sojuszu";
                    win.description.text = "Nasz sojusznik zerwał z nami sojusz!";
                    win.acceptText.text = "Ok";
                    win.deliceText.GetComponentInParent<Button>().gameObject.SetActive(false);
                }
            }

            foreach (var item in alliance.countries) {
                item.alliances.Remove(item.alliances.Where(t => t.Value == alliance).FirstOrDefault().Key);
            }

            if (alliance.countries.Contains(GameInfo.PlayerCountry)) {
                foreach (var item in alliance.countries) {
                    foreach (var p in item.provinces) {
                        p.RefreshFogOfWar();
                    }
                }
            }

            DiplomacyManager.alliances.Remove(alliance);

            DiplomacyWindow.Singleton.countryWindow.UpdateDipActions();
            messageSent = false;
        }
    }
}
