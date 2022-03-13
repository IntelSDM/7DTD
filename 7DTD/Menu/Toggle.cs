using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheat.Menu
{
    class Toggle : Entity
    {
        public unsafe bool Value
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
        public unsafe Toggle(string text,string description,ref bool value)
        {
            base.Name = text;
            base.Description = description;
            fixed (bool* ptr = &value)
            {
                bool* @Value = ptr;
                this.Bool = @Value;
            }
        }
        private unsafe bool* Bool;
    }
}
