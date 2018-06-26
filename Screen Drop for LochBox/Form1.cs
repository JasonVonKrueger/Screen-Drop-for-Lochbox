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
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnSnipIT_Click(object sender, EventArgs e)
        {
            // hide the form
            this.Hide();
            System.Threading.Thread.Sleep(200);

            var bmp = SnippingTool.Snip();
            if (bmp != null)
            {

            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }
    }
}
