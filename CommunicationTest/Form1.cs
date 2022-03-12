using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommunicationTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var namedPipeServer = new NamedPipeServerStream("my-very-cool-pipe-example", PipeDirection.InOut, 1, PipeTransmissionMode.Byte);
            var streamReader = new StreamReader(namedPipeServer);
            namedPipeServer.WaitForConnection();

            var writer = new StreamWriter(namedPipeServer);
            writer.Write("Coolio");
            writer.Write((char)0);
            writer.Flush();
            namedPipeServer.WaitForPipeDrain();
            MessageBox.Show($"read from pipe client: {streamReader.ReadLine()}");
          
            namedPipeServer.Dispose();
        }
    }
}
