using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat.Menu
{
    class SubMenu : Entity
    {
        public SubMenu(string text,string description)
        {
            base.Name = text;
        }
        public List<Entity> Items = new List<Entity>();
        public int index = 0;

    }
}
