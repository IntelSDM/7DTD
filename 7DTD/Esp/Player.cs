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
                    return; // is world active?
                if (!(Time.time > CacheTime))
                    return; // check if enough time passed
                Globals.LocalPlayer = GameManager.Instance.World.GetPrimaryPlayer(); // set local player
                PlayerList.Clear(); // clear the current player list
                foreach (EntityPlayer player in GameManager.Instance.World.Players.list)
                {
                    if (player == null)
                        continue; // is player null
                    if (player.Health <= 0)
                        continue; // is player alive
                    if (player.IsAlive() == false)
                        continue;// is player alive
                    if(player.IsSleeping)
                        continue; // is the player sleeping
                    if (player == GameManager.Instance.World.GetPrimaryPlayer())
                        continue;// check if the player is local player

                    PlayerList.Add(player); // add the player to the list
                }
                CacheTime = Time.time + 5; // create the next cache time
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
            if (Globals.LocalPlayer != null)
            {
                EntityAlive ent = Globals.LocalPlayer as EntityAlive;
                Vector3 lookdirection = new Vector3(0f, ent.GetEyeHeight(), 0f);
                Vector3 lookvector = ent.GetLookVector();
            }
            try
            {
                if (GameManager.Instance.World == null)
                    return; // check game world is active
                foreach (EntityPlayer player in PlayerList)
                {
                    if (player == null)
                        continue; // check if player is active
                    if (player.Health <= 0)
                    {
                        Helpers.ShaderHelper.RemoveShader(player.gameObject);
                        continue; // check if the player is alive
                    }
                    if (player.IsAlive() == false)
                    {
                        Helpers.ShaderHelper.RemoveShader(player.gameObject);
                        continue; // check if the player is alive
                    }

                    Vector3 screenposition = Globals.WorldPointToScreenPoint(player.transform.position);
                    if (!(Globals.IsScreenPointVisible(screenposition)))
                        continue; // check player is visible
                    int distance = (int)Vector3.Distance(Globals.MainCamera.transform.position, player.transform.position); // distance between player and camera
                    int health = player.Health; // get player health
                    string distancestr = Globals.Config.Player.Distance ? $"({distance.ToString()}m)" : ""; // concat the distance with inlined if statement
                    string playernamestr = Globals.Config.Player.Name ? $"{player.EntityName}" : "";// concat the name with inlined if statement
                    string healthstr = Globals.Config.Player.Health ? $"({health}hp)" : "";// concat the hp with inlined if statement
                    Vector3 headposition = Globals.WorldPointToScreenPoint(player.emodel.GetHeadTransform().position); // get head positon

                    if (distance > Globals.Config.Player.MaxDistance)
                        continue; // skip the entity if they are over the max distance

                    Drawing.DrawString(new Vector2(screenposition.x, screenposition.y), $"{playernamestr}{distancestr}{healthstr}", Helpers.ColourHelper.GetColour("Player Colour"), true, 11, FontStyle.Normal, 3); // draw name, health and distance
                    if ((player.IsAdmin || player.IsSpectator) && Globals.Config.Player.ShowAdmins)
                        Drawing.DrawString(new Vector2(screenposition.x, screenposition.y + 10), $"Admin", Helpers.ColourHelper.GetColour("Player Colour"), true, 11, FontStyle.Normal, 3); // draw admin status

                    if (Globals.Config.Player.Chams)
                        Helpers.ShaderHelper.ApplyShader(0, player.gameObject, Helpers.ColourHelper.GetColour("Player Chams Primary Colour"), Helpers.ColourHelper.GetColour("Player Chams Secondary Colour")); // apply chams
                    else
                        Helpers.ShaderHelper.RemoveShader(player.gameObject);
                    float height = Mathf.Abs(headposition.y - screenposition.y); // get the height difference
                    float x = screenposition.x - height * 0.3f;
                    float y = headposition.y; 
                    if (Globals.Config.Player.Box && distance < 200)
                    {
                        Drawing.PlayerCornerBox(new Vector2(headposition.x, headposition.y + 0.5f), height / 2, height, 2, distance, Helpers.ColourHelper.GetColour("Player Box Colour")); // draw a corner box
                    }
                    if (Globals.Config.Player.HealthBar && distance < 200)
                    {
                        Drawing.DrawHealthBar(player, height, x, y);
                    }
                }
            }
            catch { }
        }

    }
}
