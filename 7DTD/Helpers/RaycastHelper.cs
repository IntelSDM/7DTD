using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat.Helpers
{
    class RaycastHelper
    {
        private static RaycastHit RaycastHit; // constants.cs
        // stole the mask from the GetExecuteActionTarget projectile raycast and it works like a gem.
        public static bool PlayerPos(EntityPlayer player,Vector3 pos)
        {
            
            return Physics.Linecast(
                Camera.main.transform.position,
                pos,
                out RaycastHit,
                -538750997) && RaycastHit.collider&& RaycastHit.transform.position == pos && RaycastHit.collider.gameObject.transform.root.gameObject == player.gameObject.transform.root.gameObject;
        }
        public static bool ZombiePos(EntityZombie zombie, Vector3 pos)
        {
            return Physics.Linecast(
                Camera.main.transform.position,
                pos,
                out RaycastHit, -538750997) && RaycastHit.collider && RaycastHit.transform.position == pos && RaycastHit.collider.gameObject.transform.root.gameObject == zombie.gameObject.transform.root.gameObject;
       
        }
       
        public static string BarrelRayCastTest(EntityZombie player, Vector3 pos)
        {
            try
            {
                    Physics.Linecast(
                  Camera.main.transform.position,
                  pos,
                  out RaycastHit);
                 return RaycastHit.transform.gameObject.layer.ToString();
            }
            catch
            {
                return "Unkown";
            }
        }
    }
}
