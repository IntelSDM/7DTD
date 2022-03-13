using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheat.Menu
{
    class Toggle : Entity
    {
        public Toggle(string text,string description,ref bool value)
        {
            base.Name = text;
            base.Description = description;
            base.BoolValue = value;
        }
    }
}
