using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat.Menu
{
    class Main : MonoBehaviour
    {
        Vector2 MenuPos;
        uint MainMenuIndex = 0;
        private Entity Selected;
        SubMenu MainMenu = new SubMenu("Main","Menu");

        SubMenu Esp = new SubMenu("ESP", "Draw Visuals");
        SubMenu Aimbot = new SubMenu("Aimbot", "Lock Onto Enemies");

        List<SubMenu> MenuHistory = new List<SubMenu>();
        public static SubMenu CurrentMenu;
        void Start()
        {
            MenuPos.x = 50;
            MenuPos.y = 100;
            MenuHistory.Add(MainMenu);
            CurrentMenu = MainMenu;
            MainMenu.Items.Add(Esp);
            MainMenu.Items.Add(Aimbot);
          //  test.Items.Add(new Toggle("Toggle Test", "Nah", ref Globals.Config.Player.Name));
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
            
           /* if (currentMenu.Items.Count == 0)
            {
                return;
            }*/
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
            Drawing.DrawString(new Vector2(MenuPos.x -5, MenuPos.y - 20), text, Color.red, false, 12, FontStyle.Normal, 0); // draw menu history

            foreach (Entity entity in CurrentMenu.Items)
            {
                if (Selected == entity)
                {
                    Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), "> " + entity.Name, Color.white, false, 12, FontStyle.Normal, 0);
                }
                else
                {
                    Drawing.DrawString(new Vector2(MenuPos.x, MenuPos.y + (20 * CurrentMenu.Items.IndexOf(entity))), entity.Name, Color.white, false, 12, FontStyle.Normal, 0);
                }
            }
              
            }
        void Update1()
        {
            foreach (Entity entity in CurrentMenu.Items)
            {
                if (CurrentMenu.index == CurrentMenu.Items.IndexOf(entity))
                    Selected = entity;
                if (Input.GetKeyDown(KeyCode.DownArrow) && CurrentMenu.index < CurrentMenu.Items.Count)
                    CurrentMenu.index++;
                if (Input.GetKeyDown(KeyCode.UpArrow) && CurrentMenu.index > 0)
                    CurrentMenu.index--;
            }
        }
    }
}
