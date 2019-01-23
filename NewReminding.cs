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
using System.IO;
using System.Reflection;

namespace Kursach
{
    public partial class NewReminding : Form
    {
        //public string dirPath = new FileInfo($"{Assembly.GetAssembly(GetType()).Location}").DirectoryName;

        public string pathSound = "defPath";

        private Timer refresh;

        public NewReminding(Timer timer)//, Timer timer2)
        {
            refresh = timer;

            InitializeComponent();

            tableLayoutPanel1.BackgroundImage = new Bitmap(@"delBg.png");
            tableLayoutPanel2.BackgroundImage = new Bitmap(@"delBg.png");
            tableLayoutPanel3.BackgroundImage = new Bitmap(@"delBg.png");
            flowLayoutPanel1.BackgroundImage = new Bitmap(@"delBg.png");
            //this.BackgroundImage = new Bitmap(@"delBg.png");
            //this.BackgroundImageLayout = ImageLayout.Stretch;
            Color bg = ColorTranslator.FromHtml("#fffee9");
            label1.BackColor = label2.BackColor = label3.BackColor = label4.BackColor = bg;

            currentTime.Start();
        }

        private void currentTime_Tick(object sender, EventArgs e)
        {
            label4.Text = "Текущее время: " + Convert.ToString(DateTime.Now);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog oFD = new OpenFileDialog()
            {
                Filter = "MP3|*.mp3",

                Multiselect = false,

                ValidateNames = true
            };

            if (oFD.ShowDialog() == DialogResult.OK)
            {
                pathSound = oFD.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            currentTime.Stop();

            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text) &&
                !string.IsNullOrEmpty(maskedTextBox1.Text) && !string.IsNullOrWhiteSpace(maskedTextBox1.Text) &&
                !string.IsNullOrEmpty(maskedTextBox2.Text) && !string.IsNullOrWhiteSpace(maskedTextBox2.Text) &&
                !string.IsNullOrEmpty(comboBox1.Text) && !string.IsNullOrWhiteSpace(comboBox1.Text) &&
                !string.IsNullOrEmpty(richTextBox1.Text) && !string.IsNullOrWhiteSpace(richTextBox1.Text))
            {
                date = maskedTextBox1.Text + " " + maskedTextBox2.Text;

                if (Convert.ToDateTime(date) > DateTime.Now)
                {
                    try
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                        {

                            connection.Open();

                            string query = "INSERT INTO [Reminding] (Header, Description, Date, DateNow, Importance, Sound, Edit)VALUES(@Header, @Description, @Date, @DateNow,@Importance, @Sound, @Edit)";

                            SQLiteCommand command = new SQLiteCommand(query, connection);

                            command.Parameters.AddWithValue("Header", textBox1.Text);

                            command.Parameters.AddWithValue("Description", richTextBox1.Text);

                            string Date = Convert.ToString(maskedTextBox1.Text) + " " + Convert.ToString(maskedTextBox2.Text) + ":00";

                            command.Parameters.AddWithValue("Date", Convert.ToDateTime(Date));

                            command.Parameters.AddWithValue("DateNow", DateTime.Now);

                            command.Parameters.AddWithValue("Importance", comboBox1.Text);

                            command.Parameters.AddWithValue("Sound", pathSound);

                            command.Parameters.AddWithValue("Edit", false);

                            command.ExecuteNonQuery();

                            refresh.Start();

                            mainMenu frm = new mainMenu();

                            frm.checkArray();

                            currentTime.Stop();

                            this.Close();
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Укажите корректное время!");
                }
            }
            else
            {
                MessageBox.Show("Поля не должны быть пустыми!");
            }
        }
    }
}
