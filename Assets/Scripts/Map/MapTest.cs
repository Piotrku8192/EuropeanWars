using MapBuilder;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EuropeanWars.Map {
    public class MapTest : MonoBehaviour {
        public string provincesPath;
        public string bordersPath;
        public Material material;
        public Material borderMaterial;
        public List<GameObject> meshes = new List<GameObject>();
        public List<LineRenderer> borderRenderers = new List<LineRenderer>();

        public Dictionary<string, Province> provinces = new Dictionary<string, Province>();
        public List<Border> borders = new List<Border>();

        public float scale = 0.25f;

        public void Start() {
            provinces = JsonConvert.DeserializeObject<Dictionary<string, Province>>(File.ReadAllText(provincesPath));
            borders = JsonConvert.DeserializeObject<List<Border>>(File.ReadAllText(bordersPath));

            foreach (var item in provinces) {
                UnityEngine.Mesh mesh = new UnityEngine.Mesh();
                List<Vector3> verts = new List<Vector3>();
                foreach (var v in item.Value.mesh.vertices) {
                    verts.Add(new Vector3(v.X, -v.Y) * scale);
                }
                mesh.vertices = verts.ToArray();
                mesh.triangles = item.Value.mesh.indices.ToArray();

                GameObject go = new GameObject(item.Key);
                go.transform.SetParent(transform);
                MeshFilter filter = go.AddComponent<MeshFilter>();
                filter.sharedMesh = mesh;
                MeshRenderer renderer = go.AddComponent<MeshRenderer>();
                renderer.material = new Material(material);

                int color = int.Parse(item.Key, System.Globalization.NumberStyles.HexNumber);

                renderer.material.color = new Color(0.27f, 0.41f, 0.22f);//ToColor(color);
            }

            foreach (var item in borders) {
                foreach (var verts in item.vertices) {
                    GameObject go = new GameObject(item.firstProvince + item.secondProvince);
                    go.transform.SetParent(transform);
                    LineRenderer line = go.AddComponent<LineRenderer>();
                    borderRenderers.Add(line);
                    List<Vector3> positions = new List<Vector3>();

                    foreach (var vert in verts) {
                        positions.Add(new Vector3(vert.X, -vert.Y, 0) * scale);
                    }

                    line.sortingOrder = 10;
                    line.positionCount = verts.Count;
                    line.SetPositions(positions.ToArray());
                    line.startColor = Color.white;
                    line.endColor = Color.white;
                    line.material = borderMaterial;
                    line.startWidth = 0.7f * scale;
                    line.endWidth = 0.7f * scale;
                    line.numCornerVertices = 0;
                    line.numCapVertices = 3;
                    line.widthMultiplier = 1.2f;
                    line.textureMode = LineTextureMode.Tile;
                    Color32 color = ToColor(int.Parse(item.firstProvince, System.Globalization.NumberStyles.HexNumber));
                    //line.material.color = new Color32((byte)(color.r - 50), (byte)(color.g - 50), (byte)(color.b - 50), color.a);
                }
            }
        }

        public Color32 ToColor(int HexVal) {
            byte R = (byte)((HexVal >> 16) & 0xFF);
            byte G = (byte)((HexVal >> 8) & 0xFF);
            byte B = (byte)((HexVal) & 0xFF);
            return new Color32(R, G, B, 255);
        }
    }
}
