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
    class OnBlockDamaged : MonoBehaviour
    {
        public static DumbHook Hook;
        public void Start()
        {

                Hook = new DumbHook();
                Hook.Init(typeof(Block).GetMethod("OnBlockDamaged", BindingFlags.Public | BindingFlags.Instance), typeof(OnBlockDamaged).GetMethod("HookFunction", BindingFlags.Public | BindingFlags.Instance));
                Hook.Hook();
            
        }
        public int HookFunction(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
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

            Hook.Unhook();


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
            Hook.OriginalMethod.Invoke(this, parameters);

            Hook.Hook();
            return 0;

        }
    }
}
