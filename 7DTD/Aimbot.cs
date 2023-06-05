using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat
{
    class Aimbot : MonoBehaviour
    {
        // EnumPlayerKillingMode look into so you can try to disable pve
        // TODO: Add friendslist
        // Fix Box Height
        public static EntityZombie Zombie; // assign our values here so we dont need to keep calling the targetzombie/player function which loops through loads of stuff
        public static EntityPlayer Player;
        public static Vector3 ZombieHitPos;
        public static Vector3 PlayerHitPos;
        public static bool TargettingPlayer = false;
        public static bool TargettingZombie = false;

        // We sort closest to crosshair so we will get the first target which is closest
        private static List<EntityZombie> ZombieClosestToCrosshair(List<EntityZombie> p)
        {
            return (from tempPlayer in p
                    orderby Vector2.Distance(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2)), Camera.main.WorldToScreenPoint(tempPlayer.transform.position))
                    select tempPlayer).ToList<EntityZombie>();
        }
        private static List<EntityPlayer> PlayerClosestToCrosshair(List<EntityPlayer> p)
        {
            return (from tempPlayer in p
                    orderby Vector2.Distance(new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2)), Camera.main.WorldToScreenPoint(tempPlayer.transform.position))
                    select tempPlayer).ToList<EntityPlayer>();
        }
        EntityZombie TargetZombie()
        {
            EntityZombie result = new EntityZombie();
            try
            {
            
            if (!Globals.Config.Aimbot.ZombieAimbot)
                return result; // returns null
            if (Globals.LocalPlayer == null)
                return result; // returns null, checks if the player is null, something might break if its null
           
           

                foreach (EntityZombie zombie in ZombieClosestToCrosshair(Esp.Zombie.ZombieList))
                {
                  
                        if (!(zombie.IsAlive()) || zombie == null)
                        continue;

                    Vector3 Pos = zombie.emodel.GetHeadTransform().position;
                    if (!Globals.IsScreenPointVisible(Globals.WorldPointToScreenPoint(Pos)))
                        continue; // check if the zombie is on screen
                    int fov = (int)Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(Globals.WorldPointToScreenPoint(Pos).x, Globals.WorldPointToScreenPoint(Pos).y));
                    if (fov > Globals.Config.Aimbot.Fov)
                        continue; // are they in fov?

                    if (Globals.Config.Aimbot.ZombieVisibilityChecks && !Helpers.RaycastHelper.ZombiePos(zombie, Pos))
                        continue; // if visibility checks are enabled then check if they are visible.



                    return zombie;
                }

            }
            catch { }
            return result; // must have a return, so return null

        }
        EntityPlayer TargetPlayer()
        {

            EntityPlayer result = new EntityPlayer();
            try
            {
                if (!Globals.Config.Aimbot.PlayerAimbot)
                return result;
            if (Globals.LocalPlayer == null)
                return result;
          
            

                foreach (EntityPlayer player in PlayerClosestToCrosshair(Esp.Player.PlayerList))
                {
                    if (Globals.LocalPlayer.IsFriendsWith(player) && Globals.Config.Aimbot.PlayerAimbotTargetFriends) // make check so they can turn off this check
                        continue;

                    if (player == null)
                        continue; // check if the player is real
                    if (player.Health <= 0)
                        continue; // are they alive
                    if (player.IsAlive() == false)
                        continue; // another alive check
                    if (player.IsSleeping)
                        continue; // checking if the player is alive


                    Vector3 Pos = player.emodel.GetHeadTransform().position;

                    if (!Globals.IsScreenPointVisible(Globals.WorldPointToScreenPoint(Pos)))
                        continue; // is the player on screen

                    int fov = (int)Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(Globals.WorldPointToScreenPoint(Pos).x, Globals.WorldPointToScreenPoint(Pos).y));
                    if (fov > Globals.Config.Aimbot.Fov)
                        continue; // is the user in the aimbot fov

                    if (Globals.Config.Aimbot.PlayerVisibilityChecks && !Helpers.RaycastHelper.PlayerPos(player, Pos))
                        continue; // if vis checks are on then vis check them

                    return player;
                }

            }
            catch { }
            return result;

        }
        [ObfuscationAttribute(Exclude = true)]
        void Update()
        {
            Update1();
        }
        [ObfuscationAttribute(Exclude = true)]
        void OnGUI()
        {
            OnGUI1();
        }
        void OnGUI1()
        {
            // so basically we cant target a player and zombie at the same time as we exit the function in the silentaim method when we have a target
            try
            {
                if (Globals.LocalPlayer == null)
                    return;

                if (Globals.Config.Aimbot.DrawFov)
                    Drawing.DrawCircle(Helpers.ColourHelper.GetColour("Aimbot Fov Colour"), new Vector2(Screen.width / 2, Screen.height / 2), Globals.Config.Aimbot.Fov); // draw fov circle

                if ((Zombie == null) && Player == null)
                    return;
                //   TargetZombie();
                Vector3 pos = Vector3.zero;
                if (TargettingPlayer)
                    pos = PlayerHitPos;
                if (TargettingZombie)
                    pos = ZombieHitPos;
                pos = Globals.WorldPointToScreenPoint(pos);
                // get the aimbot pos, w2s it


                if (Globals.Config.Aimbot.DrawTargetLine && Globals.IsScreenPointVisible(pos) && pos != Vector3.zero && pos != null) // check if the target position is on screen and not null, check if target line is enabled
                    Drawing.DrawLine(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(pos.x, pos.y), Helpers.ColourHelper.GetColour("Aimbot Target Line Colour"), 1); // draw a line to target
            }
            catch { }
        }
        void SilentAim()
        {
            if (Globals.LocalPlayer == null || Globals.MainCamera == null)
                return; // check if we should be aimbotting
            System.Random rand = new System.Random();
            if (rand.Next(0, 100) >= Globals.Config.Aimbot.Hitchance)
                return; // random value is over hitchance?

            if (Aimbot.Player != null)
            {
               
                TargettingPlayer = true;
                if (!Input.GetKey(Globals.Config.Aimbot.PlayerKey))
                    return; // player aimbot key being held?
                Globals.LocalPlayer.transform.eulerAngles = new Vector3(0f, Globals.LocalPlayer.transform.rotation.eulerAngles.y, 0f); // null x and y
                Camera.main.transform.LookAt(PlayerHitPos); // set lookat angles to hitpos
                return; // stop it changing target to zombie(proritising players)
            }
            TargettingPlayer = false;
            if (Aimbot.Zombie != null)
            {
               
                TargettingZombie = true;
                if (!Input.GetKey(Globals.Config.Aimbot.ZombieKey))
                    return; // Zombie key being held?
                Globals.LocalPlayer.transform.eulerAngles = new Vector3(0f, Globals.LocalPlayer.transform.rotation.eulerAngles.y, 0f); // null x and y
                Camera.main.transform.LookAt(ZombieHitPos); // set looking at angles to hitpos
                return;
            }
            TargettingZombie = false;
        }
        public bool IsFriend(EntityPlayer player)
        {
            if (player.Party?.PartyID == 0)
                return false; // null or not in party?
            if (Globals.LocalPlayer?.Party?.PartyID == 0)
                return false; // is it null? not in a party?
            if (Globals.LocalPlayer.Party.PartyID == player.Party.PartyID)
                return true; // check if the partyid is the same as the local player
         
            return false;

        }
        void SetZombieAimPos()
        {
         
            if (Aimbot.Zombie == null)
                return;
          
                ZombieHitPos = Zombie.emodel.GetHeadTransform().position;
        }
        void SetPlayerAimPos()
        {
            
           
            if (Aimbot.Player == null)
                return;
                PlayerHitPos = Player.emodel.GetHeadTransform().position;
        }
        void Update1()
        {
            Zombie = TargetZombie();
           SetZombieAimPos();
            Player = TargetPlayer();
            SetPlayerAimPos();
            SilentAim();
          
        }
    }
}
