using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursach
{
    public partial class aboutProgram : Form
    {
        public aboutProgram()
        {
            InitializeComponent();
            tableLayoutPanel1.BackgroundImage = new Bitmap(@"delBg.png");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
