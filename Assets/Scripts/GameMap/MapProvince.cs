using EuropeanWars.Core;
using EuropeanWars.Core.Province;
using EuropeanWars.GameMap.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EuropeanWars.GameMap {
    public class MapProvince : MonoBehaviour {
        public List<Border> borders = new List<Border>();
        public MeshFilter filter;
        public MeshRenderer meshRenderer;
        public MeshCollider meshCollider;
        public Material material;

        private ProvinceInfo provinceInfo;
        private Coroutine selectionCoroutine;

        public void OnMouseDown() {
            if (!GameInfo.IsPointerOverUIObject()) {
                GameInfo.SelectProvince(provinceInfo);
            }
        }

        public void GenerateProvince(MeshData meshData, Material material, ProvinceInfo provinceInfo) {
            this.provinceInfo = provinceInfo;
            filter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            this.material = material;
            meshRenderer.material = material;

            Mesh m = new Mesh();
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();

            foreach (var v in meshData.vertices) {
                verts.Add(new Vector3(v.X, -v.Y));
                uv.Add(new Vector2(v.X, -v.Y));
            }

            Vector3 min = verts[0], max = verts[0];
            foreach (var v in verts) {
                if (v.x < min.x) {
                    min.x = v.x;
                }
                if (v.y > min.y) {
                    min.y = v.y;
                }
                if (v.x > max.x) {
                    max.x = v.x;
                }
                if (v.y < max.y) {
                    max.y = v.y;
                }
            }
            provinceInfo.x = Mathf.Lerp(min.x, max.x, 0.5f);
            provinceInfo.y = Mathf.Lerp(min.y, max.y, 0.5f);

            m.vertices = verts.ToArray();
            m.uv = uv.ToArray();
            m.triangles = meshData.indices.ToArray();

            filter.sharedMesh = m;
            meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = m;

            gameObject.SetActive(provinceInfo.isActive);
            foreach (var item in borders) {
                item.gameObject.SetActive(provinceInfo.isActive);
            }
        }

        public void UpdateBorders() {
            foreach (var item in borders) {
                item.UpdateBorder();
            }
        }

        public void OnSelectProvince() {
            selectionCoroutine = StartCoroutine(UpdateSelection());
            foreach (var item in borders) {
                item.SetStyle(BorderStyle.SelectedProvince);
            }
        }

        public void OnUnselectProvince() {
            if (selectionCoroutine != null) {
                StopCoroutine(selectionCoroutine);
            }
            foreach (var item in borders) {
                item.SetColor(Color.black);
            }
            UpdateBorders();
        }

        public IEnumerator UpdateSelection() {
            int i = 0;
            int max = 50;
            Color from = Color.yellow * 0.25f;
            from.a = 1.0f;
            Color to = Color.yellow;
            while (true) {
                if (i > max) {
                    i = 0;
                    Color f = from;
                    from = to;
                    to = f;
                }
                foreach (var item in borders) {
                    item.SetColor(Color.Lerp(from, to, i / (float)max));
                }

                i++;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
