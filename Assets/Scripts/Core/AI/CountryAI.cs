using EuropeanWars.Core.Army;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Time;
using EuropeanWars.Core.War;
using EuropeanWars.Network;
using Lidgren.Network;

namespace EuropeanWars.Core.AI {
    public abstract class CountryAI {
        protected readonly CountryInfo country;

        protected CountryAI(CountryInfo country) {
            this.country = country;
            TimeManager.lateOnDayElapsed += OnDayElapsed;
            TimeManager.lateOnDayElapsed += CheckIfCountryIsPlayer;
            TimeManager.lateOnMonthElapsed += OnMonthElapsed;
            TimeManager.lateOnYearElapsed += OnYearElapsed;
        }

        public void Delete() {
            TimeManager.lateOnDayElapsed -= OnDayElapsed;
            TimeManager.lateOnDayElapsed -= CheckIfCountryIsPlayer;
            TimeManager.lateOnMonthElapsed -= OnMonthElapsed;
            TimeManager.lateOnYearElapsed -= OnYearElapsed;
        }

        private void CheckIfCountryIsPlayer() {
            if (country.isPlayer) {
                GameInfo.countryAIs.Remove(country);
                Delete();
            }
        }

        protected abstract void OnDayElapsed();
        protected abstract void OnMonthElapsed();
        protected abstract void OnYearElapsed(); 

        protected virtual void BuildBuildingInSlot(BuildingInfo building, ProvinceInfo province, int slot) {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)512);
            msg.Write(building.id);
            msg.Write(province.id);
            msg.Write(slot);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }
        protected virtual void RecruitArmy(UnitInfo unit, ProvinceInfo province, int size) {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)2048);
            msg.Write(unit.id);
            msg.Write(country.id);
            msg.Write(province.id);
            msg.Write(size);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }
        protected virtual void FabricateClaim(ProvinceInfo province) {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)1032);
            msg.Write(province.id);
            msg.Write(country.id);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }
        protected virtual void DeclareWar(CountryInfo _country, int reason) {
            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)1035);
            msg.Write(country.id);
            msg.Write(_country.id);
            msg.Write(reason);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
