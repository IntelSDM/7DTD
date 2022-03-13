using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheat.Menu
{
    class FloatSlider : Entity
    {
        public unsafe FloatSlider(string text, string description, ref float value, float minvalue, float maxvalue, float incrementvalue)
        {
            base.Name = text;
            base.Description = description;
            fixed (float* ptr = &value)
            {
                float* @Value = ptr;
                this.Float = @Value;
            }
            MinValue = minvalue;
            MaxValue = maxvalue;
            IncrementValue = incrementvalue;
        }
        public unsafe float Value
        {
            get
            {
                return *this.Float;
            }
            set
            {
                *this.Float = value;
            }
        }
        private unsafe float* Float;
        public float MaxValue;
        public float MinValue;
        public float IncrementValue;
    }



}
