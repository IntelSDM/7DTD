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
    class GetRange : MonoBehaviour
    {
        public static DumbHook Hook;
        public void Start()
        {
            
                Hook = new DumbHook();
                Hook.Init(typeof(ItemActionRanged).GetMethod("GetRange", BindingFlags.Public | BindingFlags.Instance), typeof(GetRange).GetMethod("HookFunction", BindingFlags.Public | BindingFlags.Instance));
                Hook.Hook();
            
        }
        public float HookFunction(ItemActionData _actionData)
        {
            vp_FPWeapon weapon = Globals.LocalPlayer?.vp_FPWeapon;
            Inventory inventory = Globals.LocalPlayer?.inventory;
            ItemActionAttack gun = inventory?.GetHoldingGun();
            ItemActionRanged action = gun as ItemActionRanged;
            if (!Globals.Config.LocalPlayer.UnlimitedRange)
                return EffectManager.GetValue(PassiveEffects.MaxRange, _actionData.invData.itemValue, action.Range, _actionData.invData.holdingEntity, null, default(FastTags), true, true, true, true, 1, true);
            else
                return 100000000000; // return firing range of infinite
        }
    }
}
