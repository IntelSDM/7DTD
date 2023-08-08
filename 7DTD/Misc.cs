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

        /*
         * item time
             ulong worldTime = GameManager.Instance.World.GetWorldTime();
                             GameManager.Instance.World.SetTimeJump(worldTime + 6000UL, false);
                             Debug.Log("Added 6hr to World Time...");
        Draw Players and bases on map
        open locked crates
        place items in blocked zones
        isfriendswith
        drawmapicon
        nenu x and y slider and credits
         */


        #region Hook Methods
        public static DumbHook FireAnimationHook;
        public static bool FireAnimationHooked = false;
        public static DumbHook UpdateAccuracyHook;
        public static bool AccuracyHooked = false;
        public static DumbHook UnlimitedRangeHook;
        public bool UnlimitedRangeHooked = false;
        public static DumbHook BlockDamage;
        public bool BlockDamageHooked = false;
        public static DumbHook IsOwnerHook;
        public bool IsOwnerHooked = false;
        private static List<EntityPlayer> KillList = new List<EntityPlayer>();
       
        [ObfuscationAttribute(Exclude = true)]
        public bool IsOwner(PlatformUserIdentifierAbs _userIdentifier)
        {
            if (Globals.Config.LocalPlayer.OwnsVehicle)
                return true; // set the user as owning the car
            else
            {
                IsOwnerHook.Unhook();
                object[] parameters = new object[]
                  {
                    _userIdentifier,
                   
                  };
                object result = IsOwnerHook.OriginalMethod.Invoke(this, parameters);
                IsOwnerHook.Hook(); // call the original
                return false; // this wont be called
            }
        }
      
        // recreate the block damage function so we can allow instant break
        public int OnBlockDamaged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
        {
            if (Globals.Config.LocalPlayer.InstantBreak1)
            {
                // method 1, pick up the block
                _blockValue.Block.CanPickup = true;
                _world.GetGameManager().PickupBlockServer(_clrIdx, _blockPos, _blockValue, 0);
                _blockValue.Block.FallDamage = 0;
            }
            if (Globals.Config.LocalPlayer.InstantBreak2)
            {

                _entityIdThatDamaged = 0;
                 _bBypassMaxDamage = true;
                  _damagePoints = _blockValue.Block.MaxDamage;
         
            }
            if (Globals.Config.LocalPlayer.InstantBreak3)
            {
                // set the block health to 1
                _bBypassMaxDamage = true;
                _damagePoints = 1;
                _blockValue.Block.MaxDamage = 1;
            }

            BlockDamage.Unhook();


            object[] parameters = new object[]
               {
                    _world,
                    _clrIdx,
                    _blockPos,
                    _blockValue,
                    _damagePoints,
                    _entityIdThatDamaged,
                    _attackHitInfo,
                    _bUseHarvestTool,
                    _bBypassMaxDamage,
                    _recDepth
               };
           BlockDamage.OriginalMethod.Invoke(this, parameters);

            BlockDamage.Hook();
            return 0;

        }

        [ObfuscationAttribute(Exclude = true)]
        public void FireAnimation()
        {
            // just null it
        }
        [ObfuscationAttribute(Exclude = true)]
        public float Accuracy(ItemActionData _actionData, bool _isAimingGun)
        {
            return Accuracy1(_actionData, _isAimingGun);
        }
        public float Accuracy1(ItemActionData _actionData, bool _isAimingGun)
        {
            (_actionData as ItemActionRanged.ItemActionDataRanged).lastAccuracy = 0; // set this incase its used elsewhere

            return (_actionData as ItemActionRanged.ItemActionDataRanged).lastAccuracy; // could return 0 but ehh
        }
        [ObfuscationAttribute(Exclude = true)]
        public float GetRange(ItemActionData _actionData)
        {
            return (GetRange1(_actionData));
        }
        public float GetRange1(ItemActionData _actionData)
        {
            // return actual range
            vp_FPWeapon weapon = Globals.LocalPlayer?.vp_FPWeapon;
            Inventory inventory = Globals.LocalPlayer?.inventory;
            ItemActionAttack gun = inventory?.GetHoldingGun();
            ItemActionRanged action = gun as ItemActionRanged;
            if (!Globals.Config.LocalPlayer.UnlimitedRange)
                return EffectManager.GetValue(PassiveEffects.MaxRange, _actionData.invData.itemValue, action.Range, _actionData.invData.holdingEntity, null, default(FastTags), true, true, true, true, 1, true);
            else
                return 100000000000; // return firing range of infinite
        }
        public static void EnableNoRecoil()
        {
            if (!FireAnimationHooked)
            {
                FireAnimationHook = new DumbHook();
                FireAnimationHook.Init(typeof(EntityPlayerLocal).GetMethod("OnFired", BindingFlags.Public | BindingFlags.Instance), typeof(Misc).GetMethod("FireAnimation", BindingFlags.Public | BindingFlags.Instance));
                FireAnimationHook.Hook();
                FireAnimationHooked = true;
            }
        }

        public static void EnableNoSpread()
        {
            if (!AccuracyHooked)
            {
                UpdateAccuracyHook = new DumbHook();
                UpdateAccuracyHook.Init(typeof(ItemActionRanged).GetMethod("updateAccuracy"), typeof(Misc).GetMethod("Accuracy"));
                UpdateAccuracyHook.Hook();
                AccuracyHooked = true;
            }
        }
        bool silentaim = true;
   
 
        #endregion
        [ObfuscationAttribute(Exclude = true)]
        void Update()
        {
            Update1();
        }
        void Update1()
        {
            
       
            try
            {
                DamageSource source = new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.BloodLoss);
                foreach(EntityPlayer player in KillList)
                    if(player.IsAlive()) // check if the player is alive
                player.DamageEntity(source, 100000000, false, 1); // kill the player if they are on our kill list
            }
            catch { }
            Speedhack();
            SetProperties();
           Noclip();
            Worlds();
            #region Hooks
            // initiating hooks
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
            if (!BlockDamageHooked)
            {
                BlockDamage = new DumbHook();
                BlockDamage.Init(typeof(Block).GetMethod("OnBlockDamaged", BindingFlags.Public | BindingFlags.Instance), typeof(Misc).GetMethod("OnBlockDamaged", BindingFlags.Public | BindingFlags.Instance));
                BlockDamage.Hook();
                BlockDamageHooked = true;
            }
         
            if (!IsOwnerHooked)
            {
               IsOwnerHook = new DumbHook();
                IsOwnerHook.Init(typeof(EntityVehicle).GetMethod("IsOwner", BindingFlags.Public | BindingFlags.Instance), typeof(Misc).GetMethod("IsOwner", BindingFlags.Public | BindingFlags.Instance));
                IsOwnerHook.Hook();
                IsOwnerHooked = true;
            }
          
            #endregion

            if (Globals.LocalPlayer == null)
                return;
      

            #region LocalPlayer
            try
            {
                if (Globals.Config.LocalPlayer.CameraFovChanger)
                    Globals.MainCamera.fieldOfView = Globals.Config.LocalPlayer.CameraFov; // change camera fov
                vp_FPWeapon weapon = Globals.LocalPlayer?.vp_FPWeapon;
                if (weapon)
                {
                    if (Globals.Config.LocalPlayer.NoViewBob)
                    {
                        // remove view bob by nulling and 0ing
                        weapon.BobRate = Vector4.zero;
                        weapon.ShakeAmplitude = Vector3.zero;
                        weapon.StepForceScale = 0f;
                        weapon.ShakeSpeed = 0;
                    }
                    if (Globals.Config.LocalPlayer.WeaponFovChanger)
                    {
                        // change weapon position in weaponfov
                        weapon.RenderingFieldOfView = Globals.Config.LocalPlayer.WeaponFov;
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
                        // not all of this is to do with unlimited ammo
                        gun.InfiniteAmmo = true; // set ammo
                        gun.Velocity.Value = 100000000; // set speed
                        gun.Range = 1000000000; // set range on players
                        gun.AutoFire.Value = true; // full auto
                        gun.BlockRange = 100000000; // block max attack range
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
            System.Random rand = new System.Random();
            int randsource = 0;
            int randtype = 0;
            randsource = rand.Next(0, 1);
            randtype = rand.Next(0, 27);
            if (randtype == 21)
                randtype++; 
            // server plugins and admin only check for the concuss damage source, you should randomize the damage source to be different for all different damage sources and types.
            DamageSource source = new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.BloodLoss);
            player.DamageEntity(source, player.Health, false, 1); // do it for their entire health, nothing more nothing less. you could do 1 health at a time to troll.
        }
        public static void StartConstantlyKillPlayer(EntityPlayer player)
        {
            KillList.Add(player);
        }
        public static void StopConstantlyKillPlayer(EntityPlayer player)
        {
            KillList.Remove(player);
        }
        public static void SpoofName(EntityPlayer player)
        {
            Name = player.EntityName;
        }
        public static void SpoofID(EntityPlayer player)
        {
           Entity = player.entityId;
        }
        public static void SpoofStats(EntityPlayer player)
        {
            // set stats to local player from another player's pointer
            Globals.LocalPlayer.Progression.Level = player.Progression.Level;
            Globals.LocalPlayer.Progression.SkillPoints = player.Progression.SkillPoints;
            Globals.LocalPlayer.Progression.ExpToNextLevel = player.Progression.ExpToNextLevel;
            Globals.LocalPlayer.KilledPlayers = player.KilledPlayers;
            Globals.LocalPlayer.KilledZombies = player.KilledZombies;
            EntityAlive alive = Globals.LocalPlayer as EntityAlive;
            EntityAlive alive2 = player as EntityAlive;
            alive.Died = alive2.Died;
            Globals.LocalPlayer.distanceWalked = player.distanceWalked;
            Globals.LocalPlayer.distanceWalked = player.distanceSwam;
            Globals.LocalPlayer.distanceClimbed = player.distanceClimbed;
            Globals.LocalPlayer.totalTimePlayed = player.totalTimePlayed;
            Globals.LocalPlayer.totalItemsCrafted = player.totalItemsCrafted;
        


        }
        #region Movement
        public static void TeleportToPlayer(EntityPlayer player)
        {

            Globals.LocalPlayer.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 10, player.transform.position.z); // +10 y so they cant see us teleporting to them.
        }
        void Speedhack()
        {
            if (Globals.LocalPlayer == null)
                return;
            if (!Globals.Config.LocalPlayer.Speedhack)
                return;
                if (Input.GetKey(Globals.Config.LocalPlayer.SpeedKey))
                {
                Time.timeScale = Globals.Config.LocalPlayer.SpeedAmount; // set timescale
                }
                else
                {
                    Time.timeScale = 1; // back to default
                }
            

        }
        void Noclip()
        {
            if (Globals.LocalPlayer == null)
                return;
            if (!Globals.Config.LocalPlayer.BtecNoclip)
                return; // is noclip on
            if (!Input.GetKey(Globals.Config.LocalPlayer.NoclipKey))
                return; // is noclip key pressed
            // controls for noclip
            if (Input.GetKey(KeyCode.W))
                Globals.LocalPlayer.transform.position = Globals.LocalPlayer.transform.position + Camera.main.transform.forward * Globals.Config.LocalPlayer.NoclipSpeed;
            if (Input.GetKey(KeyCode.S))
                Globals.LocalPlayer.transform.position = Globals.LocalPlayer.transform.position - Camera.main.transform.forward * Globals.Config.LocalPlayer.NoclipSpeed;
            if (Input.GetKey(KeyCode.A))
                Globals.LocalPlayer.transform.position = Globals.LocalPlayer.transform.position - Camera.main.transform.right * Globals.Config.LocalPlayer.NoclipSpeed;
            if (Input.GetKey(KeyCode.D))
                Globals.LocalPlayer.transform.position = Globals.LocalPlayer.transform.position + Camera.main.transform.right * Globals.Config.LocalPlayer.NoclipSpeed;
            if (Input.GetKey(KeyCode.Space))
                Globals.LocalPlayer.transform.position = Globals.LocalPlayer.transform.position + Camera.main.transform.up * Globals.Config.LocalPlayer.NoclipSpeed;
            if (Input.GetKey(KeyCode.LeftControl))
                Globals.LocalPlayer.transform.position = Globals.LocalPlayer.transform.position - Camera.main.transform.up * Globals.Config.LocalPlayer.NoclipSpeed;
        }
        #endregion
        #region Skills
        // setting the skills for the local player to a set amount
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
        public static void SetDistanceTraveled(float distance)
        {
            if (Globals.LocalPlayer == null)
                return;
            Globals.LocalPlayer.distanceWalked = distance;
        }
        public static void SetTotalItemsCrafted(int items)
        {
            if (Globals.LocalPlayer == null)
                return;
            Globals.LocalPlayer.totalItemsCrafted = (uint)items;
        }
        public static void SetTotalTimePlayed(int seconds)
        {
            if (Globals.LocalPlayer == null)
                return;
            int hours = (seconds * 60) * 60;
            Globals.LocalPlayer.totalTimePlayed = hours;
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
        #region Properties
        float NextClear = 0;
        public static string Name = Globals.LocalPlayer?.EntityName;
        public static int Entity = 0;
        private bool EntitySet = false;
        public static void ClipboardToString(out string text)
        {
            string systemCopyBuffer = GUIUtility.systemCopyBuffer;
            text = systemCopyBuffer;
        }
        void SetProperties()
        {
            if (Globals.LocalPlayer == null)
                return;
            if (!EntitySet)
            {
                Entity = Globals.LocalPlayer.entityId;
                EntitySet = true; // set default entity id
            }
            if (Globals.Config.LocalPlayer.SpoofID && EntitySet)
                Globals.LocalPlayer.entityId = Entity; // set to random entity id
            if (Globals.Config.LocalPlayer.UnlimitedStamina)
            {
                // set unlimited stamina values
                Globals.LocalPlayer.Stats.Stamina.Value = 100000f;
                Globals.LocalPlayer.Stamina = 100000f;
                Globals.LocalPlayer.AddStamina(100000f);
                Globals.LocalPlayer.classMaxStamina = 100000;
            }
            if (Globals.Config.LocalPlayer.UnlimitedHunger)
            {
                // set hunger
                Globals.LocalPlayer.Stats.Food.Value = 100000f;
                Globals.LocalPlayer.classMaxFood = 100000;
 
            }
            if (Globals.Config.LocalPlayer.UnlimitedThirtst)
            {
                // set thirst values
                Globals.LocalPlayer.Stats.Water.Value = 100000f;
                Globals.LocalPlayer.Water = 100000f;
                Globals.LocalPlayer.classMaxWater = 100000;

            }
            if (Globals.Config.LocalPlayer.InstantHealth)
            {
                // instantly heal to max
                Globals.LocalPlayer.Stats.Health.Value = 10000000f;
                Globals.LocalPlayer.Health = 10000;
                Globals.LocalPlayer.AddHealth(10000000);
                Globals.LocalPlayer.fallDistance = 0;
                Globals.LocalPlayer.ClearStun();
                Globals.LocalPlayer.classMaxHealth = 100000;
                Globals.LocalPlayer.Stats.Health.MaxModifier = 1000000000;

            }
            try
            {
                if (Globals.Config.LocalPlayer.RandomlySpoofName)
                {
                    foreach (EntityPlayer player in Esp.Player.PlayerList)
                        if(Name != player.EntityName)
                        Name = player.EntityName; // keep changing your name so no one can see who you are, could add a slight delay
                }
                if (Globals.Config.LocalPlayer.SpoofName)
                {
                    if(Globals.LocalPlayer.EntityName != Name)
                    Globals.LocalPlayer.SetEntityName(Name); // set the name while logging the original
                }
                
            }
            catch { }
          

        }
        #endregion
        #region Console
        public static void ExecuteCommandFromClipboard()
        {
            if (Globals.LocalPlayer == null)
                return;
            string systemCopyBuffer = GUIUtility.systemCopyBuffer;
            ExecuteConsoleCommand(systemCopyBuffer);
        }
        private static void ExecuteConsoleCommand(string text)
        {
            if (Globals.LocalPlayer == null)
                return;
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"{text}", null); // sdtd console singleton and execute our commands.
        }
        public static void ClearDebuff()
        {
            if (Globals.LocalPlayer == null)
                return;
            // execute the debuff commands
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffInfectionCatch", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffAbrasionCatch", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffLegSprainedCHTrigger", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffLegBroken", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffArmSprainedCHTrigger", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffArmBroken", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffFatiguedTrigger", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffInjuryBleedingTwo", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffLaceration", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffInjuryStunned01CHTrigger", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffInjuryStunned01Cooldown", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffInjuryConcussion", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffInjuryBleedingOne", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffInjuryBleedingBarbedWire", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffInjuryBleeding", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffInjuryBleedingParticle", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffPlayerFallingDamage", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffFatigued", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffStayDownKO", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffInjuryCrippled01", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffInjuryUnconscious", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffBatterUpSlowDown", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffRadiation03", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffNearDeathTrauma", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffDysenteryCatchFood", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffDysenteryCatchDrink", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffDysenteryMain", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffIllPneumonia00", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffIllPneumonia01", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffInfectionMain", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffInfection04", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffStatusHungry01", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffStatusHungry02", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffStatusHungry03", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffStatusThirsty01", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffStatusThirsty02", null);
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"debuff buffStatusThirsty03", null);
        }
        public static void GiveItemFromClipboard(int amount)
        {
            string systemCopyBuffer = GUIUtility.systemCopyBuffer;
            string text = $"giveself {systemCopyBuffer} 6 {amount.ToString()} true";
            ExecuteConsoleCommand(text); // bypass hiding items on item list.
        }
        #endregion
        #region World
        void Worlds()
        {
            if (Globals.LocalPlayer == null)
                return;
            try
            {
                if (Globals.Config.LocalPlayer.AllahMode)
                {
                    System.Random rand = new System.Random();
                    GameManager.Instance.persistentLocalPlayer.EntityId = rand.Next(0, 1000000);
                    GameManager.Instance.persistentLocalPlayer.PlayerName = rand.Next(0, 1000000).ToString();
                    Globals.LocalPlayer.entityId = rand.Next(100000, 100000000); // spam change our entity id so the server has no idea who we are, prevents the server banning you somehow.
                }
            }
            catch { }
            try
            {
                if (Globals.Config.LocalPlayer.CreativeMenu)
                {
                    if (!GamePrefs.GetBool(EnumGamePrefs.CreativeMenuEnabled))
                        GamePrefs.Set(EnumGamePrefs.CreativeMenuEnabled, true); // set creative mode
                    if (!GameStats.GetBool(EnumGameStats.IsCreativeMenuEnabled))
                        GameStats.Set(EnumGameStats.IsCreativeMenuEnabled, true); 
                }
                if (Globals.Config.LocalPlayer.DebugMenu)
                {
                    // set debug mode
                    if (!GameStats.GetBool(EnumGameStats.IsFlyingEnabled))
                        GameStats.Set(EnumGameStats.IsFlyingEnabled, true);
                    if (!GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled))
                        GamePrefs.Set(EnumGamePrefs.DebugMenuEnabled, true);
                    if (!GameStats.GetBool(EnumGameStats.IsPlayerCollisionEnabled))
                        GameStats.Set(EnumGameStats.IsPlayerCollisionEnabled, false); // disable noclip to prevent bans
                }
                if (Globals.Config.LocalPlayer.FarInteract)
                {
                    //https://www.mpgh.net/forum/showthread.php?t=1421801
                    Constants.cDigAndBuildDistance = Globals.Config.LocalPlayer.FarInteractDistance;
                    Constants.cCollectItemDistance = Globals.Config.LocalPlayer.FarInteractDistance; 
            
                    //    Constants.cBuildIntervall = 0.1f;
                }
                if (Globals.Config.LocalPlayer.LandClaim)
                {
                    // turns off land claim durability modifier
                    if (!GameStats.GetBool(EnumGameStats.LandClaimOnlineDurabilityModifier))
                        GameStats.Set(EnumGameStats.LandClaimOnlineDurabilityModifier, 1);
                    if (!GameStats.GetBool(EnumGameStats.LandClaimOfflineDurabilityModifier))
                        GameStats.Set(EnumGameStats.LandClaimOfflineDurabilityModifier, 1);

                }
            }
            catch { }
        }
       
        public static void KillEveryone()
        {
            // kill the entire server, helps you blend in
            foreach (EntityPlayer player in GameManager.Instance.World.Players.list)
            {
                if (player == null && !player.IsAlive())
                    continue; // check if player is killable
                DamageSource source = new DamageSource(EnumDamageSource.External, EnumDamageTypes.VehicleInside);
                player.DamageEntity(source,1000000,false,1); // kill them
            }
        }
        public static void KillEveryoneElse()
        {
            // kills everyone but you 
            foreach (EntityPlayer player in Esp.Player.PlayerList)
            {
                if (player == null && !player.IsAlive())
                    continue; // check if player is killable
                DamageSource source = new DamageSource(EnumDamageSource.External, EnumDamageTypes.Sprain); 
                player.DamageEntity(source, 1000000, false, 1); // kill
            }
        }
        public static void InstantCraft()
        {
            // allows you to craft instantly and at no cost and without blueprints
            if (Globals.LocalPlayer == null)
                return;
            try
            {
                foreach (Recipe recipe in CraftingManager.GetAllRecipes())
                {
                    CraftingManager.UnlockRecipe(recipe, Globals.LocalPlayer); // loop recipes and unlock them all
                    recipe.ingredients.Clear(); // remove the ingredients from the list
                    recipe.craftingTime = 0.1f; // set crafting time 
                }
            }
            catch (Exception e)
            {
              
            }
        }
        #endregion

    }
}
