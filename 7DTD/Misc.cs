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
        // public virtual float GetRange(ItemActionData _actionData) // longer range
        // protected virtual Vector3 getDirectionOffset hook this and return target playerpos

      
        #region Hook Methods
        public static DumbHook FireAnimationHook;
        public bool FireAnimationHooked = false;
        public static DumbHook UpdateAccuracyHook;
        public bool AccuracyHooked = false;
        public static DumbHook UnlimitedRangeHook;
        public bool UnlimitedRangeHooked = false;
        public static DumbHook SilentAimHook;
        public bool SilentAimHooked = false;
        public void FireAnimation()
        {

        }
        public float Accuracy(ItemActionData _actionData, bool _isAimingGun)
        {
            (_actionData as ItemActionRanged.ItemActionDataRanged).lastAccuracy = 0;

            return (_actionData as ItemActionRanged.ItemActionDataRanged).lastAccuracy;
        }
        public float GetRange(ItemActionData _actionData)
        {
            vp_FPWeapon weapon = Globals.LocalPlayer?.vp_FPWeapon;
            Inventory inventory = Globals.LocalPlayer?.inventory;
            ItemActionAttack gun = inventory?.GetHoldingGun();
            ItemActionRanged action = gun as ItemActionRanged;
            if (!Globals.Config.LocalPlayer.UnlimitedRange)
                return EffectManager.GetValue(PassiveEffects.MaxRange, _actionData.invData.itemValue, action.Range, _actionData.invData.holdingEntity, null, default(FastTags), true, true, true, true, 1, true);
            else
                return 100000000000;
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
        bool silentaim = true;
        public Vector3 fireShot(int _shotIdx, ItemActionRanged.ItemActionDataRanged _actionData)
        {
            // getlookray might be a bit better, we just need the ray.origin - direction to be our target's position
            return Vector3.zero;
        }
        public  Ray GetLookRay()
        {
            EntityAlive ent = Globals.LocalPlayer as EntityAlive;
            return new Ray(ent.position + new Vector3(0f, ent.GetEyeHeight(), 0f), Vector3.zero);
         //   return new Ray(ent.position + new Vector3(0f, ent.GetEyeHeight(), 0f), ent.GetLookVector());
        }
        #endregion
        [ObfuscationAttribute(Exclude = true)]
        void Update()
        {
            Update1();
        }
        void Update1()
        {
            Speedhack();
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
            if ( !UnlimitedRangeHooked) 
            {
                UnlimitedRangeHook = new DumbHook();
                UnlimitedRangeHook.Init(typeof(ItemActionRanged).GetMethod("GetRange", BindingFlags.Public | BindingFlags.Instance), typeof(Misc).GetMethod("GetRange", BindingFlags.Public | BindingFlags.Instance));
                UnlimitedRangeHook.Hook();
                UnlimitedRangeHooked = true;
            }
            if (!SilentAimHooked) 
            {
                SilentAimHook = new DumbHook();
                SilentAimHook.Init(typeof(EntityAlive).GetMethod("GetLookRay", BindingFlags.Public | BindingFlags.Instance), typeof(Misc).GetMethod("GetLookRay", BindingFlags.Public | BindingFlags.Instance));
                SilentAimHook.Hook();
                SilentAimHooked = true;
            }
            #endregion

            if (Globals.LocalPlayer == null)
                return;

             GamePrefs.Set(EnumGamePrefs.CreativeMenuEnabled, true);
            GameStats.Set(EnumGameStats.IsFlyingEnabled, true);
            GamePrefs.Set(EnumGamePrefs.DebugMenuEnabled, true);
            GameStats.Set(EnumGameStats.IsPlayerCollisionEnabled, false);
            GameStats.Set(EnumGameStats.IsCreativeMenuEnabled, true);

            #region LocalPlayer
            try
            {
                vp_FPWeapon weapon = Globals.LocalPlayer?.vp_FPWeapon;
                if (weapon)
                {
                    if (Globals.Config.LocalPlayer.NoViewBob)
                    {
                        weapon.BobRate = Vector4.zero;
                        weapon.ShakeAmplitude = Vector3.zero;
                        weapon.StepForceScale = 0f;
                        weapon.ShakeSpeed = 0;
                    }
                }
                Inventory inventory = Globals.LocalPlayer?.inventory;
                if (inventory != null)
                {
                    ItemActionAttack gun = inventory?.GetHoldingGun();
                    if (gun is ItemActionRanged)
                    {
                     
                    }
                    if (Globals.Config.LocalPlayer.UnlimitedAmmo)
                    {
                        gun.InfiniteAmmo = true;
                        gun.Velocity.Value = 100000000;
                        gun.Range = 1000000000;
                        gun.AutoFire.Value = true;
                        gun.BlockRange = 100000000;
                    }
                }
            }
            catch { }
            #endregion

        }

        [ObfuscationAttribute(Exclude = true)]
        void Start()
        { 
        }
        void Start1()
        {
    
        }
        public static void KillPlayer(EntityPlayer player)
        {
            DamageSource source = new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.BloodLoss);
            player.DamageEntity(source, 100000000, false, 1);
        }
        #region Movement
        public static void TeleportToPlayer(EntityPlayer player)
        { 
        }
        void Speedhack()
        {
            if (!Globals.Config.LocalPlayer.Speedhack)
                return;
                if (Input.GetKey(Globals.Config.LocalPlayer.SpeedKey))
                {
                Time.timeScale = Globals.Config.LocalPlayer.SpeedAmount;
                }
                else
                {
                    Time.timeScale = 1;
                }
            

        }
        #endregion
        #region Skills
        public static void SetLevel(int level)
        {
            if (Globals.LocalPlayer == null)
                return;
            Globals.LocalPlayer.Progression.Level = level;
        }
        public static void SetKills(int kills)
        {
            if (Globals.LocalPlayer == null)
                return;
            Globals.LocalPlayer.KilledPlayers = kills;
        }
        public static void SetZombieKills(int kills)
        {
            if (Globals.LocalPlayer == null)
                return;
            Globals.LocalPlayer.KilledZombies = kills;
        }
        public static void SetSkillPoints(int points)
        {
            if (Globals.LocalPlayer == null)
                return;
            Globals.LocalPlayer.Progression.SkillPoints = points;
        }
        public static void SetDeaths(int deaths)
        {
            if (Globals.LocalPlayer == null)
                return;
            //   GameSparksCollector.IncrementCounter(GameSparksCollector.GSDataKey.PlayerDeathCauses, text, 1, true, GameSparksCollector.GSDataCollection.SessionUpdates);
            EntityAlive alive = Globals.LocalPlayer as EntityAlive;
            alive.Died = deaths;
        }
        #endregion

    }
}
