using EuropeanWars.Core.Army;
using EuropeanWars.Core.Building;
using EuropeanWars.Core.Country;
using EuropeanWars.Core.Culture;
using EuropeanWars.Core.Language;
using EuropeanWars.Core.Province;
using EuropeanWars.Core.Religion;
using EuropeanWars.GameMap;
using EuropeanWars.GameMap.Data;
using EuropeanWars.Network;
using Lidgren.Network;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.Core.Data {
    public class DataLoader : MonoBehaviour
    {
        [HideInInspector] public GameData gameData;
        [HideInInspector] public MapData map;

        public MapGenerator generator;

        public string path;
        public GameObject loadingScreen;
        public Image progressBar;
        public Text progressText;
        public int steps;

        public void Start() {
            path = Application.dataPath + "\\Content";
#if UNITY_EDITOR
            path = Directory.GetCurrentDirectory() + "\\Tests\\Content";
#endif

            UnitData[] u = JsonConvert.DeserializeObject<UnitData[]>(File.ReadAllText(path + "\\units.json"));
            gameData.units = new UnitData[u.Length];
            for (int i = 0; i < u.Length; i++) {
                gameData.units[i] = u[i];
            }
            File.WriteAllText(path + "\\units", DataConverter.ToString(gameData.units));

            //LanguageDictionary.languages = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(File.ReadAllText(path + "\\language.json"));
            //File.WriteAllText(path + "\\language", DataConverter.ToString(LanguageDictionary.languages));
            StartCoroutine(LoadContent());
        }

        public IEnumerator LoadContent() {
            yield return new WaitForEndOfFrame();

            UpdateProgressBar(0, "Wczytywanie języków...");
            yield return new WaitForEndOfFrame();
            LanguageDictionary.languages = DataConverter.FromString<List<Dictionary<string, string>>>(File.ReadAllText(path + "\\language"));
            LanguageDictionary.language = LanguageDictionary.languages[0];
            //////////////////////////////////////////////////////////////////////////////////////

            UpdateProgressBar(1, "Wczytywanie grafik...");
            yield return new WaitForEndOfFrame();
            foreach (var item in Directory.GetFiles(path + "\\gfx")) {
                string name = item.Split('\\').Last().Split('.').First();
                byte[] bytes = File.ReadAllBytes(item);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(bytes);
                GameInfo.gfx.Add(name, Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2()));
            }
            //////////////////////////////////////////////////////////////////////////////////////

            UpdateProgressBar(2, "Wczytywanie budynków...");
            yield return new WaitForEndOfFrame();
            gameData.buildings = DataConverter.FromString<BuildingData[]>(File.ReadAllText(path + "\\buildings"));
            foreach (var item in gameData.buildings) {
                GameInfo.buildings.Add(item.id, new BuildingInfo(item));
            }
            //////////////////////////////////////////////////////////////////////////////////////

            UpdateProgressBar(3, "Wczytywanie religii...");
            yield return new WaitForEndOfFrame();
            gameData.religions = DataConverter.FromString<ReligionData[]>(File.ReadAllText(path + "\\religions"));
            foreach (var item in gameData.religions) {
                GameInfo.religions.Add(item.id, new ReligionInfo(item));
            }
            //////////////////////////////////////////////////////////////////////////////////////

            UpdateProgressBar(4, "Wczytywanie kultur...");
            yield return new WaitForEndOfFrame();
            gameData.cultures = DataConverter.FromString<CultureData[]>(File.ReadAllText(path + "\\cultures"));
            foreach (var item in gameData.cultures) {
                GameInfo.cultures.Add(item.id, new CultureInfo(item));
            }
            //////////////////////////////////////////////////////////////////////////////////////

            UpdateProgressBar(5, "Wczytywanie państw...");
            yield return new WaitForEndOfFrame();
            gameData.countries = DataConverter.FromString<CountryData[]>(File.ReadAllText(path + "\\countries"));
            foreach (var item in gameData.countries) {
                GameInfo.countries.Add(item.id, new CountryInfo(item));
            }
            //////////////////////////////////////////////////////////////////////////////////////

            UpdateProgressBar(6, "Wczytywanie prowincji...");
            yield return new WaitForEndOfFrame();
            gameData.provinces = DataConverter.FromString<ProvinceData[]>(File.ReadAllText(path + "\\provinces"));
            int i = 0;
            foreach (var item in gameData.provinces) {
                item.id = i;
                GameInfo.provinces.Add(item.id, new ProvinceInfo(item));
                i++;
            }
            //File.WriteAllText(path + "\\provinces", DataConverter.ToString(gameData.provinces));
            //////////////////////////////////////////////////////////////////////////////////////

            UpdateProgressBar(6, "Wczytywanie jednostek wojskowych...");
            yield return new WaitForEndOfFrame();
            gameData.units = DataConverter.FromString<UnitData[]>(File.ReadAllText(path + "\\units"));
            foreach (var item in gameData.units) {
                GameInfo.units.Add(item.id, new UnitInfo(item));
            }
            //////////////////////////////////////////////////////////////////////////////////////
            
            UpdateProgressBar(7, "Wczytywanie mapy...");
            yield return new WaitForEndOfFrame();
            map = DataConverter.FromJson<MapData>(File.ReadAllText(path + "\\map"));
            //////////////////////////////////////////////////////////////////////////////////////
            
            UpdateProgressBar(8, "Generowanie mapy...");
            yield return new WaitForEndOfFrame();
            generator.mapData = map;
            generator.GenerateMap();
            //////////////////////////////////////////////////////////////////////////////////////

            UpdateProgressBar(9, "Initializacja obiektów...");
            yield return new WaitForEndOfFrame();
            GameInfo.Initialize();
            //////////////////////////////////////////////////////////////////////////////////////

            NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
            msg.Write((ushort)1);
            Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);

            loadingScreen.SetActive(false);
        }

        public void UpdateProgressBar(int step, string text) {
            progressBar.fillAmount = (float)step / steps;
            progressText.text = text;
        }
    }
}
