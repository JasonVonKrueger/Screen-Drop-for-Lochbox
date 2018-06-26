using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.NetworkInformation;
using System.Net;

namespace Screen_Drop_for_LochBox
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // make sure lochbox is available
            if (!LochPing())
            {
                MessageBox.Show("LochBox server not available.", "Oh, darn!");
                Application.Exit();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
        }

        static bool LochPing()
        {
            // ping lochbox
            Ping pingSender = new Ping();
            IPAddress address = IPAddress.Parse("168.28.240.205");
            PingReply reply = pingSender.Send(address);

            if (reply.Status == IPStatus.Success)            
                return true;            
            else
                return false;
        }
    }
}
