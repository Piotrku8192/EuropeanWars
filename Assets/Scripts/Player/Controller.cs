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

        //private bool isPerspectiveView = false;

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
            transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * Time.deltaTime * speed;

            Vector3 pos = playerCam.transform.localPosition;
            playerCam.orthographicSize = Mathf.Clamp(playerCam.orthographicSize - Input.mouseScrollDelta.y * scaleSpeed, minScope, maxScope);
            playerCam.transform.localPosition = new Vector3(pos.x, pos.y, -10);
        }
    }

}
