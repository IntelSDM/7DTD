using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat.Esp
{
    class Animal : MonoBehaviour
    {
        private float CacheTime;
        public static List<EntityAnimal> AnimalList = new List<EntityAnimal>();

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
                    return; // check if game manager is null
                if (!(Time.time > CacheTime))
                    return; // check if cache time has passed
                AnimalList.Clear();
                foreach (EntityAnimal animal in FindObjectsOfType<EntityAnimal>().ToList())
                {
                    if (animal == null)
                        continue; // check animal is active
                    if (animal.Health <= 0)
                        continue; // is animal alive
                    if (animal.IsAlive() == false)
                        continue; // is animal alive


                    AnimalList.Add(animal); // add animal to esp list
                }
                CacheTime = Time.time + 5; // make new cache time and add delay
            }
            catch { }
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
                    return;// check if gamemanager is active
                foreach (EntityAnimal animal in AnimalList)
                {
                    if (animal == null)
                        continue; // check if animal is active
                    if (animal.Health <= 0)
                        continue; // check if animal is alive
                    if (animal.IsAlive() == false)
                        continue; // check if animal is alive


                    Vector3 screenposition = Globals.WorldPointToScreenPoint(animal.transform.position);
                    if (!(Globals.IsScreenPointVisible(screenposition)))
                        continue; // check if animal is on screen
                    int distance = (int)Vector3.Distance(Globals.MainCamera.transform.position, animal.transform.position); // get distance
                    int health = animal.Health; // get health
                    string distancestr = Globals.Config.Animal.Distance ? $"({distance.ToString()}m)" : ""; // concat the distance with inlined if statement
                    string namestr = Globals.Config.Animal.Name ? $"{animal.EntityName}" : "";// concat the name with inlined if statement
                    string healthstr = Globals.Config.Animal.Health ? $"({health}hp)" : "";// concat the hp with inlined if statement
                    if (distance > Globals.Config.Animal.MaxDistance)
                        continue;// check if they are under max distance

                    Drawing.DrawString(new Vector2(screenposition.x, screenposition.y), $"{namestr}{distancestr}{healthstr}", Helpers.ColourHelper.GetColour("Animal Colour"), true, 12, FontStyle.Normal, 0); // draw information
                }
            }
            catch { }
        }
    }
}
