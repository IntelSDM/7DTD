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
    class Auth
    {
        // Clean up our files, probably best to do this in our loader.
        [ObfuscationAttribute(Exclude = true)]
        public static void Start()
        {
           

            Start1();
        }
        private static void Start1()
        {
            // alright so basically we make a pipe, our loader connects to this 
            var namedPipeServer = new NamedPipeServerStream("my-7dtd-pipe", PipeDirection.InOut, 1, PipeTransmissionMode.Byte);
            var streamReader = new StreamReader(namedPipeServer);
            namedPipeServer.WaitForConnection();

            var writer = new StreamWriter(namedPipeServer);
            writer.Write("Coolio");
            writer.Write((char)0);
            writer.Flush();
            namedPipeServer.WaitForPipeDrain();

            namedPipeServer.Dispose();
            Loader.Init();
        }
    }
}
