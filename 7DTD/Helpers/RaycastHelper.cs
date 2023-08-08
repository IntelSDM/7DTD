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

        public static bool VisbilityCheck(EntityAlive entity, Vector3 pos)
        {
            // -538750997 is from the game's project raycast mask.
            int modelLayer = Globals.LocalPlayer.GetModelLayer();
            Globals.LocalPlayer.SetModelLayer(2, false, null);
            bool flag = Physics.Linecast(
                            Globals.LocalPlayer.cameraTransform.transform.position,
                            pos,
                            out RaycastHit, -538750997) &&
                        RaycastHit.collider &&
                        Vector3.Distance(RaycastHit.transform.position, pos) <= 2 &&
                        RaycastHit.collider.gameObject.transform.root.gameObject == entity.gameObject.transform.root.gameObject;
            Globals.LocalPlayer.SetModelLayer(modelLayer, false, null);

            return flag;
        }
        public static string BarrelRayCastTest()
        {
            try
            {
                    Physics.Linecast(
                  Camera.main.transform.position,
                  Camera.main.transform.forward * 1000,
                  out RaycastHit);
                 return RaycastHit.transform.gameObject.layer.ToString(); // check the layer the raycast hits
            }
            catch
            {
                return "Unkown";
            }
        }
    }
}
