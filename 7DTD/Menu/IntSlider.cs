using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheat.Menu
{
    class IntSlider : Entity
    {
        public unsafe IntSlider(string text, string description, ref int value,int minvalue,int maxvalue,int incrementvalue)
        {
            base.Name = text;
            base.Description = description;
            fixed (int* ptr = &value)
            {
                int* @Value = ptr;
                this.Int = @Value;
            }
            MinValue = minvalue;
            MaxValue = maxvalue;
            IncrementValue = incrementvalue;
        }
        public unsafe int Value
        {
            get
            {
                return *this.Int;
            }
            set
            {
                *this.Int = value;
            }
        }
        private unsafe int* Int;
        public int MaxValue;
        public int MinValue;
        public int IncrementValue;
    }
   
   
    
}
