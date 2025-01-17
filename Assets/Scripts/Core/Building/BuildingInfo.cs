﻿using EuropeanWars.Core.Data;
using EuropeanWars.Core.Language;
using EuropeanWars.Core.Province;
using System.Linq;
using UnityEngine;

namespace EuropeanWars.Core.Building {
    public class BuildingInfo {
        public int id;
        public string name;
        public Sprite image;
        public int cost;

        public int incomeModifier;
        public int foodModifier;
        public int defenceModifier;

        public TerrainType terrain;

        private BuildingData data;

        public BuildingInfo(BuildingData data) {
            this.data = data;
            id = data.id;
            cost = data.cost;
            incomeModifier = data.incomeModifier;
            defenceModifier = data.defenceModifier;
            foodModifier = data.foodModifier;
            terrain = (TerrainType)data.terrain;
        }

        public void Initialize() {
            image = GameInfo.gfx["Building-" + id];
        }

        public void UpdateLanguage() {
            name = LanguageDictionary.language["BuildingName-" + id];
        }

        public bool CanBuildInProvince(ProvinceInfo province) {
            if (id == 0 || (!province.buildings.Contains(this) && province.Country.gold >= cost)) {
                return true;
            }
            return false;
        }
    }
}
