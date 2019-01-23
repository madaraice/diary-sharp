using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using static GlobalVars;
using System.Windows.Forms;

namespace Kursach
{
    public partial class missedEvent : Form
    {
        public missedEvent()
        {
            TopMost = true;

            InitializeComponent();

            tableLayoutPanel1.BackgroundImage = new Bitmap(@"delBg.png");
            tableLayoutPanel2.BackgroundImage = new Bitmap(@"delBg.png");
            this.BackgroundImage = new Bitmap(@"delBg.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            label1.Text = "Вы пропустили событие:";
            label2.Text = "Описание:";
            label3.Text = "Степень важности:";

            Color bg = ColorTranslator.FromHtml("#fffee9");
            label1.BackColor = label2.BackColor = label3.BackColor =
            label4.BackColor = label5.BackColor = label6.BackColor = bg;
        }

        private void missedEvent_Load(object sender, EventArgs e)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM [Reminding] WHERE [Id]='" + ID + "'";

                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    using (SQLiteDataReader sqlReader = command.ExecuteReader())
                    {
                        while (sqlReader.Read())
                        {
                            this.Text = $"Пропущенное событие: «{Convert.ToString(sqlReader["Header"])}»";

                            label4.Text = $"{Convert.ToString(sqlReader["Header"])} - {Convert.ToString(sqlReader["Date"])}";

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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            del = true;

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tempDate.AddHours(6) < DateTime.Now)
            {
                MessageBox.Show("Упс, событие которое произошло более шести часов назад нельзя перенести!\n" +
                    "Вы можете его восстановить из корзины.");
            }
            else
            {
                question frm = new question();

                frm.ShowDialog();

                if (cls)
                {
                    cls = false;

                    this.Close();
                }
            }
        }
    }
}
