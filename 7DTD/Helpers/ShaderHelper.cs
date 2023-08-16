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
        public static Dictionary<string, Shader> ModelShaders = new Dictionary<string, Shader>();
        public static void GetShader()
        {
            using (WebClient webclient = new WebClient())
            { // yes i do just stream a shader from github. 
                AssetBundle bundle = AssetBundle.LoadFromMemory(webclient.DownloadData("https://github.com/Coopyy/EgguWare-Unturned/blob/master/Assets/EgguWareV1.assets?raw=true")); // steal it from egguware

                foreach (Shader s in bundle.LoadAllAssets<Shader>())
                    Shaders.Add(s.name, s);
            }
        }
        public static void ApplyShader(Shader shader, GameObject pgo, Color32 visiblecolour, Color32 occludedcolour)
        {
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
                        m.SetColor("_ColorVisible", visiblecolour);
                        m.SetColor("_ColorBehind", occludedcolour);
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
            }
        }
       
    }
}
