using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Screen_Drop_for_LochBox
{
    public partial class frmResults : Form
    {
        public frmResults(string linkLabel1Text)
        {
            InitializeComponent();

            // set the link label
            this.linkLabel1.Text = linkLabel1Text;
            this.linkLabel1.Links.Add(0, 150, linkLabel1Text);                    
        }

        private void frmResults_FormClosed(object sender, FormClosedEventArgs e)
        {
            // exit the app
            Application.Exit();
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            // set the clipboard text
            Clipboard.SetText(this.linkLabel1.Text);

            Application.Exit();
        }
    }
}
