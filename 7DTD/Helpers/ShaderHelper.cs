using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Net;
namespace Cheat.Helpers
{
    class ShaderHelper
    {
        public static Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>();
        public static void GetShader()
        {
            using (WebClient webClient = new WebClient())
            {
                AssetBundle Bundle = AssetBundle.LoadFromMemory(webClient.DownloadData("https://github.com/Coopyy/EgguWare-Unturned/blob/master/Assets/EgguWareV1.assets?raw=true"));

                foreach (Shader s in Bundle.LoadAllAssets<Shader>())
                    Shaders.Add(s.name, s);
            }
        }
        [ObfuscationAttribute(Exclude = true)]
        public static void ApplyShader(Shader shader, GameObject pgo, Color32 VisibleColor, Color32 OccludedColor)
        {
            if (shader == null) return;

            Renderer[] rds = pgo.GetComponentsInChildren<Renderer>();

            for (int j = 0; j < rds.Length; j++)
            {
                Material[] materials = rds[j].materials;

                for (int k = 0; k < materials.Length; k++)
                {
                    materials[k].shader = shader;
                    materials[k].SetColor("_ColorVisible", VisibleColor);
                    materials[k].SetColor("_ColorBehind", OccludedColor);
                }
            }
        }
        [ObfuscationAttribute(Exclude = true)]
        public static void RemoveShaders(GameObject pgo)
        {
            if (Shader.Find("Standard") == null) return;

            Renderer[] rds = pgo.GetComponentsInChildren<Renderer>();

            for (int j = 0; j < rds.Length; j++)
            {
                if (!(rds[j].material.shader != Shader.Find("Standard"))) continue;

                Material[] materials = rds[j].materials;

                for (int k = 0; k < materials.Length; k++)
                {
                    materials[k].shader = Shader.Find("Standard");
                }
            }
        }
    }
}
