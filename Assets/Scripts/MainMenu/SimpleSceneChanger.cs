using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EuropeanWars.MainMenu {
    public class SimpleSceneChanger : MonoBehaviour
    {
        public void ChangeScene(int id) {
            SceneManager.LoadScene(id);
        }   
    }
}
