using UnityEngine;
using UnityEngine.SceneManagement;

namespace EuropeanWars.MainMenu {
    public class SimpleSceneChanger : MonoBehaviour {
        public void ChangeScene(int id) {
            SceneManager.LoadScene(id);
        }

        public void QuitApplication() {
            Application.Quit();
        }
    }
}
