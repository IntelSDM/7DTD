using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat.Esp
{
    class Player : MonoBehaviour
    {
        private float CacheTime;
        public static List<EntityPlayer> PlayerList = new List<EntityPlayer>();
        [ObfuscationAttribute(Exclude = true)]
        void Update()
        {
            Update1();
        }
    
        void Update1()
        {
           
            if (GameManager.Instance.World == null)
                return;
            if (!(Time.time > CacheTime))
                return;
            Globals.LocalPlayer = GameManager.Instance.World.GetPrimaryPlayer();
            PlayerList.Clear();
            foreach (EntityPlayer player in GameManager.Instance.World.Players.list)
            {
                if (player.Health <= 0)
                    continue;
                if (player.IsAlive() == false)
                    continue;
                if (player == null)
                    continue;
                if (player == GameManager.Instance.World.GetPrimaryPlayer())
                    continue;

                PlayerList.Add(player);
            }
            CacheTime = Time.time + 5;
        }
        [ObfuscationAttribute(Exclude = true)]
        void OnGUI()
        {
            OnGUI1();

        }
        void OnGUI1()
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
                string DistanceStr = Globals.Config.Player.Distance ? $"({Distance.ToString()}m)"  : "";
                string PlayernameStr = Globals.Config.Player.Name ? $"{player.EntityName}" : "";
                string HealthStr = Globals.Config.Player.Health ? $"({Health}hp)" : "";
                Drawing.DrawString(new Vector2(ScreenPosition.x, ScreenPosition.y), $"{PlayernameStr}{DistanceStr}{HealthStr}", Helpers.ColourHelper.GetColour("PlayerColour"), true, 11, FontStyle.Normal, 3);
                if ((player.IsAdmin || player.IsSpectator) && Globals.Config.Player.ShowAdmins)
                    Drawing.DrawString(new Vector2(ScreenPosition.x, ScreenPosition.y + 10), $"Admin", Helpers.ColourHelper.GetColour("PlayerColour"), true, 11, FontStyle.Normal, 3);
            }
        }

    }
}
