using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EdgeDetectionWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Bitmap inputBitmap = new Bitmap(2, 2);

            inputBitmap.SetPixel(0, 0, Color.Black);
            inputBitmap.SetPixel(1, 0, Color.White);
            inputBitmap.SetPixel(0, 1, Color.White);
            inputBitmap.SetPixel(1, 1, Color.Black);
            inputBitmap.Save("1.bmp");
            
            pictureBox1.Image = inputBitmap;
        }
    }
}
