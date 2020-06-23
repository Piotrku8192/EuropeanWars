using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace EuropeanWars.Core.Data {
    public static class DataConverter {
        public static T FromString<T>(string text) {
            byte[] bytes = System.Convert.FromBase64String(text);

            using (var memStream = new MemoryStream()) {
                var binForm = new BinaryFormatter();
                memStream.Write(bytes, 0, bytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return (T)obj;
            }
        }

        public static string ToString(object obj) {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream()) {
                bf.Serialize(ms, obj);
                return System.Convert.ToBase64String(ms.ToArray());
            }
        }

        public static T FromJson<T>(string text) {
            byte[] bytes = System.Convert.FromBase64String(text);
            return JsonConvert.DeserializeObject<T>(Encoding.ASCII.GetString(bytes));
        }

        public static string ToJson(object obj) {
            return System.Convert.ToBase64String(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj)));
        }

        public static Color32 ToColor(int HexVal) {
            byte R = (byte)((HexVal >> 16) & 0xFF);
            byte G = (byte)((HexVal >> 8) & 0xFF);
            byte B = (byte)((HexVal) & 0xFF);
            return new Color32(R, G, B, 255);
        }
    }
}
