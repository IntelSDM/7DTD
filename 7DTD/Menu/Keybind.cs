using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat.Menu
{
    class Keybind : Entity
    {
        public unsafe KeyCode Value
        {
            get
            {
                return *this.Bool;
            }
            set
            {
                *this.Bool = value;
            }
        }
        public unsafe Keybind(string text, string description, ref KeyCode value)
        {
            base.Name = text;
            base.Description = description;
            fixed (KeyCode* ptr = &value)
            {
                KeyCode* @Value = ptr;
                this.Bool = @Value;
            }
        }
        private unsafe KeyCode* Bool;
    }
}
