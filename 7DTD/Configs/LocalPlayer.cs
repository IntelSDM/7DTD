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
        public bool WeaponFovChanger = false;
        public bool CameraFovChanger = false;
        public int WeaponFov = 90;
        public int CameraFov = 90;

        public int Kills = 0;
        public int Deaths = 0;
        public int ZombieKills = 0;
        public int Level = 0;
        public int SkillPoints = 0;
        public int ItemsCrafted = 0;
        public int DistanceTravelled = 0;
        public int TimePlayed = 0;

        public KeyCode SpeedKey = KeyCode.B;
        public int SpeedAmount = 10;
        public bool Speedhack = false;
        public bool BtecNoclip = true;
        public KeyCode NoclipKey = KeyCode.N;
        public int NoclipSpeed = 5;


        public bool UnlimitedStamina = true;
        public bool UnlimitedHunger = true;
        public bool UnlimitedThirtst = true;
        public bool InstantHealth = true;
        public bool AllahMode = false;
        public bool SpoofName = true;
        public bool SpoofID = false;
        public bool RandomlySpoofName = false;
        public bool ClearDebuffs = false;

        public bool LandClaim = true;
        public bool InstantBreak1 = true;
        public bool InstantBreak2 = false;
        public bool InstantBreak3 = false;
        public bool DebugMenu = true;
        public bool CreativeMenu = true;
        public bool FarInteract = false;
        public int FarInteractDistance = 35;
        public bool OwnsVehicle = true;
        public bool NoFallDamage = true;

    }
}
