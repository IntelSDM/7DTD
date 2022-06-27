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
        FIX MEMORY LEAK - not caused by misc update, 
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

        [ObfuscationAttribute(Exclude = true)]
        public virtual int Damage(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
        {
            return Dam(_world, _clrIdx, _blockPos, _blockValue, _damagePoints, _entityIdThatDamaged, _bUseHarvestTool, _bBypassMaxDamage, _recDepth);
        }

        public int Dam(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
        {
            if (Globals.Config.LocalPlayer.InstantBreak1)
            {
                _blockValue.Block.CanPickup = true;
                _world.GetGameManager().PickupBlockServer(_clrIdx, _blockPos, _blockValue, 0);
                _blockValue.Block.FallDamage = 0;
            }
            if (Globals.Config.LocalPlayer.InstantBreak2)
            {
                System.Random rand = new System.Random();
           _entityIdThatDamaged = rand.Next(0,10000);
            _bBypassMaxDamage = true;
                _damagePoints = int.MaxValue;
                // _blockValue.Block.IsCollideMelee // setting this would be good
             //   _blockValue.damage = 0;
            }
            if (Globals.Config.LocalPlayer.InstantBreak3)
                _blockValue.Block.MaxDamage = 1;

            ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
            if (chunkCluster == null)
            {
                return 0;
            }
            if (_blockValue.Block.isMultiBlock && _blockValue.ischild)
            {
                Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
                BlockValue block = chunkCluster.GetBlock(parentPos);
                if (block.ischild)
                {
                    /*    Log.Error("Block on position {0} with name '{1}' should be a parent but is not! (6)", new object[]
                        {
                        parentPos,
                        block.Block.blockName
                        });*/
                    return 0;
                }
                return block.Block.OnBlockDamaged(_world, _clrIdx, parentPos, block, _damagePoints, _entityIdThatDamaged, _bUseHarvestTool, _bBypassMaxDamage, _recDepth + 1);
            }
            else
            {
                int num = _blockValue.damage;
                bool flag = num >= _blockValue.Block.MaxDamage;
                num += _damagePoints;
                chunkCluster.InvokeOnBlockDamagedDelegates(_blockPos, _blockValue, _damagePoints, _entityIdThatDamaged);
                Block block2 = _blockValue.Block;
                if (num < 0)
                {
                    if (!_blockValue.Block.UpgradeBlock.isair)
                    {
                        BlockValue blockValue = _blockValue.Block.UpgradeBlock;
                        blockValue = BlockPlaceholderMap.Instance.Replace(blockValue, _world.GetGameRandom(), _blockPos.x, _blockPos.z, false, QuestTags.none);
                        blockValue.rotation = _blockValue.rotation;
                        blockValue.meta = _blockValue.meta;
                        blockValue.damage = 0;
                        Block block3 = blockValue.Block;
                        if (!block3.shape.IsTerrain())
                        {
                            _world.SetBlockRPC(_clrIdx, _blockPos, blockValue);
                            if (chunkCluster.GetTextureFull(_blockPos) != 0L)
                            {
                                GameManager.Instance.SetBlockTextureServer(_blockPos, BlockFace.None, 0, _entityIdThatDamaged);
                            }
                        }
                        else
                        {
                            _world.SetBlockRPC(_clrIdx, _blockPos, blockValue, block3.Density);
                        }
                        DynamicMeshManager.ChunkChanged(_blockPos, _entityIdThatDamaged, _blockValue.type);
                        return blockValue.damage;
                    }
                    if (_blockValue.damage != 0)
                    {
                        _blockValue.damage = 0;
                        _world.SetBlockRPC(_clrIdx, _blockPos, _blockValue);
                    }
                    return 0;
                }
                else
                {
                    if (!flag && num >= block2.MaxDamage)
                    {
                        num -= block2.MaxDamage;
                        DynamicMeshManager.ChunkChanged(_blockPos, _entityIdThatDamaged, _blockValue.type);
                        Block.DestroyedResult destroyedResult = _blockValue.Block.OnBlockDestroyedBy(_world, _clrIdx, _blockPos, _blockValue, _entityIdThatDamaged, _bUseHarvestTool);
                        if (destroyedResult != Block.DestroyedResult.Keep)
                        {
                            if (!_blockValue.Block.DowngradeBlock.isair && destroyedResult == Block.DestroyedResult.Downgrade)
                            {
                                if (_recDepth == 0)
                                {
                                    _blockValue.Block.SpawnDestroyParticleEffect(_world, _blockValue, _blockPos, 1f, _blockValue.Block.tintColor, _entityIdThatDamaged);
                                }
                                BlockValue blockValue2 = _blockValue.Block.DowngradeBlock;
                                blockValue2 = BlockPlaceholderMap.Instance.Replace(blockValue2, _world.GetGameRandom(), _blockPos.x, _blockPos.z, false, QuestTags.none);
                                blockValue2.rotation = _blockValue.rotation;
                                blockValue2.meta = _blockValue.meta;
                                Block block4 = blockValue2.Block;
                                if (!block4.shape.IsTerrain())
                                {
                                    _world.SetBlockRPC(_clrIdx, _blockPos, blockValue2);
                                    if (chunkCluster.GetTextureFull(_blockPos) != 0L)
                                    {
                                        if (_blockValue.Block.RemovePaintOnDowngrade == null)
                                        {
                                            GameManager.Instance.SetBlockTextureServer(_blockPos, BlockFace.None, 0, _entityIdThatDamaged);
                                        }
                                        else
                                        {
                                            for (int i = 0; i < _blockValue.Block.RemovePaintOnDowngrade.Count; i++)
                                            {
                                                GameManager.Instance.SetBlockTextureServer(_blockPos, _blockValue.Block.RemovePaintOnDowngrade[i], 0, _entityIdThatDamaged);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    _world.SetBlockRPC(_clrIdx, _blockPos, blockValue2, block4.Density);
                                }
                                if ((num > 0 && _blockValue.Block.EnablePassThroughDamage) || _bBypassMaxDamage)
                                {
                                    block4.OnBlockDamaged(_world, _clrIdx, _blockPos, blockValue2, num, _entityIdThatDamaged, _bUseHarvestTool, _bBypassMaxDamage, _recDepth + 1);
                                }
                            }
                            else
                            {
                                QuestEventManager.Current.BlockDestroyed(block2, _blockPos);
                                _blockValue.Block.SpawnDestroyParticleEffect(_world, _blockValue, _blockPos, 1f, _blockValue.Block.GetColorForSide(_blockValue, BlockFace.Top), _entityIdThatDamaged);
                                _world.SetBlockRPC(_clrIdx, _blockPos, BlockValue.Air);
                                TileEntityLootContainer tileEntityLootContainer = _world.GetTileEntity(_clrIdx, _blockPos) as TileEntityLootContainer;
                                if (tileEntityLootContainer != null)
                                {
                                    tileEntityLootContainer.OnDestroy();
                                    for (int j = 0; j < LocalPlayerUI.PlayerUIs.Count; j++)
                                    {
                                        if (LocalPlayerUI.PlayerUIs[j].windowManager.IsWindowOpen("looting") && ((XUiC_LootWindow)LocalPlayerUI.PlayerUIs[j].xui.GetWindow("windowLooting").Controller).GetLootBlockPos() == _blockPos)
                                        {
                                            LocalPlayerUI.PlayerUIs[j].windowManager.Close("looting");
                                        }
                                    }
                                    Chunk chunk = _world.GetChunkFromWorldPos(_blockPos) as Chunk;
                                    if (chunk != null)
                                    {
                                        chunk.RemoveTileEntityAt<TileEntityLootContainer>((World)_world, World.toBlock(_blockPos));
                                    }
                                }
                            }
                        }
                        return block2.MaxDamage;
                    }
                    if (_blockValue.damage != num)
                    {
                        _blockValue.damage = num;
                        if (!block2.shape.IsTerrain())
                        {
                            _world.SetBlocksRPC(new List<BlockChangeInfo>
                        {
                            new BlockChangeInfo(_blockPos, _blockValue, false, true)
                        });
                        }
                        else
                        {
                            sbyte density = _world.GetDensity(_clrIdx, _blockPos);
                            sbyte b = (sbyte)Utils.FastMin(-1f, (float)MarchingCubes.DensityTerrain * (1f - (float)num / (float)block2.MaxDamage));
                            if ((_damagePoints > 0 && b > density) || (_damagePoints < 0 && b < density))
                            {
                                _world.SetBlockRPC(_clrIdx, _blockPos, _blockValue, b);
                            }
                            else
                            {
                                _world.SetBlockRPC(_clrIdx, _blockPos, _blockValue);
                            }
                        }
                    }
                    return _blockValue.damage;
                }
            }
        }

        [ObfuscationAttribute(Exclude = true)]
        public void FireAnimation()
        {

        }
        [ObfuscationAttribute(Exclude = true)]
        public float Accuracy(ItemActionData _actionData, bool _isAimingGun)
        {
            return Accuracy1(_actionData, _isAimingGun);
        }
        public float Accuracy1(ItemActionData _actionData, bool _isAimingGun)
        {
            (_actionData as ItemActionRanged.ItemActionDataRanged).lastAccuracy = 0;

            return (_actionData as ItemActionRanged.ItemActionDataRanged).lastAccuracy;
        }
        [ObfuscationAttribute(Exclude = true)]
        public float GetRange(ItemActionData _actionData)
        {
            return (GetRange1(_actionData));
        }
        public float GetRange1(ItemActionData _actionData)
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
                UpdateAccuracyHook.Init(typeof(ItemActionRanged).GetMethod("updateAccuracy", BindingFlags.NonPublic | BindingFlags.Instance), typeof(Misc).GetMethod("Accuracy", BindingFlags.Public | BindingFlags.Instance));
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
            Speedhack();
            SetProperties();
           Noclip();
        //    Worlds();
            #region Hooks
            // some of these should just be done in a start function but i kinda want them all in 1 area so i am not splitting them up
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
                UnlimitedRangeHook.Init(typeof(Block).GetMethod("OnBlockDamaged", BindingFlags.Public | BindingFlags.Instance), typeof(Misc).GetMethod("Damage", BindingFlags.Public | BindingFlags.Instance));
                UnlimitedRangeHook.Hook();
                UnlimitedRangeHooked = true;
            }
            if (!BlockDamageHooked)
            {
                BlockDamage = new DumbHook();
                BlockDamage.Init(typeof(ItemActionRanged).GetMethod("GetRange", BindingFlags.Public | BindingFlags.Instance), typeof(Misc).GetMethod("GetRange", BindingFlags.Public | BindingFlags.Instance));
                BlockDamage.Hook();
                BlockDamageHooked = true;
            }
          
            #endregion

            if (Globals.LocalPlayer == null)
                return;
      

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
                    if (Globals.Config.LocalPlayer.WeaponFovChanger)
                    {
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

            Globals.LocalPlayer.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 10, player.transform.position.z);
        }
        void Speedhack()
        {
            if (Globals.LocalPlayer == null)
                return;
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
        void Noclip()
        {
            if (Globals.LocalPlayer == null)
                return;
            if (!Globals.Config.LocalPlayer.BtecNoclip)
                return;
            if (!Input.GetKey(Globals.Config.LocalPlayer.NoclipKey))
                return;
            Globals.LocalPlayer.inWaterPercent = 100;
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
        public static void ClipboardToString(out string text)
        {
            string systemCopyBuffer = GUIUtility.systemCopyBuffer;
            text = systemCopyBuffer;
        }
        void SetProperties()
        {
            if (Globals.LocalPlayer == null)
                return;
            if (Globals.Config.LocalPlayer.UnlimitedStamina)
            {
                Globals.LocalPlayer.Stats.Stamina.Value = 100000f;
                Globals.LocalPlayer.Stamina = 100000f;
                Globals.LocalPlayer.AddStamina(100000f);
                Globals.LocalPlayer.classMaxStamina = 100000;
            }
            if (Globals.Config.LocalPlayer.UnlimitedHunger)
            {
                Globals.LocalPlayer.Stats.Food.Value = 100000f;
                Globals.LocalPlayer.classMaxFood = 100000;
 
            }
            if (Globals.Config.LocalPlayer.UnlimitedThirtst)
            {
                Globals.LocalPlayer.Stats.Water.Value = 100000f;
                Globals.LocalPlayer.Water = 100000f;
                Globals.LocalPlayer.classMaxWater = 100000;

            }
            if (Globals.Config.LocalPlayer.InstantHealth)
            {
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
                if (Globals.Config.LocalPlayer.SpoofName)
                {
                    if(Globals.LocalPlayer.EntityName != Name)
                    Globals.LocalPlayer.SetEntityName(Name);
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
            SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync($"{text}", null);
        }
        public static void ClearDebuff()
        {
            if (Globals.LocalPlayer == null)
                return;
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
            ExecuteConsoleCommand(text);
        }
        #endregion
        #region World
        void Worlds()
        {
            if (Globals.LocalPlayer == null)
                return;
            if (Globals.Config.LocalPlayer.CreativeMenu)
            {
       
            }
            if (Globals.Config.LocalPlayer.DebugMenu)
            {
            
            }
            if (Globals.Config.LocalPlayer.FarInteract)
            {
                Constants.cDigAndBuildDistance = Globals.Config.LocalPlayer.FarInteractDistance;
                Constants.cCollectItemDistance = Globals.Config.LocalPlayer.FarInteractDistance;
            //    Constants.cBuildIntervall = 0.1f;
            }
            if (Globals.Config.LocalPlayer.LandClaim)
            {
                if (!GameStats.GetBool(EnumGameStats.LandClaimOnlineDurabilityModifier))
                    GameStats.Set(EnumGameStats.LandClaimOnlineDurabilityModifier, 1);
                if (!GameStats.GetBool(EnumGameStats.LandClaimOfflineDurabilityModifier))
                    GameStats.Set(EnumGameStats.LandClaimOfflineDurabilityModifier, 1);

            }

        }
        public static void CreativeMenu()
        {
            if (Globals.LocalPlayer == null)
                return;
            //      if(!GamePrefs.GetBool(EnumGamePrefs.CreativeMenuEnabled))
            GamePrefs.Set(EnumGamePrefs.CreativeMenuEnabled, true);
            //    if (!GameStats.GetBool(EnumGameStats.IsCreativeMenuEnabled))
            GameStats.Set(EnumGameStats.IsCreativeMenuEnabled, true);
        }
        public static void DebugMenu()
        {
            if (Globals.LocalPlayer == null)
                return;
            if (!GameStats.GetBool(EnumGameStats.IsFlyingEnabled))
                GameStats.Set(EnumGameStats.IsFlyingEnabled, true);
            if (!GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled))
                GamePrefs.Set(EnumGamePrefs.DebugMenuEnabled, true);
            if (!GameStats.GetBool(EnumGameStats.IsPlayerCollisionEnabled))
                GameStats.Set(EnumGameStats.IsPlayerCollisionEnabled, false);
        }
        public static void KillEveryone()
        {
            foreach (EntityPlayer player in GameManager.Instance.World.Players.list)
            {
                if (player == null && !player.IsAlive())
                    continue;
                DamageSource source = new DamageSource(EnumDamageSource.External, EnumDamageTypes.VehicleInside);
                player.DamageEntity(source,1000000,false,1);
            }
        }
        public static void KillEveryoneElse()
        {
            foreach (EntityPlayer player in Esp.Player.PlayerList)
            {
                if (player == null && !player.IsAlive())
                    continue;
                DamageSource source = new DamageSource(EnumDamageSource.External, EnumDamageTypes.Sprain);
                player.DamageEntity(source, 1000000, false, 1);
            }
        }
        public static void InstantCraft()
        {
            if (Globals.LocalPlayer == null)
                return;
            try
            {
                foreach (Recipe recipe in CraftingManager.GetAllRecipes())
                {
                    CraftingManager.UnlockRecipe(recipe, Globals.LocalPlayer);
                    recipe.ingredients.Clear();
                    recipe.craftingTime = 0.1f;
                }
            }
            catch (Exception e)
            {
              
            }
        }
        #endregion

    }
}
