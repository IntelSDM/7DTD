using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheat.Configs
{
    class Config
    {
        public Colours Colours = new Colours();
        public Player Player = new Player();
        public Zombie Zombie = new Zombie();
        public Animal Animal = new Animal();
        public LocalPlayer LocalPlayer = new LocalPlayer();
        public Aimbot Aimbot = new Aimbot();
        public Friends Friends = new Friends();
        public Tiles Tiles = new Tiles();
        public Debug Debug = new Debug();
    }
}
