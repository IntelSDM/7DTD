using Cheat.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat.Hooks
{
    class OnFired : MonoBehaviour
    {
        public static DumbHook Hook;
        public void Start()
        {
            if (Globals.Config.LocalPlayer.NoRecoil)
            {
                Hook = new DumbHook();
                Hook.Init(typeof(EntityPlayerLocal).GetMethod("OnFired", BindingFlags.Public | BindingFlags.Instance), typeof(OnFired).GetMethod("HookFunction", BindingFlags.Public | BindingFlags.Instance));
                Hook.Hook();
            }
        }
        public void HookFunction()
        {
       // do nothing
        }
    }
}
