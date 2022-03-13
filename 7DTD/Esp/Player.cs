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
            try
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
                    if(player.IsSleeping)
                        continue;
                    if (player == GameManager.Instance.World.GetPrimaryPlayer())
                        continue;

                    PlayerList.Add(player);
                }
                CacheTime = Time.time + 5;
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
                    string DistanceStr = Globals.Config.Player.Distance ? $"({Distance.ToString()}m)" : "";
                    string PlayernameStr = Globals.Config.Player.Name ? $"{player.EntityName}" : "";
                    string HealthStr = Globals.Config.Player.Health ? $"({Health}hp)" : "";
                    Vector3 HeadPosition = Globals.WorldPointToScreenPoint(Globals.GetLimbPosition(player.transform, "Head"));

                    if (Distance > Globals.Config.Player.MaxDistance)
                        continue;

                    Drawing.DrawString(new Vector2(ScreenPosition.x, ScreenPosition.y), $"{PlayernameStr}{DistanceStr}{HealthStr}", Helpers.ColourHelper.GetColour("Player Colour"), true, 11, FontStyle.Normal, 3);
                    if ((player.IsAdmin || player.IsSpectator) && Globals.Config.Player.ShowAdmins)
                        Drawing.DrawString(new Vector2(ScreenPosition.x, ScreenPosition.y + 10), $"Admin", Helpers.ColourHelper.GetColour("Player Colour"), true, 11, FontStyle.Normal, 3);

                    if (Globals.Config.Player.Chams)
                        Helpers.ShaderHelper.ApplyShader(Helpers.ShaderHelper.Shaders["Chams"], player.gameObject, Helpers.ColourHelper.GetColour("Player Chams Visible Colour"), Helpers.ColourHelper.GetColour("Player Chams Invisible Colour"));
                    else
                        Helpers.ShaderHelper.RemoveShaders(player.gameObject);

                    if (Globals.GetLimbPosition(player.transform, "Head") == null)
                        continue;
                    float height = Mathf.Abs(ScreenPosition.y - HeadPosition.y);
                    Drawing.PlayerCornerBox(new Vector2(HeadPosition.x, HeadPosition.y), height / 2, height, 2, Distance, Helpers.ColourHelper.GetColour("Player Box Colour"));
                }
            }
            catch { }
        }

    }
}
