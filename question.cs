using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GlobalVars;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Kursach
{
    public partial class question : Form
    {
        public DateTime temp = tempDate;

        public question()
        {
            TopMost = true;
            this.BackgroundImage = new Bitmap(@"delBg.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            InitializeComponent();
            tableLayoutPanel1.BackgroundImage = new Bitmap(@"delBg.png");
            tableLayoutPanel2.BackgroundImage = new Bitmap(@"delBg.png");
            panel1.BackgroundImage = new Bitmap(@"delBg.png");
            Color bg = ColorTranslator.FromHtml("#fffee9");
            label1.BackColor = bg;
            radioButton1.BackColor = radioButton2.BackColor = radioButton3.BackColor =
            radioButton4.BackColor = radioButton5.BackColor = radioButton6.BackColor = bg;

            temp = tempDate;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            tempDate = temp;

            tempDate = tempDate.AddMinutes(5);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            tempDate = temp;

            tempDate = tempDate.AddMinutes(15);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            tempDate = temp;

            tempDate = tempDate.AddMinutes(30);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            tempDate = temp;

            tempDate = tempDate.AddHours(1);
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            tempDate = temp;

            tempDate = tempDate.AddHours(3);
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            tempDate = temp;

            tempDate = tempDate.AddHours(6);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tempDate < DateTime.Now)
            {
                MessageBox.Show("Установите корректное время.");
            }
            else
            {
                edt = true;

                cls = true;

                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
