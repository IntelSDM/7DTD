using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat.Esp
{
    class Animal : MonoBehaviour
    {
        private float CacheTime;
        public static List<EntityAnimal> AnimalList = new List<EntityAnimal>();
        void Update()
        {
            if (GameManager.Instance.World == null)
                return;
            if (!(Time.time > CacheTime))
                return;
            AnimalList.Clear();
            foreach (EntityAnimal animal in FindObjectsOfType<EntityAnimal>().ToList())
            {
                if (animal.Health <= 0)
                    continue;
                if (animal.IsAlive() == false)
                    continue;
                if (animal == null)
                    continue;

                AnimalList.Add(animal);
            }
            CacheTime = Time.time + 5;
        }
        void OnGUI()
        {
            if (GameManager.Instance.World == null)
                return;
            foreach (EntityAnimal animal in AnimalList)
            {
                if (animal.Health <= 0)
                    continue;
                if (animal.IsAlive() == false)
                    continue;
                if (animal == null)
                    continue;

                Vector3 ScreenPosition = Globals.WorldPointToScreenPoint(animal.transform.position);
                if (!(Globals.IsScreenPointVisible(ScreenPosition)))
                    continue;
                int Distance = (int)Vector3.Distance(Globals.MainCamera.transform.position, animal.transform.position);
                Drawing.DrawString(new Vector2(ScreenPosition.x, ScreenPosition.y), $"{animal.EntityName}({Distance}m)", Helpers.ColourHelper.GetColour("AnimalColour"), true, 12, FontStyle.Normal, 0);
            }

        }

    }
}
