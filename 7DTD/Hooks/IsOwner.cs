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
    class IsOwner : MonoBehaviour
    {
        public static DumbHook Hook;
        public void Start()
        {
            
                Hook = new DumbHook();
                Hook.Init(typeof(EntityVehicle).GetMethod("IsOwner", BindingFlags.Public | BindingFlags.Instance), typeof(IsOwner).GetMethod("HookFunction", BindingFlags.Public | BindingFlags.Instance));
                Hook.Hook();
            
        }
        public bool HookFunction(PlatformUserIdentifierAbs _userIdentifier)
        {
            if (Globals.Config.LocalPlayer.OwnsVehicle)
                return true; // set the user as owning the car
            else
            {
                Hook.Unhook();
                object[] parameters = new object[]
                  {
                    _userIdentifier,

                  };
                object result = Hook.OriginalMethod.Invoke(this, parameters);
                Hook.Hook(); // call the original
                return false; // this wont be called
            }
        }
    }
}
