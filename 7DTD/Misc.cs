using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cheat.Helpers;

namespace Cheat
{
    class Misc : MonoBehaviour
    {

        // protected virtual float updateAccuracy(ItemActionData _actionData, bool _isAimingGun) hook this to do nospread
        // protected virtual Vector3 fireShot(int _shotIdx, ItemActionRanged.ItemActionDataRanged _actionData) for silentaim 	public virtual Ray GetLookRay()

        #region Hook Methods
        public static DumbHook FireAnimationHook;
        public bool FireAnimationHooked = false;
        public static DumbHook UpdateAccuracyHook;
        public bool AccuracyHooked = false;
        public void FireAnimation()
        {

        }
        public float Accuracy(ItemActionData _actionData, bool _isAimingGun)
        {
            (_actionData as ItemActionRanged.ItemActionDataRanged).lastAccuracy = 0;
            return (_actionData as ItemActionRanged.ItemActionDataRanged).lastAccuracy;
        }
        public static void EnableNoRecoil()
        {
            FireAnimationHook = new DumbHook();
            FireAnimationHook.Init(typeof(EntityPlayerLocal).GetMethod("OnFired", BindingFlags.Public | BindingFlags.Instance), typeof(Misc).GetMethod("FireAnimation", BindingFlags.Public | BindingFlags.Instance));
            FireAnimationHook.Hook();
        }
        public static void EnableNoSpread()
        {
            UpdateAccuracyHook = new DumbHook();
            UpdateAccuracyHook.Init(typeof(ItemActionRanged).GetMethod("updateAccuracy", BindingFlags.NonPublic | BindingFlags.Instance), typeof(Misc).GetMethod("Accuracy", BindingFlags.Public | BindingFlags.Instance));
            UpdateAccuracyHook.Hook();
        }
        #endregion
        [ObfuscationAttribute(Exclude = true)]
        void Update()
        {
            Update1();
        }
        void Update1()
        {
            #region Hooks
            if (Globals.Config.LocalPlayer.NoRecoil && !FireAnimationHooked) // allows us to hook it on config load
            {
                FireAnimationHook = new DumbHook();
                FireAnimationHook.Init(typeof(EntityPlayerLocal).GetMethod("OnFired", BindingFlags.Public | BindingFlags.Instance), typeof(Misc).GetMethod("FireAnimation", BindingFlags.Public | BindingFlags.Instance));
                FireAnimationHook.Hook();
                FireAnimationHooked = true;
            }
            if (Globals.Config.LocalPlayer.NoSpread && !AccuracyHooked) // allows us to hook it on config load
            {
                UpdateAccuracyHook = new DumbHook();
                UpdateAccuracyHook.Init(typeof(ItemActionRanged).GetMethod("updateAccuracy", BindingFlags.NonPublic | BindingFlags.Instance), typeof(Misc).GetMethod("Accuracy", BindingFlags.Public | BindingFlags.Instance));
                UpdateAccuracyHook.Hook();
                AccuracyHooked = true;
            }
            #endregion
            if (Globals.LocalPlayer == null)
                return;

             GamePrefs.Set(EnumGamePrefs.CreativeMenuEnabled, true);
            GameStats.Set(EnumGameStats.IsFlyingEnabled, true);
            GamePrefs.Set(EnumGamePrefs.DebugMenuEnabled, true);
            GameStats.Set(EnumGameStats.IsPlayerCollisionEnabled, false);
            GameStats.Set(EnumGameStats.IsCreativeMenuEnabled, true);
            // GamePrefs.GetString(EnumGamePrefs.TelnetPassword)// get rcon password
         //   EffectManager.GetValue
            try
            {
                vp_FPWeapon weapon = Globals.LocalPlayer?.vp_FPWeapon;
                if (weapon)
                {
                    weapon.BobRate = Vector4.zero;
                    weapon.ShakeAmplitude = Vector3.zero;
                    weapon.StepForceScale = 0f;
                    weapon.ShakeSpeed = 0;
                }
                Inventory inventory = Globals.LocalPlayer?.inventory;
                if (inventory != null)
                {
                    ItemActionAttack gun = inventory?.GetHoldingGun();
                    if (gun is ItemActionRanged)
                    {
                     
                    }
                    gun.InfiniteAmmo = true;
                    gun.Velocity.Value = 100000000;
                    gun.Range = 1000000000;
                    gun.BlockRange = 100000000;
                }
            }
            catch { }
  
        }
        [ObfuscationAttribute(Exclude = true)]
        void Start()
        { 
        }
        void Start1()
        {
       /* CreateShot_Silent = new DumbHook();
            CreateShot_Silent.Init(typeof(Block).GetMethod("OnBlockDamaged", BindingFlags.Public | BindingFlags.Instance), typeof(Cheat).GetMethod("OnBlockDamaged", BindingFlags.Public | BindingFlags.Instance));
            CreateShot_Silent.Hook();*/
        }
        public static void KillPlayer(EntityPlayer player)
        {
            DamageSource source = new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.BloodLoss);
            player.DamageEntity(source, 100000000, false, 1);
        }
        void NoSpread(int seed, Transform target)
        {
            
            if (!Globals.Config.LocalPlayer.NoSpread)
            {
          /*      UnityEngine.Random.InitState(seed);
                target.Rotate(0f, 0f, (float)UnityEngine.Random.Range(0, 360));
                target.Rotate(0f, UnityEngine.Random.Range(-this.ProjectileSpread, this.ProjectileSpread), 0f);*/

            }
        }
    }
}
