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
    class UpdateAccuracy : MonoBehaviour
    {
        public static DumbHook Hook;
        public void Start()
        {
            if (Globals.Config.LocalPlayer.NoSpread)
            {
                Hook = new DumbHook();
                Hook.Init(typeof(ItemActionRanged).GetMethod("updateAccuracy", BindingFlags.NonPublic | BindingFlags.Instance), typeof(UpdateAccuracy).GetMethod("HookFunction", BindingFlags.NonPublic | BindingFlags.Instance));
                Hook.Hook();
            }
        }
        float HookFunction(ItemActionData _actionData, bool _isAimingGun)
        {
            (_actionData as ItemActionRanged.ItemActionDataRanged).lastAccuracy = 0; // set this incase its used elsewhere

            return (_actionData as ItemActionRanged.ItemActionDataRanged).lastAccuracy; // could return 0 but ehh
        }
    }
}
