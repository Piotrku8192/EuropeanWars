using Lidgren.Network;

namespace EuropeanWars.Network {
    public class ClientInfo {
        public NetConnection connection;
        public string nick;
        public int countryId;
        public bool isReady;
    }
}
