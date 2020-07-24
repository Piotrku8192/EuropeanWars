using EuropeanWars.Core.Pathfinding;
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
using EuropeanWars.Core.Army;
using System.Threading;

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
        private bool loaded;

        private bool convertFromJsonFiles = false;
        private Thread loadingThread;

        public void Start() {
            path = Application.dataPath + "\\Content";
#if UNITY_EDITOR
            path = Directory.GetCurrentDirectory() + "\\Tests\\Content";
            convertFromJsonFiles = true;
#endif
            loadingThread = new Thread(LoadData);
            loadingThread.Start();

        }

        public void Update() {
            //UpdateProgressBar();
            if (loaded) {
                foreach (var item in gameData.gfx) {
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(item.Value);
                    GameInfo.gfx.Add(item.Key, Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2()));
                }

                MapData m = DataConverter.FromJson<MapData>(gameData.map);
                MapGenerator.Singleton.mapData = m;
                MapGenerator.Singleton.GenerateMap();

                GameInfo.Initialize();

                //Generate armies
                foreach (var item in gameData.armies) {
                    new ArmyInfo(item);
                }

                NetOutgoingMessage msg = Client.Singleton.c.CreateMessage();
                msg.Write((ushort)1);
                Client.Singleton.c.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);

                loadingScreen.SetActive(false);
                loaded = false;
            }
        }

        private void LoadData() {
            if (convertFromJsonFiles) {
                ConvertFromJsonFiles();
            }
            gameData = DataConverter.FromString<GameData>(File.ReadAllText(path + "\\gameData"));
            gameData.FillGameInfo();

            loaded = true;
            loadingThread.Abort();
        }

        private void ConvertFromJsonFiles() {
            foreach (var item in Directory.GetFiles(path + "\\gfx")) {
                string name = item.Split('\\').Last().Split('.').First();
                byte[] bytes = File.ReadAllBytes(item);
                gameData.gfx.Add(name, bytes);
            }
            gameData.languages = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(File.ReadAllText(path + "\\languages.json"));
            gameData.buildings = JsonConvert.DeserializeObject<BuildingData[]>(File.ReadAllText(path + "\\buildings.json"));
            gameData.religions = JsonConvert.DeserializeObject<ReligionData[]>(File.ReadAllText(path + "\\religions.json"));
            gameData.cultures = JsonConvert.DeserializeObject<CultureData[]>(File.ReadAllText(path + "\\cultures.json"));
            gameData.countries = JsonConvert.DeserializeObject<CountryData[]>(File.ReadAllText(path + "\\countries.json"));
            gameData.provinces = JsonConvert.DeserializeObject<ProvinceData[]>(File.ReadAllText(path + "\\provinces.json"));
            gameData.units = JsonConvert.DeserializeObject<UnitData[]>(File.ReadAllText(path + "\\units.json"));
            gameData.armies = JsonConvert.DeserializeObject<ArmyData[]>(File.ReadAllText(path + "\\armies.json"));
            gameData.map = File.ReadAllText(path + "\\map");
            File.WriteAllText(path + "\\gameData", DataConverter.ToString(gameData));
        }

        private void ConvertToJsonFiles() {
            File.WriteAllText(path + "\\languages.json", JsonConvert.SerializeObject(LanguageDictionary.languages));
            File.WriteAllText(path + "\\buildings.json", JsonConvert.SerializeObject(gameData.buildings));
            File.WriteAllText(path + "\\religions.json", JsonConvert.SerializeObject(gameData.religions));
            File.WriteAllText(path + "\\cultures.json", JsonConvert.SerializeObject(gameData.cultures));
            File.WriteAllText(path + "\\countries.json", JsonConvert.SerializeObject(gameData.countries));
            File.WriteAllText(path + "\\provinces.json", JsonConvert.SerializeObject(gameData.provinces));
            File.WriteAllText(path + "\\units.json", JsonConvert.SerializeObject(gameData.units));
            File.WriteAllText(path + "\\armies.json", JsonConvert.SerializeObject(gameData.armies));
        }
    }
}
