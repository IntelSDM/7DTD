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
                        continue; // check if player is alive 
                    if (player.IsAlive() == false)
                        continue; // check if the player is alive
                    

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
                        Helpers.ShaderHelper.ApplyShader(Helpers.ShaderHelper.Shaders["Chams"], player.gameObject, Helpers.ColourHelper.GetColour("Player Chams Visible Colour"), Helpers.ColourHelper.GetColour("Player Chams Invisible Colour")); // apply chams

                    float height = Mathf.Abs(screenposition.y - screenposition.y); // get the height difference
                    if (Globals.Config.Player.Box && distance < 200)
                    {
                       
                        Drawing.PlayerCornerBox(new Vector2(headposition.x, headposition.y + 0.5f), height / 2, height, 2, distance, Helpers.ColourHelper.GetColour("Player Box Colour")); // draw a corner box
                    }
                    if (Globals.Config.Player.HealthBar && distance < 200)
                    {
                        float maxhp = player.Stats.Health.Max;
                        float hp = player.Stats.Health.Value;
                        float percent = hp / maxhp;
                        float percent2 = (hp / maxhp) * 100;
                        float use = percent * height - 2f;
                        Color32 barcol = new Color32();
                        if (percent2 <= 100 && percent2 >= 86)
                        {
                            barcol = new Color32(15, 212, 10, 255);
                        }
                        if (percent2 <= 85 && percent2 >= 66)
                        {
                            barcol = new Color32(253, 219, 9, 200);

                        }
                        if (percent2 <= 65 && percent2 >= 35)
                        {
                            barcol = new Color32(249, 108, 24, 200);
                        }
                        if (percent2 <= 34 && percent2 >= 0)
                        {
                            barcol = new Color32(249, 3, 3, 255);
                        }
                        Drawing.DrawFilledBox(headposition.x - height / 4 - 5, headposition.y, 4, height, new Color32(0, 0, 0, 180)); // draw health bar background
                        Drawing.DrawFilledBox(headposition.x - height / 4 - 4, headposition.y + height - use - 1, 2f, use, barcol); // draw healthbar
                    }
                }
            }
            catch { }
        }

    }
}
