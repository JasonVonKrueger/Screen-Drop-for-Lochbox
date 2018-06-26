using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Web;
using System.Net;

namespace Screen_Drop_for_LochBox
{
    public partial class SnippingTool : Form
    {
        // privates
        private Rectangle rcSelect = new Rectangle();
        private Point pntStart;
        private string _DropPath = null;

        public Image Image { get; set; }   

        public SnippingTool()
        {
            InitializeComponent();
        }

        public SnippingTool(Image screenShot)
        {
            InitializeComponent();
            this.BackgroundImage = screenShot;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.DoubleBuffered = true;
        }

        public static Image Snip()
        {
            var rc = Screen.PrimaryScreen.Bounds;

            using (Bitmap bmp = new Bitmap(rc.Width, rc.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb))
            {
                using (Graphics gr = Graphics.FromImage(bmp))
                    gr.CopyFromScreen(0, 0, 0, 0, bmp.Size);
                using (var snipper = new SnippingTool(bmp))
                {
                    if (snipper.ShowDialog() == DialogResult.OK)
                    {
                        return snipper.Image;
                    }
                }
                return null;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Start the snip on mouse down
            if (e.Button != MouseButtons.Left) return;
            pntStart = e.Location;
            rcSelect = new Rectangle(e.Location, new Size(0, 0));
            this.Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Modify the selection on mouse move
            if (e.Button != MouseButtons.Left) return;
            int x1 = Math.Min(e.X, pntStart.X);
            int y1 = Math.Min(e.Y, pntStart.Y);
            int x2 = Math.Max(e.X, pntStart.X);
            int y2 = Math.Max(e.Y, pntStart.Y);
            rcSelect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Complete the snip on mouse-up
            if (rcSelect.Width <= 0 || rcSelect.Height <= 0)
                return;

            Image = new Bitmap(rcSelect.Width, rcSelect.Height);

            using (Graphics gr = Graphics.FromImage(Image))
            {
                gr.DrawImage(this.BackgroundImage, new Rectangle(0, 0, Image.Width, Image.Height),
                    rcSelect, GraphicsUnit.Pixel);
            }

            DialogResult = DialogResult.OK;

            // Play the shutter sound
            using (System.Media.SoundPlayer player = new System.Media.SoundPlayer())
            {
                player.Stream = Properties.Resources.camera_shutter_click_01;
                player.Play();
            }

            // save the file
            if (saveImage(Image))
            {
                // show the links form
                frmResults resultsForm = new frmResults(_DropPath);
                
                resultsForm.Show();
                resultsForm.ShowInTaskbar = false;
            }
            else
            {
                MessageBox.Show("Could not save the image!", "Well, darn!");
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw the current selection
            using (Brush br = new SolidBrush(Color.FromArgb(150, Color.Black)))
            {
                int x1 = rcSelect.X; int x2 = rcSelect.X + rcSelect.Width;
                int y1 = rcSelect.Y; int y2 = rcSelect.Y + rcSelect.Height;
                e.Graphics.FillRectangle(br, new Rectangle(0, 0, x1, this.Height));
                e.Graphics.FillRectangle(br, new Rectangle(x2, 0, this.Width - x2, this.Height));
                e.Graphics.FillRectangle(br, new Rectangle(x1, 0, x2 - x1, y1));
                e.Graphics.FillRectangle(br, new Rectangle(x1, y2, x2 - x1, this.Height - y2));
            }

            using (Pen pen = new Pen(Color.Orange, 3))
            {
                e.Graphics.DrawRectangle(pen, rcSelect);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Allow canceling the snip with the Escape key
            if (keyData == Keys.Escape) this.DialogResult = DialogResult.Cancel;
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private bool saveImage(Image image)
        {
            try
            {
                // generate a unique file name...
                string strFilename = DateTime.Now.Ticks.ToString() + ".jpg";
                string strUsername = Environment.GetEnvironmentVariable("USERNAME");
                string strHomeDrive = Environment.GetEnvironmentVariable("HOMEDRIVE");
                string strHomePath = Environment.GetEnvironmentVariable("HOMEPATH");
                string strDestPath = @"\\lochbox.clayton.edu\home\" + strUsername + @"\_Public\ScreenDrops";

                _DropPath = GetShortenedURL("https://lochbox.clayton.edu/users/" + strUsername + "/screendrops/" + strFilename);

                // make sure the ScreenDrops folder exists in the user's home folder
                if (!Directory.Exists(strDestPath))
                    Directory.CreateDirectory(strDestPath);

                image.Save(strDestPath + "\\" + strFilename, System.Drawing.Imaging.ImageFormat.Jpeg);

                System.Threading.Thread.Sleep(200);

                return true;
            }
            catch (Exception x)
            {
                return false;
            }
        }

        // get short URL
        private string GetShortenedURL(string inURL)
        {
            // returns the homemade url shortener thingy ma bob
            string shortURL = inURL;
            inURL = shortURL;

            string queryURL = "https://lochbox.clayton.edu/publicwebservices/url.asmx/getShortURL?strOriginalURL=" + inURL.Replace("https://lochbox.clayton.edu/", "");

            using (WebClient wc = new WebClient())
            {
                Stream data = wc.OpenRead(queryURL);

                using (StreamReader reader = new StreamReader(data))
                {
                    shortURL = reader.ReadToEnd();
                    data.Close();
                    reader.Close();
                }
            }
            return shortURL;
        }  

    }
}
