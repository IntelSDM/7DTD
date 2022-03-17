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
                return result;
            if (Globals.LocalPlayer == null)
                return result;
           
           

                foreach (EntityZombie zombie in ZombieClosestToCrosshair(Esp.Zombie.ZombieList))
                {
                  
                        if (!(zombie.IsAlive()) || zombie == null)
                        continue;
                    Vector3 Pos = Vector3.zero;

                    Pos = zombie.emodel.GetHeadTransform().position;
                    if (!Globals.IsScreenPointVisible(Globals.WorldPointToScreenPoint(Pos)))
                        continue;
                    int fov = (int)Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(Globals.WorldPointToScreenPoint(Pos).x, Globals.WorldPointToScreenPoint(Pos).y));
                    if (fov > Globals.Config.Aimbot.Fov)
                        continue;

                    if (Globals.Config.Aimbot.ZombieVisibilityChecks && !Helpers.RaycastHelper.ZombiePos(zombie, Pos))
                        continue;
                    

                   
                    result = zombie;
                    break; // break so it wont loop through the list again and get a further from crosshair entity
                }

            }
            catch { }
            return result;

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

                    if (player.Health <= 0)
                        continue;
                    if (player.IsAlive() == false)
                        continue;
                    if (player == null)
                        continue;
                    if (player.IsSleeping)
                        continue;
                    Vector3 Pos = Vector3.zero;

                    Pos = player.emodel.GetHeadTransform().position;

                    if (!Globals.IsScreenPointVisible(Globals.WorldPointToScreenPoint(Pos)))
                        continue;

                    int fov = (int)Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(Globals.WorldPointToScreenPoint(Pos).x, Globals.WorldPointToScreenPoint(Pos).y));
                    if (fov > Globals.Config.Aimbot.Fov)
                        continue;

                    if (Globals.Config.Aimbot.PlayerVisibilityChecks && !Helpers.RaycastHelper.PlayerPos(player, Pos))
                        continue;

                    result = player;
                    break; // break so it wont loop through the list again and get a further from crosshair entity
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

                if (Zombie == null)
                {
                    Drawing.DrawCircle(Color.blue, new Vector2(Screen.width / 2, Screen.height / 2), 30);
                }
                if (Globals.Config.Aimbot.DrawFov)
                    Drawing.DrawCircle(Helpers.ColourHelper.GetColour("Aimbot Fov Colour"), new Vector2(Screen.width / 2, Screen.height / 2), Globals.Config.Aimbot.Fov);

                if ((Zombie == null) && Player == null)
                    return;
                //   TargetZombie();
                Vector3 pos = Vector3.zero;
                if (TargettingPlayer)
                    pos = PlayerHitPos;
                if (TargettingZombie)
                    pos = ZombieHitPos;
                pos = Globals.WorldPointToScreenPoint(pos);



                if (Globals.Config.Aimbot.DrawTargetLine && Globals.IsScreenPointVisible(pos) && pos != Vector3.zero && pos != null)
                    Drawing.DrawLine(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(pos.x, pos.y), Helpers.ColourHelper.GetColour("Aimbot Target Line Colour"), 1);
            }
            catch { }
        }
        void SilentAim()
        {
            if (Globals.LocalPlayer == null || Globals.MainCamera == null)
                return;
            System.Random rand = new System.Random();
            if (rand.Next(0, 100) >= Globals.Config.Aimbot.Hitchance)
                return;

            if (Aimbot.Player != null)
            {
               
                TargettingPlayer = true;
                if (!Input.GetKey(Globals.Config.Aimbot.PlayerKey))
                    return;
                Globals.LocalPlayer.transform.eulerAngles = new Vector3(0f, Globals.LocalPlayer.transform.rotation.eulerAngles.y, 0f);
                Camera.main.transform.LookAt(PlayerHitPos);
                return; // stop it changing target to zombie(proritising players)
            }
            TargettingPlayer = false;
            if (Aimbot.Zombie != null)
            {
               
                TargettingZombie = true;
                if (!Input.GetKey(Globals.Config.Aimbot.ZombieKey))
                    return;
                Globals.LocalPlayer.transform.eulerAngles = new Vector3(0f, Globals.LocalPlayer.transform.rotation.eulerAngles.y, 0f);
                Camera.main.transform.LookAt(ZombieHitPos);
                return;
            }
            TargettingZombie = false;
        }
        public bool IsFriend(EntityPlayer player)
        {
            if (player.Party?.PartyID == 0)
                return false;
            if (Globals.LocalPlayer?.Party?.PartyID == 0)
                return false;
            if (Globals.LocalPlayer.Party.PartyID == player.Party.PartyID)
                return true;
         
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
