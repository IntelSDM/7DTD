using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat
{
    class Misc : MonoBehaviour
    {
        public static void KillPlayer(EntityPlayer player)
        {
            DamageSource source = new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.BloodLoss);
            player.DamageEntity(source, 100000000, false, 1);
        }
    }
}
