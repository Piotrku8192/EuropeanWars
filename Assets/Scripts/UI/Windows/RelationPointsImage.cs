using UnityEngine;
using UnityEngine.UI;
using EuropeanWars.Core.Diplomacy;
using EuropeanWars.Core.Language;

namespace EuropeanWars.UI.Windows {
    public class RelationPointsImage : MonoBehaviour {
        public Sprite badRelations;
        public Sprite normalRelations;
        public Sprite goodRelations;

        public Image image;
        public DescriptionText description;

        public void SetRelation(CountryRelation relation) {
            if (relation.Points < -35) {
                image.sprite = badRelations;
            }
            else if (relation.Points > 35) {
                image.sprite = goodRelations;
            }
            else {
                image.sprite = normalRelations;
            }

            int p = relation.Points;
            Color32 c = Color32.Lerp(Color.red, Color.green, (p + 100) / 200.0f);
            description.text = $"<color=#{c.r.ToString("X") + c.g.ToString("X") + c.b.ToString("X")}>{p}</color>\n" +
                $"{LanguageDictionary.language["ImproveRelationsModifier"]}: {relation.monthlyPointsIncreaseChance * 100}%";
        }
    }
}
