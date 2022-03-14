using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat
{
    /*
    You may see some unity methods just calling a function of the method with a 1 on the end. This is because we are excluding the unity method from obfuscation.
    We need to exclude these to keep them working but we make another function to keep the actual code obfuscated.
     */
    class Globals : MonoBehaviour
    {
        public static Camera MainCamera;
        public static Cheat.Configs.Config Config = new Configs.Config();
        public static EntityPlayerLocal LocalPlayer;
        public static bool IsScreenPointVisible(Vector3 screenPoint)
        {
            return screenPoint.z > 0.01f && screenPoint.x > -5f && screenPoint.y > -5f && screenPoint.x < (float)Screen.width && screenPoint.y < (float)Screen.height;
        }

        public static Vector3 WorldPointToScreenPoint(Vector3 worldPoint)
        {
            Vector3 vector = MainCamera.WorldToScreenPoint(worldPoint);
            vector.y = (float)Screen.height - vector.y;
            return vector;
        }
        public static Vector3 GetLimbPosition(Transform target, string objName)
        {
            var componentsInChildren = target.transform.GetComponentsInChildren<Transform>();
            var result = Vector3.zero;

            if (componentsInChildren == null) return result;

            foreach (var transform in componentsInChildren)
            {
                if (transform.name.Trim() != objName) continue;

                result = transform.position + new Vector3(0f, 0.4f, 0f);
                break;
            }

            return result;
        }
        [ObfuscationAttribute(Exclude = true)]
        private void Start()
        {
            Helpers.ShaderHelper.GetShader();
            Helpers.ConfigHelper.CreateEnvironment();
            Helpers.ColourHelper.AddColours();
        }
    }
}
