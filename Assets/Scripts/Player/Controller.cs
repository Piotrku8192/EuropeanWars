using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EuropeanWars
{
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
            pos += new Vector3(0, 0, Input.mouseScrollDelta.y * scaleSpeed);
            playerCam.transform.localPosition = new Vector3(pos.x, pos.y, -Mathf.Clamp(-pos.z, minScope, maxScope));

            //if (Input.GetKeyDown(KeyCode.P))
            //{
            //    transform.rotation = Quaternion.Euler(new Vector3(isPerspectiveView ? 0 : -30, 0, 0));
            //    isPerspectiveView = !isPerspectiveView;
            //}
        }

        private bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            bool b = false;
            foreach (RaycastResult item in results)
            {
                if (item.gameObject.layer != 11)
                {
                    b = true;
                }
            }

            return results.Count > 0 && b;
        }
    }

}
