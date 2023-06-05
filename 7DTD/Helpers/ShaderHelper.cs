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
            using (WebClient webclient = new WebClient())
            { // yes i do just stream a shader from github. 
                AssetBundle bundle = AssetBundle.LoadFromMemory(webclient.DownloadData("https://github.com/Coopyy/EgguWare-Unturned/blob/master/Assets/EgguWareV1.assets?raw=true")); // steal it from egguware

                foreach (Shader s in bundle.LoadAllAssets<Shader>())
                    Shaders.Add(s.name, s);
            }
        }
        public static void ApplyShader(Shader shader, GameObject pgo, Color32 visiblecolour, Color32 occludedcolour)
        {
            if (shader == null) return;

            Renderer[] rds = pgo.GetComponentsInChildren<Renderer>();

            for (int j = 0; j < rds.Length; j++)
            {
                Material[] materials = rds[j].materials;

                for (int k = 0; k < materials.Length; k++)
                {
                    // set shader and the colour
                    materials[k].shader = shader;
                    materials[k].SetColor("_ColorVisible", visiblecolour);
                    materials[k].SetColor("_ColorBehind", occludedcolour);
                }
            }
        }
       
    }
}
