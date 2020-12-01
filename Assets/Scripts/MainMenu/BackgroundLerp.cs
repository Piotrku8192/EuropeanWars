using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EuropeanWars.MainMenu {
    public class BackgroundLerp : MonoBehaviour {
        public List<Sprite> backgrounds = new List<Sprite>();
        public float bgTime;
        public float changeTime;

        public Image background;

        private int currentBackground;
        private float time;

        public void Start() {
            background.sprite = backgrounds[0];
        }

        public void Update() {
            if (time >= bgTime) {
                StartCoroutine(ChangeBackground());
                time = 0;
            }
            time += Time.deltaTime;
        }

        public IEnumerator ChangeBackground() {
            StartCoroutine(LerpBackground(1.0f, 0.0f));
            yield return new WaitForSeconds(changeTime);

            currentBackground++;
            if (currentBackground == backgrounds.Count) {
                currentBackground = 0;
            }
            background.sprite = backgrounds[currentBackground];

            StartCoroutine(LerpBackground(0.0f, 1.0f));
            yield return new WaitForSeconds(changeTime);
        }

        public IEnumerator LerpBackground(float from, float to) {
            int steps = (int)(changeTime / (float)Time.fixedDeltaTime);
            float step = 1.0f / (float)steps;
            for (int i = 0; i < steps; i++) {
                Color c = background.color;
                c.a = Mathf.Lerp(from, to, step * i);
                background.color = c;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
