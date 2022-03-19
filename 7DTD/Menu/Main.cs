using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat.Menu
{ 
    // credit to sCrub, some of the code with handling menu objects is his.
    class Main : MonoBehaviour
    {
        Vector2 MenuPos;
        uint MainMenuIndex = 0;
        private Entity Selected;
        private double NextPlayerTime;
        private KeyCode KeyToBind;
        int itemamount = 1;
        SubMenu MainMenu = new SubMenu("Main","Menu");

        SubMenu Esp = new SubMenu("ESP", "Draw Visuals");
        SubMenu Aimbot = new SubMenu("Aimbot", "Lock Onto Enemies");
        SubMenu LocalPlayer = new SubMenu("Local Player", "Modify Your Player");
        SubMenu PlayerMenu = new SubMenu("Player Menu", "Allows You To Abuse Other Players");
        SubMenu Colours = new SubMenu("Colour Menu", "Allows You To Change Colours On The Cheat");
        SubMenu Config = new SubMenu("Config Menu", "Allows You To Save And Load Settings");

        SubMenu Save = new SubMenu("Save Config", "Allows You To Save Settings");
        SubMenu Load = new SubMenu("Load Config", "Allows You To Load Settings");

        SubMenu PlayerAimbot = new SubMenu("Player Aimbot", "Configure Aimbot For Players");
        SubMenu ZombieAimbot = new SubMenu("Zombie Aimbot", "Configure Aimbot For Zombies");

        SubMenu Skill = new SubMenu("Skills", "Edit Your Player's Skills");
        SubMenu Movement = new SubMenu("Movement", "Edit Your Player's Movement");
        SubMenu PlayerProperties = new SubMenu("Properties", "Edit Stamina And Other Player Properties");
        SubMenu Weapon = new SubMenu("Weapon", "Modify Your Weapon");
        SubMenu World = new SubMenu("World", "Modify Player World Settings");
        List<SubMenu> MenuHistory = new List<SubMenu>();
        SubMenu CurrentMenu;
        #region Config
        void Configs()
        {
            // putting our options into 2 submenus and adding submenus to config menu
            Config.Items.Add(Save);
            Config.Items.Add(Load);
            #region Save Config
            Button Cfg1 = new Button("Config 1", "", () => Helpers.ConfigHelper.SaveConfig("Config1"));
            Button Cfg2 = new Button("Config 2", "", () => Helpers.ConfigHelper.SaveConfig("Config2"));
            Button Cfg3 = new Button("Config 3", "", () => Helpers.ConfigHelper.SaveConfig("Config3"));
            Button Cfg4 = new Button("Config 4", "", () => Helpers.ConfigHelper.SaveConfig("Config4"));
            Button Cfg5 = new Button("Config 5", "", () => Helpers.ConfigHelper.SaveConfig("Config5"));
            Save.Items.Add(Cfg1);
            Save.Items.Add(Cfg2);
            Save.Items.Add(Cfg3);
            Save.Items.Add(Cfg4);
            Save.Items.Add(Cfg5);
            #endregion
            #region Load Config
            Button Cfg6 = new Button("Config 1", "", () => Helpers.ConfigHelper.LoadConfig("Config1"));
            Button Cfg7 = new Button("Config 2", "", () => Helpers.ConfigHelper.LoadConfig("Config2"));
            Button Cfg8 = new Button("Config 3", "", () => Helpers.ConfigHelper.LoadConfig("Config3"));
            Button Cfg9 = new Button("Config 4", "", () => Helpers.ConfigHelper.LoadConfig("Config4"));
            Button Cfg10 = new Button("Config 5", "", () => Helpers.ConfigHelper.LoadConfig("Config5"));
            Load.Items.Add(Cfg6);
            Load.Items.Add(Cfg7);
            Load.Items.Add(Cfg8);
            Load.Items.Add(Cfg9);
            Load.Items.Add(Cfg10);
            #endregion
        }
        #endregion
        #region ESP
        void ESP()
        {
            
            SubMenu player = new SubMenu("Player Esp", "Shows Player Information");
            SubMenu zombies = new SubMenu("Zombie Esp", "Shows Zombie Information");
            SubMenu animal = new SubMenu("Animal Esp", "Shows Animal Information");

            Esp.Items.Add(player);
            Esp.Items.Add(zombies);
            Esp.Items.Add(animal);
            #region Player
            Toggle playername = new Toggle("Player Name", "Shows The Player Name", ref Globals.Config.Player.Name);
            Toggle playerdistance = new Toggle("Player Distance", "Shows Your Distance From The Player", ref Globals.Config.Player.Distance);
            Toggle playerhealth = new Toggle("Player Health", "Shows The Player Health", ref Globals.Config.Player.Health);
            IntSlider playermaxdistance = new IntSlider("Max Distance", "Max Distance Players Will Render", ref Globals.Config.Player.MaxDistance, 0, 10000, 50);
            Toggle playeradmin = new Toggle("Show Admin Information", "Shows If Player Is An Admin", ref Globals.Config.Player.ShowAdmins);
            Toggle playerbox = new Toggle("Show Boxes", "Draws Boxes Around Players", ref Globals.Config.Player.Box);
            Toggle playerhealthbar = new Toggle("Show Health Bar", "Draws Health Bar Next To Player", ref Globals.Config.Player.HealthBar);
            Toggle playerchams = new Toggle("Chams", "Changes Player Model Colour", ref Globals.Config.Player.Chams);

            player.Items.Add(playername);
            player.Items.Add(playerdistance);
            player.Items.Add(playerhealth);
            player.Items.Add(playermaxdistance);
            player.Items.Add(playeradmin);
            player.Items.Add(playerbox);
            player.Items.Add(playerhealthbar);
            player.Items.Add(playerchams);
            #endregion
            #region Zombie
            Toggle zombiename = new Toggle("Zombie Name", "Shows The Type Of Zombie", ref Globals.Config.Zombie.Name);
            Toggle zombiedistance = new Toggle("Zombie Distance", "Shows Your Distance From The Zombie", ref Globals.Config.Zombie.Distance);
            Toggle zombiehealth = new Toggle("Zombie Health", "Shows The Zombie Health", ref Globals.Config.Zombie.Health);
            IntSlider zombiemaxdistance = new IntSlider("Max Distance", "Max Distance Zombies Will Render", ref Globals.Config.Zombie.MaxDistance, 0, 2000, 50);

            zombies.Items.Add(zombiename);
            zombies.Items.Add(zombiedistance);
            zombies.Items.Add(zombiehealth);
            zombies.Items.Add(zombiemaxdistance);
            #endregion

            #region Animal
            Toggle animalname = new Toggle("Animal Name", "Shows The Type Of Animal", ref Globals.Config.Animal.Name);
            Toggle animaldistance = new Toggle("Animal Distance", "Shows Your Distance From The Animal", ref Globals.Config.Animal.Distance);
            Toggle animalhealth = new Toggle("Animal Health", "Shows The Animal Health", ref Globals.Config.Animal.Health);
            IntSlider animalmaxdistance = new IntSlider("Max Distance", "Max Distance Animals Will Render", ref Globals.Config.Animal.MaxDistance, 0, 2000, 50);

            animal.Items.Add(animalname);
            animal.Items.Add(animaldistance);
            animal.Items.Add(animalhealth);
            animal.Items.Add(animalmaxdistance);
            #endregion
        }
        #endregion
        #region Weapon
        void Weapons()
        {
           
            Button RemoveRecoil = new Button("Remove Recoil", "!This Cant Be Undone! This Button Removes Recoil", () => Misc.EnableNoRecoil());
            Toggle NoRecoil = new Toggle("No Recoil From Start Up", "This Will Remove Recoil On Start Or Config Load", ref Globals.Config.LocalPlayer.NoRecoil);
            Button RemoveSpread = new Button("Remove Spread", "!This Cant Be Undone! This Button Removes Spread", () => Misc.EnableNoSpread());
            Toggle NoSpread = new Toggle("No Spread From Start Up", "This Will Remove Spread On Start Or Config Load", ref Globals.Config.LocalPlayer.NoSpread);
            Toggle NoViewBob = new Toggle("No Shake", "Removes Shake/ViewBob", ref Globals.Config.LocalPlayer.NoViewBob);
            Toggle UnlimitedAmmo = new Toggle("Unlimited Ammo", "Gives You Unlimited Ammo", ref Globals.Config.LocalPlayer.UnlimitedAmmo);
            Toggle UnlimitedRange = new Toggle("Unlimited Range", "Allows You To Shoot Further", ref Globals.Config.LocalPlayer.UnlimitedRange);
            Toggle FovChanger = new Toggle("Weapon Fov Changer", "Changes Distance Of Weapon From Camera", ref Globals.Config.LocalPlayer.WeaponFovChanger);
            IntSlider FovSlider = new IntSlider("Weapon Fov", "Amount Of Fov The Fov Changer Will Change", ref Globals.Config.LocalPlayer.WeaponFov, 10, 190, 5);
            Weapon.Items.Add(RemoveRecoil);
            Weapon.Items.Add(NoRecoil);
            Weapon.Items.Add(RemoveSpread);
            Weapon.Items.Add(NoSpread);
            Weapon.Items.Add(NoViewBob);
            Weapon.Items.Add(UnlimitedAmmo);
            Weapon.Items.Add(FovChanger);
            Weapon.Items.Add(FovSlider);
            LocalPlayer.Items.Add(Weapon);
        }
        #endregion
        #region Aimbot
        void Aimbots()
        {
            IntSlider fov = new IntSlider("Aimbot Fov", "Circle Range From Centre Of Your Screen That Aimbot Will Target", ref Globals.Config.Aimbot.Fov, 0, 1800, 25);
            Toggle drawfov = new Toggle("Draw Fov", "Draws A Circle To Display Aimbot Fov", ref Globals.Config.Aimbot.DrawFov);
            Toggle drawtarget = new Toggle("Draw Aimbot Target Line", "Draws A Line To Aimbot Target", ref Globals.Config.Aimbot.DrawTargetLine);
            IntSlider hitchance = new IntSlider("Aimbot Hotchance", "% Chance Your Aimbot Hits Target", ref Globals.Config.Aimbot.Hitchance, 0, 100, 10);
            Aimbot.Items.Add(PlayerAimbot);
            Aimbot.Items.Add(ZombieAimbot);
            Aimbot.Items.Add(fov);
            Aimbot.Items.Add(drawfov);
            Aimbot.Items.Add(drawtarget);
            Aimbot.Items.Add(hitchance);

            Toggle targetplayer = new Toggle("Enable Player Aimbot", "Makes Aimbot Target Players", ref Globals.Config.Aimbot.PlayerAimbot);
            Toggle targetfriend = new Toggle("Target Friends", "Makes Aimbot Target Friends", ref Globals.Config.Aimbot.PlayerAimbotTargetFriends);
            Toggle visibleplayer = new Toggle("Visibility Check", "Aimbot Only Targets Visible Players", ref Globals.Config.Aimbot.PlayerVisibilityChecks);
            Keybind playerbind = new Keybind("PLayer Aimbot Keybind", "Key Aimbot Will Work On - To have it turned on when you shoot use your mouse0", ref Globals.Config.Aimbot.PlayerKey);
            PlayerAimbot.Items.Add(targetplayer);
            PlayerAimbot.Items.Add(targetfriend);
            PlayerAimbot.Items.Add(visibleplayer);
            PlayerAimbot.Items.Add(playerbind);

            Toggle targetzombie = new Toggle("Enable Zombie Aimbot", "Makes Aimbot Target Zombies", ref Globals.Config.Aimbot.ZombieAimbot);
            Toggle visiblezombie = new Toggle("Visibility Check", "Aimbot Only Targets Visible Zombies", ref Globals.Config.Aimbot.ZombieVisibilityChecks);
            Keybind zombiebind = new Keybind("Zombie Aimbot Keybind", "Key Aimbot Will Work On - To have it turned on when you shoot use your mouse0", ref Globals.Config.Aimbot.ZombieKey);
            ZombieAimbot.Items.Add(targetzombie);
            ZombieAimbot.Items.Add(visiblezombie);
            ZombieAimbot.Items.Add(zombiebind);

        }
        #endregion
        #region Skills
        void Skills()
        {
           
            IntSlider kill = new IntSlider("Amount Of Player Kills", "Amount Of Player Kills To Set", ref Globals.Config.LocalPlayer.Kills,0,30000,3);
            Button killbtn = new Button("Set Player Kills", "Sets The Amount Of Player Kills Your Player Has", () => Misc.SetKills(Globals.Config.LocalPlayer.Kills));
            IntSlider zombiekill = new IntSlider("Amount Of Zombie Kills", "Amount Of Zombie Kills To Set", ref Globals.Config.LocalPlayer.ZombieKills, 0, 30000, 3);
            Button zombiekillbtn = new Button("Set Zombie Kills", "Sets The Amount Of Zombie Kills Your Player Has", () => Misc.SetZombieKills(Globals.Config.LocalPlayer.ZombieKills));
            IntSlider death = new IntSlider("Amount Of Kills", "Amount Of Kills To Set", ref Globals.Config.LocalPlayer.Deaths, 0, 30000, 3);
            Button deathbtn = new Button("Set Deaths", "Sets The Amount Of Deaths Your Player Has", () => Misc.SetDeaths(Globals.Config.LocalPlayer.Deaths));
            IntSlider level = new IntSlider("Amount Of Levels", "Amount Of Zombie Levels To", ref Globals.Config.LocalPlayer.Level, 0, 30000, 3);
            Button levelbtn = new Button("Set Player Levels", "Sets Your Player Level", () => Misc.SetLevel(Globals.Config.LocalPlayer.Level));
            IntSlider distance = new IntSlider("Amount Of Distance KM", "Amount Of Distance KM To Set", ref Globals.Config.LocalPlayer.DistanceTravelled, 0, 30000, 3);
            Button distancebtn = new Button("Set Player Traveled Distance", "Sets Your Player's Total Distance Traveled", () => Misc.SetDistanceTraveled(Globals.Config.LocalPlayer.DistanceTravelled));
            IntSlider crafted = new IntSlider("Amount Of Items Crafted", "Amount Of Items Crafted To Set", ref Globals.Config.LocalPlayer.ItemsCrafted, 0, 70000, 7);
            Button craftedbtn = new Button("Set Items Crafted", "Sets Your Player's Items Crafted", () => Misc.SetTotalItemsCrafted(Globals.Config.LocalPlayer.ItemsCrafted));
            IntSlider time = new IntSlider("Amount Of Time Played", "Amount Of Time Played On Server In Hours", ref Globals.Config.LocalPlayer.TimePlayed, 0, 70000, 7);
            Button timebtn = new Button("Set Time Played", "Sets Player's Time Played On Server", () => Misc.SetTotalTimePlayed(Globals.Config.LocalPlayer.TimePlayed));
            IntSlider skill = new IntSlider("Amount Of Skill Points", "Amount Of Skill Points To Add", ref Globals.Config.LocalPlayer.SkillPoints, 0, 30000, 3);
            Button skillbtn = new Button("Set Player Skill Points", "Sets Your Player's Skillpoints", () => Misc.SetLevel(Globals.Config.LocalPlayer.SkillPoints));

            Skill.Items.Add(kill);
            Skill.Items.Add(killbtn);
            Skill.Items.Add(zombiekill);
            Skill.Items.Add(zombiekillbtn);
            Skill.Items.Add(death);
            Skill.Items.Add(deathbtn);
            Skill.Items.Add(level);
            Skill.Items.Add(levelbtn);
            Skill.Items.Add(distance);
            Skill.Items.Add(distancebtn);
            Skill.Items.Add(crafted);
            Skill.Items.Add(craftedbtn);
            Skill.Items.Add(time);
            Skill.Items.Add(timebtn);
            Skill.Items.Add(skill);
            Skill.Items.Add(skillbtn);
            LocalPlayer.Items.Add(Skill);

        }
        #endregion
        #region Movement
        void Movements()
        {
            Toggle speedhack = new Toggle("Speedhack", "Allows You To Zoom", ref Globals.Config.LocalPlayer.Speedhack);
            IntSlider speed = new IntSlider("Speed Value", "Change The Speed Your Speedhack Works At", ref Globals.Config.LocalPlayer.SpeedAmount, 1, 30, 1);
            Keybind speedbind = new Keybind("Speedhack Keybind", "Sets The Key You Hold To Use Speedhack", ref Globals.Config.LocalPlayer.SpeedKey);
            Toggle noclip = new Toggle("Custom Noclip", "Custom Noclip To Avoid Instant Bans, Noclip With This Then Turn On Fly For Best Usage", ref Globals.Config.LocalPlayer.BtecNoclip);
            IntSlider noclipspeed = new IntSlider("Noclip Speed Value", "Change The Speed You Noclip At", ref Globals.Config.LocalPlayer.NoclipSpeed, 1, 30, 1);
            Keybind noclipbind = new Keybind("Noclip Keybind", "Sets The Key You Hold To Use Custom Noclip", ref Globals.Config.LocalPlayer.NoclipKey);
            Movement.Items.Add(speedhack);
            Movement.Items.Add(speed);
            Movement.Items.Add(speedbind);
            Movement.Items.Add(noclip);
            Movement.Items.Add(noclipspeed);
            Movement.Items.Add(noclipbind);
            LocalPlayer.Items.Add(Movement); // teleport to waypoint, speedhack, also try find the function that shows other player's bases and hook it
        }
        #endregion
        #region Properties
        void Properties()
        {
            Toggle unlimitedstamina = new Toggle("Unlimited Stamina", "Run Forrest, Run", ref Globals.Config.LocalPlayer.UnlimitedStamina);
            Toggle unlimitedhunger = new Toggle("Unlimited Hunger", "You Are Too Chonky To Lose Hunger", ref Globals.Config.LocalPlayer.UnlimitedHunger);
            Toggle unlimitedwater = new Toggle("Unlimited Water", "Never Run Out Of Water Like A Camel", ref Globals.Config.LocalPlayer.UnlimitedThirtst);
            Toggle unlimitedhealth = new Toggle("Instant Health Regeneration", "You Health Instantly Regenerates To Max", ref Globals.Config.LocalPlayer.InstantHealth);
            Button cleardebuff = new Button("Clear Debuffs", "Removes Injuries Such As Broken Legs", () => Misc.ClearDebuff());
            Toggle spoofname = new Toggle("Spoof name", "Spoofs Your Name", ref Globals.Config.LocalPlayer.SpoofName);
            Button name = new Button("Copy Name From Clipboard", "Copies Name From Clipboard And Spoofs Name To It", () => Misc.ClipboardToString(out Misc.Name));
            PlayerProperties.Items.Add(unlimitedstamina);
            PlayerProperties.Items.Add(unlimitedhunger);
            PlayerProperties.Items.Add(unlimitedwater);
            PlayerProperties.Items.Add(unlimitedhealth);
            PlayerProperties.Items.Add(cleardebuff);
            PlayerProperties.Items.Add(spoofname);
            PlayerProperties.Items.Add(name);
            LocalPlayer.Items.Add(PlayerProperties);
        
        }
        #endregion
        #region World
        void Worlds()
        {
            LocalPlayer.Items.Add(World);
            Button cmd = new Button("Execute Console Comand From Clipboard", "Copy A Comand Then Click Enter", () => Misc.ExecuteCommandFromClipboard());
            IntSlider itmamount = new IntSlider("Amount Of Items", "Amount Of Items To Give Yourself", ref itemamount, 1, 100, 1);
            Button giveitem = new Button("Give Item From Name From Clipboard", "Copy An Item Code And Input Amount And You Will Be Given The Item", () => Misc.GiveItemFromClipboard(itemamount));
            Toggle block = new Toggle("Disable Land Claim Durability", "Disables Damage Protection From Land Claim Blocks", ref Globals.Config.LocalPlayer.LandClaim);
            Toggle instant1 = new Toggle("Instant Break Blocks 1", "Breaks Blocks And Bypasses Most Server Checks", ref Globals.Config.LocalPlayer.InstantBreak1);
            Toggle instant2 = new Toggle("Instant Break Blocks 2", "Breaks Blocks With A Different Bypass", ref Globals.Config.LocalPlayer.InstantBreak2);
            Toggle instant3 = new Toggle("Instant Break Blocks 3", "Breaks Blocks With A Different Bypass", ref Globals.Config.LocalPlayer.InstantBreak3);
            Button craft = new Button("Instant Free Craft", "Craft Items Instantly With No Resources", () => Misc.InstantCraft());
            Button nuke = new Button("Kill Everyone Excluding Yourself", "Kills Everyone But You", () => Misc.KillEveryoneElse());
            Button nuke2 = new Button("Kill Everyone Including Yourself", "Kills Everyone", () => Misc.KillEveryone());
            Button creative = new Button("Creative Menu", "Allows You To Spawn Items", () => Misc.CreativeMenu());
            Button debug = new Button("Debug Menu", "Allows You To Fly And Teleport On Map", () => Misc.DebugMenu());
            Toggle farinteract = new Toggle("Far Interact", "Allows You To Place And Pickup Blocks Further Away", ref Globals.Config.LocalPlayer.FarInteract);
            IntSlider interactdistance = new IntSlider("Far Interact Distance", "How Far You Can Interact", ref Globals.Config.LocalPlayer.FarInteractDistance, 25, 1000, 5);
            World.Items.Add(cmd);
            World.Items.Add(itmamount);
            World.Items.Add(giveitem);
            World.Items.Add(block);
            World.Items.Add(instant1);
            World.Items.Add(instant2);
            World.Items.Add(instant3);
            World.Items.Add(craft);
            World.Items.Add(nuke);
            World.Items.Add(nuke2);
            World.Items.Add(creative);
            World.Items.Add(debug);
            World.Items.Add(farinteract);
            World.Items.Add(interactdistance);
        }
        #endregion
        // if entity is keybind. keybind = keycode.none, if any keybind == keycode.null make it = setkey
        KeyCode SetKey()
        {
            KeyCode Key = new KeyCode();
            Event e = Event.current;
            if (e.keyCode != KeyCode.RightArrow)
            {
                Key = e.keyCode;
              

            }
            else
            {
                Key = KeyCode.None;

            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Key = KeyCode.Mouse0;
               
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Key = KeyCode.Mouse1;
                
            }
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                Key = KeyCode.Mouse2;
               
            }
            if (Input.GetKeyDown(KeyCode.Mouse3))
            {
                Key = KeyCode.Mouse3;
               
            }
            if (Input.GetKeyDown(KeyCode.Mouse4))
            {
                Key = KeyCode.Mouse4;
              
            }
            if (Input.GetKeyDown(KeyCode.Mouse5))
            {
                Key = KeyCode.Mouse5;
                
            }
            if (Input.GetKeyDown(KeyCode.Mouse6))
            {
                Key = KeyCode.Mouse6;
                
            }
            return Key;
        }
        [ObfuscationAttribute(Exclude = true)]
        void Start()
        {
            Start1();
        }
        void Start1()
        {
            MenuPos.x = 50;
            MenuPos.y = 100;
            MenuHistory.Add(MainMenu);
            CurrentMenu = MainMenu;
            MainMenu.Items.Add(Esp);
            MainMenu.Items.Add(Aimbot);
            MainMenu.Items.Add(LocalPlayer);
            MainMenu.Items.Add(PlayerMenu);
            MainMenu.Items.Add(Colours);
            MainMenu.Items.Add(Config);
            Weapons();
            Configs();
            ESP();
            Aimbots();
            Skills();
            Movements();
            Properties();
            Worlds();
            #region Colour Picker
            // amount of colours in the dictionary is always the same in game so we dont need to update this.
            foreach (KeyValuePair<string, Color32> value in Globals.Config.Colours.GlobalColors)
            {
                SubMenu colourmenu = new SubMenu(value.Key,"");
                int alpha = Helpers.ColourHelper.GetColour(value.Key).a;
                IntSlider slidera = new IntSlider("Alpha", "Change The Colour Opacity", ref alpha, 0, 255, 10);
                int red = Helpers.ColourHelper.GetColour(value.Key).r;
                IntSlider sliderr = new IntSlider("Red", "Change Amount Of Red In Colour", ref red, 0, 255, 10);
                int green = Helpers.ColourHelper.GetColour(value.Key).g;
                IntSlider sliderg = new IntSlider("Green", "Change Amount Of Green In Colour", ref green, 0, 255, 10);
                int blue = Helpers.ColourHelper.GetColour(value.Key).b;
                IntSlider sliderb = new IntSlider("Blue", "Change Amount Of Blue In Colour", ref blue, 0, 255, 10);
                colourmenu.Items.Add(slidera);
                colourmenu.Items.Add(sliderr);
                colourmenu.Items.Add(sliderg);
                colourmenu.Items.Add(sliderb);
                colourmenu.Items.Add(new Button("Save Colour", "Right Arrow To Save The Colour", () => Helpers.ColourHelper.SetColour(value.Key, new Color32((byte)red, (byte)green, (byte)blue, (byte)alpha))));
                Colours.Items.Add(colourmenu);
            }
            #endregion


        }
        [ObfuscationAttribute(Exclude = true)]
        void OnGUI()
        {
            OnGUI1();

        }
        [ObfuscationAttribute(Exclude = true)]
        void Update()
        {
            Update1();
          
        }
        void OnGUI1()
        {
          
            Globals.MainCamera = Camera.main;

            string text = string.Empty;
            if (MenuHistory.Count > 0)
            {
                foreach (SubMenu subMenu in MenuHistory)
                {
                    if (subMenu != null)
                    {
                        if (subMenu == MenuHistory.Last<SubMenu>())
                        {
                            text += subMenu.Name + " v ";
                        }
                        else
                        {
                            text = text + subMenu.Name + " > ";
                        }
                    }
                }
            }

            Drawing.DrawString(new Vector2(MenuPos.x - 5, MenuPos.y - 20), text, Helpers.ColourHelper.GetColour("Menu Primary Colour"), false, 12, FontStyle.Normal, 0); // draw menu history
            #region PlayerMenu
            if (CurrentMenu == PlayerMenu)
            {
                // so basically we can add options to the playerlist here so we can make a button to kill that player etc
                if (Time.time > NextPlayerTime)
                {
                    PlayerMenu.Items.Clear();
                    foreach (EntityPlayer player in Cheat.Esp.Player.PlayerList)
                    {
                        if (player == null)
                            continue;
                        if (!player.IsAlive())
                            continue;
                        SubMenu playermenu = new SubMenu(player.EntityName,"");
                        PlayerMenu.Items.Add(playermenu);
                        playermenu.Items.Add(new Button("Kill Player", "JFKs The Player", () => Cheat.Misc.KillPlayer(player)));
                        playermenu.Items.Add(new Button("Teleport To Player", "Teleports You To Player", () => Misc.TeleportToPlayer(player)));
                    }
                    NextPlayerTime = Time.time + 2;
                }
            }
            #endregion
            foreach (Entity entity in CurrentMenu.Items)
            {
                
                if (Selected == entity)
                {
                    if (entity.Description != null)
                        Drawing.DrawString(new Vector2(MenuPos.x - 5, MenuPos.y + (20f * (float)CurrentMenu.Items.Count)), entity.Description, Helpers.ColourHelper.GetColour("Menu Primary Colour"), false, 12, FontStyle.Normal, 0);
                    if (entity is Toggle)
                    {
                        Toggle toggle = entity as Toggle;
                        string ToggleStr = toggle.Value ? "Enabled" : "Disabled";
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}: {ToggleStr}", Helpers.ColourHelper.GetColour("Menu Primary Colour"), false, 14, FontStyle.Normal, 0);
                    }
                    if (entity is IntSlider)
                    {
                        IntSlider slider = entity as IntSlider;
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}: {slider.Value}", Helpers.ColourHelper.GetColour("Menu Primary Colour"), false, 14, FontStyle.Normal, 0);
                    }
                    if (entity is FloatSlider)
                    {
                        FloatSlider slider = entity as FloatSlider;
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}: {slider.Value}", Helpers.ColourHelper.GetColour("Menu Primary Colour"), false, 14, FontStyle.Normal, 0);
                    }
                    if (entity is Keybind)
                    {
                        Keybind bind = entity as Keybind;
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}: {bind.Value.ToString()}", Helpers.ColourHelper.GetColour("Menu Primary Colour"), false, 14, FontStyle.Normal, 0);
                    }
                    if (entity is SubMenu)
                    Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"> {entity.Name}", Helpers.ColourHelper.GetColour("Menu Primary Colour"), false, 14, FontStyle.Normal, 0);
                    if (entity is Button)
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}", Helpers.ColourHelper.GetColour("Menu Primary Colour"), false, 14, FontStyle.Normal, 0);
                }
                else
                {
                    if (entity is Toggle)
                    {
                        Toggle toggle = entity as Toggle;
                        string ToggleStr = toggle.Value ? "Enabled" : "Disabled";
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}: {ToggleStr}", Helpers.ColourHelper.GetColour("Menu Secondary Colour"), false, 12, FontStyle.Normal, 0);
                    }
                    if (entity is IntSlider)
                    {
                        IntSlider slider = entity as IntSlider;
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}: {slider.Value}", Helpers.ColourHelper.GetColour("Menu Secondary Colour"), false, 12, FontStyle.Normal, 0);
                    }
                    if (entity is FloatSlider)
                    {
                        FloatSlider slider = entity as FloatSlider;
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}: {slider.Value}", Helpers.ColourHelper.GetColour("Menu Secondary Colour"), false, 12, FontStyle.Normal, 0);
                    }
                    if (entity is Keybind)
                    {
                        Keybind bind = entity as Keybind;
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}: {bind.Value.ToString()}", Helpers.ColourHelper.GetColour("Menu Secondary Colour"), false, 12, FontStyle.Normal, 0);
                    }
                    if (entity is SubMenu)
                    Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"> {entity.Name}", Helpers.ColourHelper.GetColour("Menu Secondary Colour"), false, 12, FontStyle.Normal, 0);
                    if(entity is Button)
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}", Helpers.ColourHelper.GetColour("Menu Secondary Colour"), false, 12, FontStyle.Normal, 0);
                }
            }
              
            }
        void Update1()
        {

            #region Controls
            if (Input.GetKeyDown(KeyCode.DownArrow) && CurrentMenu.index < CurrentMenu.Items.Count)
                CurrentMenu.index++;
            if (Input.GetKeyDown(KeyCode.UpArrow) && CurrentMenu.index > 0)
                CurrentMenu.index--;
            if (Input.GetKeyDown(KeyCode.Backspace) && MenuHistory.Count > 1)
            {
                CurrentMenu = MenuHistory[MenuHistory.Count - 2];
                MenuHistory.Remove(MenuHistory.Last<SubMenu>());
                return;
            }
            if (((Input.GetKeyDown(KeyCode.LeftArrow) && Selected is SubMenu)) && CurrentMenu.index < CurrentMenu.Items.Count)
            {

                CurrentMenu = MenuHistory[MenuHistory.Count - 2];
                MenuHistory.Remove(MenuHistory.Last<SubMenu>());
                return;
            }
            foreach (Entity entity in CurrentMenu.Items)
            {
            
                if (CurrentMenu.index == CurrentMenu.Items.IndexOf(entity))
                    Selected = entity;
                if (entity != Selected)
                    continue;

            }
            if (Selected is Keybind)
            {
                Keybind bind = Selected as Keybind;
                if (bind.Value == KeyCode.None)
                    bind.Value = SetKey();

            }
            if (Selected is SubMenu && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Return)))
            {
                CurrentMenu = Selected as SubMenu;
                MenuHistory.Add(Selected as SubMenu);
                return; // opens a new menu so we need to exit the loop to then render our new currentmenu
            }
            if (Selected is Toggle && Input.GetKeyDown(KeyCode.RightArrow))
            {
                Toggle toggle = Selected as Toggle;
                toggle.Value = true;
            }
            if (Selected is Toggle && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Toggle toggle = Selected as Toggle;
                toggle.Value = false;
            }
            if (Selected is Button && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Return)))
            {
                Button button = Selected as Button;
                button.Method();
            }
            if (Selected is IntSlider && Input.GetKeyDown(KeyCode.RightArrow))
            {
                IntSlider slider = Selected as IntSlider;
                int result = slider.Value + slider.IncrementValue;

                if (result > slider.MaxValue)
                    slider.Value = slider.MaxValue;
                else
                    slider.Value = result;
            }
            if (Selected is IntSlider && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                IntSlider slider = Selected as IntSlider;
                int result = slider.Value - slider.IncrementValue;

                if (result < slider.MinValue)
                    slider.Value = slider.MinValue;
                else
                    slider.Value = result;
            }
            if (Selected is FloatSlider && Input.GetKeyDown(KeyCode.RightArrow))
            {
                FloatSlider slider = Selected as FloatSlider;
                float result = slider.Value + slider.IncrementValue;

                if (result > slider.MaxValue)
                    slider.Value = slider.MaxValue;
                else
                    slider.Value = result;
            }
            if (Selected is FloatSlider && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                FloatSlider slider = Selected as FloatSlider;
                float result = slider.Value - slider.IncrementValue;

                if (result < slider.MinValue)
                    slider.Value = slider.MinValue;
                else
                    slider.Value = result;
            }
            if (Selected is Keybind && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Return)))
            {
                Keybind bind = Selected as Keybind;
                bind.Value = KeyCode.None;
            }
            #endregion
        }
    }
}
