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
        SubMenu PlayerMenu = new SubMenu("Player Menu", "Allows You To Abuse Players");
        SubMenu Colours = new SubMenu("Colour Menu", "Allows You To Change Colours On The Cheat");
        SubMenu Config = new SubMenu("Config Menu", "Allows You To Save And Load Settings");

        List<SubMenu> MenuHistory = new List<SubMenu>();
        SubMenu CurrentMenu;

        void Start()
        {
            MenuPos.x = 50;
            MenuPos.y = 100;
            MenuHistory.Add(MainMenu);
            CurrentMenu = MainMenu;
            MainMenu.Items.Add(Esp);
            MainMenu.Items.Add(Aimbot);
            MainMenu.Items.Add(PlayerMenu);
            MainMenu.Items.Add(Colours);
            MainMenu.Items.Add(Config);

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

            Esp.Items.Add(new Toggle("Name", "sdggs", ref Globals.Config.Zombie.Name));
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

            Drawing.DrawString(new Vector2(MenuPos.x - 5, MenuPos.y - 20), text, Color.red, false, 12, FontStyle.Normal, 0); // draw menu history
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
                        Drawing.DrawString(new Vector2(10f, 180f + 20f * (float)CurrentMenu.Items.Count), entity.Description, Color.red, false, 12, FontStyle.Normal, 0);
                    if (entity is Toggle)
                    {
                        Toggle toggle = entity as Toggle;
                        string ToggleStr = toggle.Value ? "Enabled" : "Disabled";
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}: {ToggleStr}", Color.red, false, 14, FontStyle.Normal, 0);
                    }
                    if (entity is IntSlider)
                    {
                        IntSlider slider = entity as IntSlider;
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}: {slider.Value}", Color.red, false, 14, FontStyle.Normal, 0);
                    }
                    if (entity is FloatSlider)
                    {
                        FloatSlider slider = entity as FloatSlider;
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}: {slider.Value}", Color.red, false, 14, FontStyle.Normal, 0);
                    }
                    if (entity is SubMenu)
                    Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"> {entity.Name}", Color.red, false, 14, FontStyle.Normal, 0);
                    if (entity is Button)
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}", Color.red, false, 14, FontStyle.Normal, 0);
                }
                else
                {
                    if (entity is Toggle)
                    {
                        Toggle toggle = entity as Toggle;
                        string ToggleStr = toggle.Value ? "Enabled" : "Disabled";
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}: {ToggleStr}", Color.white, false, 12, FontStyle.Normal, 0);
                    }
                    if (entity is IntSlider)
                    {
                        IntSlider slider = entity as IntSlider;
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}: {slider.Value}", Color.white, false, 14, FontStyle.Normal, 0);
                    }
                    if (entity is FloatSlider)
                    {
                        FloatSlider slider = entity as FloatSlider;
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}: {slider.Value}", Color.white, false, 14, FontStyle.Normal, 0);
                    }
                    if (entity is SubMenu)
                    Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"> {entity.Name}", Color.white, false, 12, FontStyle.Normal, 0);
                    if(entity is Button)
                        Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), $"- {entity.Name}", Color.white, false, 12, FontStyle.Normal, 0);
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
