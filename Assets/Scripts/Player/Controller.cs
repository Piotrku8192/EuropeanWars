using EuropeanWars.Core;
using EuropeanWars.GameMap;
using UnityEngine;

namespace EuropeanWars {
    public class Controller : MonoBehaviour
    {
        public static Controller Singleton { get; private set; }
        public Camera playerCam;
        public float speed;
        public float scaleSpeed;
        public float minScope, maxScope;
        public float armiesDistance;
        public float fogOfWarDistance;

        private void Awake()
        {
            Singleton = this;
        }

        private void Start()
        {
            playerCam = GetComponent<Camera>();
        }

        private void Update()
        {
            Vector3 p = transform.position + (new Vector3(Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical"), 0) * Time.deltaTime * speed);
            transform.position = new Vector3(Mathf.Clamp(p.x, 0, 2400), Mathf.Clamp(p.y, -2400, 0), p.z);

            Vector3 pos = playerCam.transform.localPosition;
            if (!GameInfo.IsPointerOverScrollView()) {
                playerCam.orthographicSize = Mathf.Clamp(
                    playerCam.orthographicSize - Input.mouseScrollDelta.y * scaleSpeed, minScope, maxScope);
            }
            playerCam.transform.localPosition = new Vector3(pos.x, pos.y, -10);
        }
    }

}
