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
        public static List<EntityEnemy> ZombieList = new List<EntityEnemy>();
        void Update()
        {
            // caching zombies
            try
            {
                if (GameManager.Instance.World == null)
                    return; // check if world is active
                if (!(Time.time > CacheTime))
                    return; // check if cache time has passed
                ZombieList.Clear();
                foreach (EntityEnemy zombie in FindObjectsOfType<EntityEnemy>().ToList())
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

        void OnGUI()
        {
            try
            {
                if (GameManager.Instance.World == null)
                    return; // check if world is active
                foreach (EntityEnemy zombie in ZombieList)
                {
                    if (zombie == null)
                        continue; // check if zombie is valid pointer
                    if (zombie.Health <= 0)
                    {
                        Helpers.ShaderHelper.RemoveShader(zombie.gameObject); // remove chams
                        continue; // check alive
                    }
                    if (zombie.IsAlive() == false)
                    {
                        Helpers.ShaderHelper.RemoveShader(zombie.gameObject); // remove chams
                        continue; // check alive
                    }


                    Vector3 screenposition = Globals.WorldPointToScreenPoint(zombie.transform.position);
                    if (!(Globals.IsScreenPointVisible(screenposition)))
                        continue;
                    int distance = (int)Vector3.Distance(Globals.MainCamera.transform.position, zombie.transform.position);
                    int health = zombie.Health;
                    string distancestr = Globals.Config.Zombie.Distance ? $"({distance.ToString()}m)" : "";
                    string namestr = Globals.Config.Zombie.Name ? $"{zombie.EntityName}" : "";
                    string healthstr = Globals.Config.Zombie.Health ? $"({health}hp)" : "";
                    Vector3 headposition = Globals.WorldPointToScreenPoint(zombie.emodel.GetHeadTransform().position); // get head positon
                    if (distance > Globals.Config.Zombie.MaxDistance)
                        continue;
                    Drawing.DrawString(new Vector2(screenposition.x, screenposition.y), $"{namestr}{distancestr}{healthstr}", Helpers.ColourHelper.GetColour("Zombie Colour"), true, 12, FontStyle.Normal, 0);
                 
                    float height = Mathf.Abs(headposition.y - screenposition.y); // get the height difference
                    float x = screenposition.x - height * 0.3f;
                    float y = headposition.y;
                    if (Globals.Config.Zombie.Box && distance < 200)
                    {
                        Drawing.PlayerCornerBox(new Vector2(headposition.x, headposition.y + 0.5f), height / 2, height, 2, distance, Helpers.ColourHelper.GetColour("Zombie Box Colour")); // draw a corner box
                    }
                    if (Globals.Config.Zombie.HealthBar && distance < 200)
                    {
                        Drawing.DrawHealthBar(zombie, height, x, y);
                    }
                    if (Globals.Config.Zombie.Chams)
                    {
                        Helpers.ShaderHelper.ApplyShader(Globals.Config.Zombie.ChamType, zombie.gameObject, Helpers.ColourHelper.GetColour("Zombie Chams Primary Colour"), Helpers.ColourHelper.GetColour("Zombie Chams Secondary Colour")); // apply chams
                    }
                    else
                    {
                        Helpers.ShaderHelper.RemoveShader( zombie.gameObject); // remove chams
                    }
                }
            }
            catch { }
        }

    }
}
