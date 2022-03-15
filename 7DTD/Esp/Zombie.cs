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
            try
            {
                if (GameManager.Instance.World == null)
                    return;
                if (!(Time.time > CacheTime))
                    return;
                ZombieList.Clear();
                foreach (EntityZombie zombie in FindObjectsOfType<EntityZombie>().ToList())
                {
                    if (zombie.Health <= 0)
                        continue;
                    if (zombie.IsAlive() == false)
                        continue;
                    if (zombie == null)
                        continue;

                    ZombieList.Add(zombie);
                }
                CacheTime = Time.time + 3;
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
                    return;
                foreach (EntityZombie zombie in ZombieList)
                {
                    if (zombie.Health <= 0)
                        continue;
                    if (zombie.IsAlive() == false)
                        continue;
                    if (zombie == null)
                        continue;

                    Vector3 ScreenPosition = Globals.WorldPointToScreenPoint(zombie.transform.position);
                    if (!(Globals.IsScreenPointVisible(ScreenPosition)))
                        continue;
                    int Distance = (int)Vector3.Distance(Globals.MainCamera.transform.position, zombie.transform.position);
                    int Health = zombie.Health;
                    string DistanceStr = Globals.Config.Zombie.Distance ? $"({Distance.ToString()}m)" : "";
                    string nameStr = Globals.Config.Zombie.Name ? $"{zombie.EntityName}" : "";
                    string HealthStr = Globals.Config.Zombie.Health ? $"({Health}hp)" : "";
                    if (Distance > Globals.Config.Zombie.MaxDistance)
                        continue;
                    Drawing.DrawString(new Vector2(ScreenPosition.x, ScreenPosition.y), $"{nameStr}{DistanceStr}{HealthStr}", Helpers.ColourHelper.GetColour("Zombie Colour"), true, 12, FontStyle.Normal, 0);
                }
            }
            catch { }
        }

    }
}
