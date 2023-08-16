using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Net;
using System.IO;

namespace Cheat.Helpers
{
    class ShaderHelper
    {
      //  public static Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>();
        public static Dictionary<string, Shader> ModelShaders = new Dictionary<string, Shader>();
        public static Dictionary<string, Material> ModelMaterials = new Dictionary<string, Material>();
        public static Dictionary<int, Shader> Shaders = new Dictionary<int, Shader>();
        private static Material Galaxy;
        public static void GetShader()
        {
            using (WebClient webclient = new WebClient())
            { 
                AssetBundle bundle = AssetBundle.LoadFromMemory(webclient.DownloadData("https://github.com/IntelSDM/7DTD/raw/main/Shaders/Shader1?raw=true"));
                int i = 0;
                foreach (Shader s in bundle.LoadAllAssets<Shader>())
                {
                    Shaders.Add(i, s);
                    i++;
                }
                AssetBundle transparentbundle = AssetBundle.LoadFromMemory(webclient.DownloadData("https://github.com/IntelSDM/7DTD/raw/main/Shaders/Shader3?raw=true"));
                Shader transparentshader = transparentbundle.LoadAsset<Shader>("Force Field.shader");
                Shaders.Add(i, transparentshader);
                i++;
                AssetBundle galaxybundle = AssetBundle.LoadFromMemory(webclient.DownloadData("https://github.com/IntelSDM/7DTD/raw/main/Shaders/Shader2?raw=true"));
                Material galaxyshader = galaxybundle.LoadAsset<Material>("mat12_10sdglksdg949gsgs.mat");
                Galaxy = galaxyshader;
            }
        }
        public static void ApplyShader(int shadertype, GameObject pgo, Color32 primary, Color32 secondary)
        {
            if (shadertype > Shaders.Count-1)
            {
                foreach (var r in pgo.GetComponentsInChildren<UnityEngine.Renderer>())
                {
                    if (!ModelMaterials.ContainsKey(r.material.name))
                    {
                        ModelMaterials.Add(r.material.name, r.material);
                    }
                    r.material = Galaxy;
                  
                }
                return;
            }
            Shader shader = Shaders[shadertype];

            if (shader == null)
                return;
            foreach (var r in pgo.GetComponentsInChildren<UnityEngine.Renderer>())
            {
                foreach (Material m in r.materials)
                {
                    if (!ModelShaders.ContainsKey(m.name) && m.shader != shader)
                    {
                        ModelShaders.Add(m.name, m.shader);
                    }
                    if (m.shader != shader)
                    {
                        m.shader = shader;
                        m.SetColor("_ColorVisible", primary);
                        m.SetColor("_ColorBehind", secondary);
                        m.SetColor("_Emissioncolour", primary);
                        m.SetColor("_WireColor", primary);
                        m.SetColor("_Color", primary);
                    }
                }
            }
          
         
        }
        public static void RemoveShader(GameObject pgo)
        {
            foreach (var r in pgo.GetComponentsInChildren<UnityEngine.Renderer>())
            {
                foreach (Material m in r.materials)
                {
                    if (ModelShaders.ContainsKey(m.name))
                    {
                        m.shader = ModelShaders[m.name];

                    }
                }
                if (ModelMaterials.ContainsKey(r.material.name))
                {
                    r.material = ModelMaterials[r.material.name];

                }
            }
        }
       
    }
}
