using System;
using System.Collections.Generic;
using System.Linq;
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
        void Update()
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
            CacheTime = Time.time + 5;
        }
        void OnGUI()
        {
            Globals.MainCamera = Camera.main;
            Drawing.DrawString(new Vector2(10, 10), "sdgsdgdgs", Color.red, false, 16, FontStyle.Normal, 0);
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
                Drawing.DrawString(new Vector2(ScreenPosition.x, ScreenPosition.y), $"{zombie.EntityName}({Distance}m)", Color.red, true, 12, FontStyle.Normal, 0);
            }

        }

    }
}
