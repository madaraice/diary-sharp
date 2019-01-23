using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using static GlobalVars;
using WMPLib;

namespace Kursach
{
    public partial class currentEvent : Form
    {
        WMPLib.WindowsMediaPlayer WMP = new WMPLib.WindowsMediaPlayer();

        private Timer refresh;

        public currentEvent(Timer timer)
        {
            TopMost = true;
            refresh = timer;

            InitializeComponent();

            tableLayoutPanel1.BackgroundImage = new Bitmap(@"delBg.png");
            tableLayoutPanel2.BackgroundImage = new Bitmap(@"delBg.png");
            this.BackgroundImage = new Bitmap(@"delBg.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            Color bg = ColorTranslator.FromHtml("#fffee9");
            label1.BackColor = label2.BackColor = label3.BackColor =
            label4.BackColor = label5.BackColor = label6.BackColor = bg;

            label1.Text = "Наступило событие:";
            label2.Text = "Описание:";
            label3.Text = "Степень важности:";
        }

        private void currentEvent_Load(object sender, EventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM [Reminding] WHERE [Id]='" + minID + "'";

                SQLiteCommand command = new SQLiteCommand(query, connection);

                using (SQLiteDataReader sqlReader = command.ExecuteReader())
                {
                    while (sqlReader.Read())
                    {
                        this.Text = Convert.ToString(sqlReader["Header"]);

                        label4.Text = $"{Convert.ToString(sqlReader["Header"])} - {Convert.ToString(sqlReader["Date"])}";

                        WMP.URL = Convert.ToString(sqlReader["Sound"]);

                        tempDate = Convert.ToDateTime(sqlReader["Date"]);

                        label5.Text = Convert.ToString(sqlReader["Description"]);

                        label6.Text = Convert.ToString(sqlReader["Importance"]);

                        text = Convert.ToString(sqlReader["Description"]);

                        importance = Convert.ToString(sqlReader["Importance"]);

                        date = Convert.ToString(sqlReader["Date"]);

                        dateCreated = Convert.ToString(sqlReader["DateNow"]);

                        header = Convert.ToString(sqlReader["Header"]);

                        sound = Convert.ToString(sqlReader["Sound"]);

                        edit = Convert.ToBoolean(sqlReader["Edit"]);

                        switch (label6.Text)
                        {
                            case "!"://!
                                Color green = ColorTranslator.FromHtml("#19FF19");
                                label6.ForeColor = green;
                                break;
                            case "!!"://!!
                                Color orange = ColorTranslator.FromHtml("#FF9900");
                                label6.ForeColor = orange;
                                break;
                            case "!!!"://!!!
                                Color red = ColorTranslator.FromHtml("#F80000");
                                label6.ForeColor = red;
                                break;
                        }
                    }

                }

            }

            WMP.controls.play();

            WMP.settings.setMode("loop", true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            del = true;

            WMP.controls.stop();

            mainMenu frm = new mainMenu();

            frm.checkArray();

            refresh.Start();

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            question frm = new question();

            frm.ShowDialog();

            if (cls)
            {
                cls = false;

                WMP.controls.stop();

                mainMenu frmmn = new mainMenu();

                frmmn.checkArray();

                refresh.Start();

                this.Close();
            }
        }
    }
}
