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
    public partial class NewNote : Form
    {
        private Timer timer1;

        public NewNote(Timer timer)
        {
            timer1 = timer;

            InitializeComponent();

            tableLayoutPanel1.BackgroundImage = new Bitmap(@"delBg.png");
            tableLayoutPanel2.BackgroundImage = new Bitmap(@"delBg.png");
            tableLayoutPanel3.BackgroundImage = new Bitmap(@"delBg.png");
            //this.BackgroundImage = new Bitmap(@"delBg.png");
            //this.BackgroundImageLayout = ImageLayout.Stretch;
            Color bg = ColorTranslator.FromHtml("#fffee9");
            label1.BackColor = label2.BackColor = bg;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text) &&
                    !string.IsNullOrEmpty(richTextBox1.Text) && !string.IsNullOrWhiteSpace(richTextBox1.Text) &&
                    !string.IsNullOrEmpty(comboBox1.Text) && !string.IsNullOrWhiteSpace(comboBox1.Text))//если строка не пустая
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        string query = "INSERT INTO [Notes] (Header, Text, Date, Color, Edit)VALUES(@Header, @Text, @Date, @Color, @Edit)";

                        SQLiteCommand command = new SQLiteCommand(query, connection);

                        command.Parameters.AddWithValue("Header", textBox1.Text);// включить обработку текста как в php htmlpecialchar

                        command.Parameters.AddWithValue("Text", richTextBox1.Text);

                        command.Parameters.AddWithValue("Date", DateTime.Now);

                        command.Parameters.AddWithValue("Color", comboBox1.Text);//добавить проверку на те цвета которые в комбобоксе

                        command.Parameters.AddWithValue("Edit", false);

                        command.ExecuteNonQuery();

                        timer1.Start();

                        this.Close();
                    }

                }
                else
                {
                    MessageBox.Show("Поля не должны быть пустыми!");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
