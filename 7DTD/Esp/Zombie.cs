using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat.Esp
{
    // look into ConnectionManager, looks like we can join locked servers and shit with it
    class Zombie : MonoBehaviour
    {
        private float CacheTime;
        public static List<EntityZombie> ZombieList = new List<EntityZombie>();
        [ObfuscationAttribute(Exclude = true)]
        void Update()
        {
            Update1();
        }
        void Update1()
        {
            // caching zombies
            try
            {
                if (GameManager.Instance.World == null)
                    return; // check if world is active
                if (!(Time.time > CacheTime))
                    return; // check if cache time has passed
                ZombieList.Clear();
                foreach (EntityZombie zombie in FindObjectsOfType<EntityZombie>().ToList())
                {
                    if (zombie == null)
                        continue; // check is zombie is null or not
                    if (zombie.Health <= 0)
                        continue; // is the zombie alive
                    if (zombie.IsAlive() == false)
                        continue; // another alive check


                    ZombieList.Add(zombie); // add the zombie to the zombie list
                }
                CacheTime = Time.time + 3; // set the next time we will cache
            }
            catch 
            { 
            }
        }
        [ObfuscationAttribute(Exclude = true)]
        void OnGUI()
        {
            OnGUI1();

        }
        void OnGUI1()
        {
            try
            {
                if (GameManager.Instance.World == null)
                    return; // check if world is active
                foreach (EntityZombie zombie in ZombieList)
                {
                    if (zombie == null)
                        continue; // check if zombie is valid pointer
                    if (zombie.Health <= 0)
                        continue; // check alive
                    if (zombie.IsAlive() == false)
                        continue; // check alive


                    Vector3 screenposition = Globals.WorldPointToScreenPoint(zombie.transform.position);
                    if (!(Globals.IsScreenPointVisible(screenposition)))
                        continue;
                    int distance = (int)Vector3.Distance(Globals.MainCamera.transform.position, zombie.transform.position);
                    int health = zombie.Health;
                    string distancestr = Globals.Config.Zombie.Distance ? $"({distance.ToString()}m)" : "";
                    string namestr = Globals.Config.Zombie.Name ? $"{zombie.EntityName}" : "";
                    string healthstr = Globals.Config.Zombie.Health ? $"({health}hp)" : "";
                    if (Distance > Globals.Config.Zombie.MaxDistance)
                        continue;
                    Drawing.DrawString(new Vector2(screenposition.x, screenposition.y), $"{namestr}{distancestr}{healthstr}", Helpers.ColourHelper.GetColour("Zombie Colour"), true, 12, FontStyle.Normal, 0);
                }
            }
            catch { }
        }

    }
}
