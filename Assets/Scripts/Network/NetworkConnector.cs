using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EuropeanWars.Network {
    public class NetworkConnector : MonoBehaviour
    {
        public InputField address;

        public void StartHost() {
            Server s = new GameObject("Server").AddComponent<Server>();
            address.text = "127.0.0.1";
            StartClient();
        }

        public void StartClient() {
            Client c = new GameObject("Client").AddComponent<Client>();
            c.serverAddress = address.text;
            SceneManager.LoadScene(1);
        }
    }
}
