using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cheat
{

    public class Load // renamed from auth so a moron looking through it in dnspy will have less of an idea what we are doing, we cant have this class obfuscated since we call it to load the cheat.
    {
        // Clean up our files, probably best to do this in our loader.
        // join locked/private servers
        // encrypt config
        [ObfuscationAttribute(Exclude = true)]
        public static void Start()
        {
           

            Globals.Auth();
        }
     
    }
}
