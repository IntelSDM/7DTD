using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat
{
    class Loader : MonoBehaviour
    {
		// call auth Globals.Auth to load. This is for obfuscation reasons and to establish authenticity.
		public static void Init()
		{


			if (Globals.LoggedIn)
			{
				Loader.HackObject.AddComponent<Cheat.Esp.Zombie>();
				Loader.HackObject.AddComponent<Cheat.Esp.Animal>();
				Loader.HackObject.AddComponent<Cheat.Esp.Player>();
				Loader.HackObject.AddComponent<Cheat.Esp.Tiles>();
				Loader.HackObject.AddComponent<Globals>();
				Loader.HackObject.AddComponent<Menu.Main>();
				Loader.HackObject.AddComponent<Misc>();
				Loader.HackObject.AddComponent<Aimbot>();

				HackObject.AddComponent<Hooks.OnFired>();
				HackObject.AddComponent<Hooks.GetRange>();
				HackObject.AddComponent<Hooks.UpdateAccuracy>();
				HackObject.AddComponent<Hooks.IsOwner>();
				HackObject.AddComponent<Hooks.OnBlockDamaged>();

				UnityEngine.Object.DontDestroyOnLoad(HackObject);

			}

		}
		public static GameObject HackObject = new GameObject(); // make bool in a public static class and check if its true constantly as extra auth shit
	}
}
