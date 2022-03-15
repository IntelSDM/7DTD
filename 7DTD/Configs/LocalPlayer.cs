using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat.Configs
{
    class LocalPlayer
    {
        public bool NoSpread = true;
        public bool NoRecoil = true;
        public bool NoViewBob = true;
        public bool UnlimitedAmmo = true;
        public bool UnlimitedRange = true;

        public int Kills = 0;
        public int Deaths = 0;
        public int ZombieKills = 0;
        public int Level = 0;
        public int SkillPoints = 0;

        public KeyCode SpeedKey = KeyCode.B;
        public int SpeedAmount = 10;
        public bool Speedhack = false;

    }
}
