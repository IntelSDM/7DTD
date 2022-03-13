using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat.Esp
{
    class Player : MonoBehaviour
    {
        private float CacheTime;
        public static List<EntityPlayer> PlayerList = new List<EntityPlayer>();
        void Update()
        {
            if (GameManager.Instance.World == null)
                return;
            if (!(Time.time > CacheTime))
                return;
            PlayerList.Clear();
            foreach (EntityPlayer player in GameManager.Instance.World.Players.list)
            {
                if (player.Health <= 0)
                    continue;
                if (player.IsAlive() == false)
                    continue;
                if (player == null)
                    continue;
                if(player == GameManager.Instance.World.GetPrimaryPlayer())
                    continue

                PlayerList.Add(player);
            }
            CacheTime = Time.time + 5;
        }
        void OnGUI()
        {
            if (GameManager.Instance.World == null)
                return;
            foreach (EntityPlayer player in PlayerList)
            {
                if (player.Health <= 0)
                    continue;
                if (player.IsAlive() == false)
                    continue;
                if (player == null)
                    continue;

                Vector3 ScreenPosition = Globals.WorldPointToScreenPoint(player.transform.position);
                if (!(Globals.IsScreenPointVisible(ScreenPosition)))
                    continue;
                int Distance = (int)Vector3.Distance(Globals.MainCamera.transform.position, player.transform.position);
                int Health = player.Health;
                Drawing.DrawString(new Vector2(ScreenPosition.x, ScreenPosition.y), $"{player.EntityName}({Distance}m)({Health}hp)", Helpers.ColourHelper.GetColour("PlayerColour"), true, 11, FontStyle.Normal, 3);
                if(player.IsAdmin || player.IsSpectator)
                    Drawing.DrawString(new Vector2(ScreenPosition.x, ScreenPosition.y +10), $"Admin", Helpers.ColourHelper.GetColour("PlayerColour"), true, 11, FontStyle.Normal, 3);
            }

        }

    }
}
