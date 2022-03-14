using System;
using System.Collections.Generic;
using System.Linq;
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
        #region LocalPlayer
        void LocalPlayers()
        {
            SubMenu Weapon = new SubMenu("Weapon", "Modify Your Weapon");
            Button RemoveRecoil = new Button("Remove Recoil", "!This Cant Be Undone! This Button Removes Recoil", () => Misc.EnableNoRecoil());
            Toggle NoRecoil = new Toggle("No Recoil From Start Up", "This Will Remove Recoil On Start Or Config Load", ref Globals.Config.LocalPlayer.NoRecoil);
            Button RemoveSpread = new Button("Remove Spread", "!This Cant Be Undone! This Button Removes Spread", () => Misc.EnableNoSpread());
            Toggle NoSpread = new Toggle("No Spread From Start Up", "This Will Remove Spread On Start Or Config Load", ref Globals.Config.LocalPlayer.NoSpread);
            Toggle NoViewBob = new Toggle("No Shake", "Removes Shake/ViewBob", ref Globals.Config.LocalPlayer.NoViewBob);
            Toggle UnlimitedAmmo = new Toggle("Unlimited Ammo", "Gives You Unlimited Ammo", ref Globals.Config.LocalPlayer.UnlimitedAmmo);
            Toggle UnlimitedRange = new Toggle("Unlimited Range", "Allows You To Shoot Further", ref Globals.Config.LocalPlayer.UnlimitedRange);
            Weapon.Items.Add(RemoveRecoil);
            Weapon.Items.Add(NoRecoil);
            Weapon.Items.Add(RemoveSpread);
            Weapon.Items.Add(NoSpread);
            Weapon.Items.Add(NoViewBob);
            Weapon.Items.Add(UnlimitedAmmo);
            LocalPlayer.Items.Add(Weapon);
        }
        #endregion
        #region Aimbot
        void Aimbots()
        {
            IntSlider fov = new IntSlider("Aimbot Fov", "Circle Range From Centre Of Your Screen That Aimbot Will Target", ref Globals.Config.Aimbot.Fov, 0, 1200, 25);
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
            Toggle visibleplayer = new Toggle("Visibility Check", "Aimbot Only Targets Visible Players", ref Globals.Config.Aimbot.PlayerVisibilityChecks);
            PlayerAimbot.Items.Add(targetplayer);
            PlayerAimbot.Items.Add(visibleplayer);

            Toggle targetzombie = new Toggle("Enable Zombie Aimbot", "Makes Aimbot Target Zombies", ref Globals.Config.Aimbot.ZombieAimbot);
            Toggle visiblezombie = new Toggle("Visibility Check", "Aimbot Only Targets Visible Zombies", ref Globals.Config.Aimbot.ZombieVisibilityChecks);
            ZombieAimbot.Items.Add(targetzombie);
            ZombieAimbot.Items.Add(visiblezombie);

        }
        #endregion
        void Start()
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
            LocalPlayers();
            Configs();
            ESP();
            Aimbots();
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
        void OnGUI()
        {
            OnGUI1();

        }
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
                        playermenu.Items.Add(new Button("Kill Player", "Kills The Player", () => Cheat.Misc.KillPlayer(player)));
                    }
                    NextPlayerTime = Time.time + 5;
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

            foreach (Entity entity in CurrentMenu.Items)
            {
                if (CurrentMenu.index == CurrentMenu.Items.IndexOf(entity))
                    Selected = entity;
                if (entity != Selected)
                    continue;
                if (((Input.GetKeyDown(KeyCode.LeftArrow) && Selected is SubMenu)) && CurrentMenu.index < CurrentMenu.Items.Count)
                {
                 
                }
                if (Selected is SubMenu && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Return)))
                {
                    CurrentMenu = entity as SubMenu;
                    MenuHistory.Add(entity as SubMenu);
                    return; // opens a new menu so we need to exit the loop to then render our new currentmenu
                }
                if (Selected is Toggle && Input.GetKeyDown(KeyCode.RightArrow))
                {
                    Toggle toggle = entity as Toggle;
                    toggle.Value = true;
                }
                if (Selected is Toggle && Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    Toggle toggle = entity as Toggle;
                    toggle.Value = false;
                }
                if (Selected is Button && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Return)))
                {
                    Button button = entity as Button;
                    button.Method();
                }
                if (Selected is IntSlider && Input.GetKeyDown(KeyCode.RightArrow))
                {
                    IntSlider slider = entity as IntSlider;
                    int result = slider.Value + slider.IncrementValue;

                    if (result > slider.MaxValue)
                        slider.Value = slider.MaxValue;
                    else
                        slider.Value = result;
                }   
                if (Selected is IntSlider && Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    IntSlider slider = entity as IntSlider;
                    int result = slider.Value - slider.IncrementValue;

                    if (result < slider.MinValue)
                        slider.Value = slider.MinValue;
                    else
                        slider.Value = result;
                }
                if (Selected is FloatSlider && Input.GetKeyDown(KeyCode.RightArrow))
                {
                    FloatSlider slider = entity as FloatSlider;
                    float result = slider.Value + slider.IncrementValue;

                    if (result > slider.MaxValue)
                        slider.Value = slider.MaxValue;
                    else
                        slider.Value = result;
                }
                if (Selected is FloatSlider && Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    FloatSlider slider = entity as FloatSlider;
                    float result = slider.Value - slider.IncrementValue;

                    if (result < slider.MinValue)
                        slider.Value = slider.MinValue;
                    else
                        slider.Value = result;
                }
            }
            #endregion
        }
    }
}
