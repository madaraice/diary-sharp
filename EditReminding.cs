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
    public partial class EditReminding : Form
    {
        private Timer refresh;

        private Timer chk;

        public EditReminding(Timer timer)//, Timer timer2)
        {
            refresh = timer;

            //chk = timer2;

            InitializeComponent();

            currentTime.Start();

            this.Text = $"Редактирование напоминания «{header}»";
        }

        private void EditReminding_Load(object sender, EventArgs e)
        {
            if (edit)
            {
                label1.Text = $"Напоминание было отредактировано: {dateCreated}";
            }
            else
            {
                label1.Text = $"Напоминание было создано: {dateCreated}";
            }

            textBox1.Text = header;

            richTextBox1.Text = text;

            comboBox1.Text = importance;

            int symbol_index = Convert.ToString(date).IndexOf(' ');

            string DMY = Convert.ToString(date).Substring(0, symbol_index);

            int tempCount = Convert.ToString(date).Count();

            string HMS = Convert.ToString(date).Substring(symbol_index, tempCount - symbol_index);

            DMY = DMY.Replace(" ", "");

            HMS = HMS.Replace(" ", "");

            maskedTextBox1.Text = DMY;

            maskedTextBox2.Text = HMS;
        }

        private void currentTime_Tick(object sender, EventArgs e)
        {
            label2.Text = "Текущее время: " + Convert.ToString(DateTime.Now);
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
                sound = oFD.FileName;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(richTextBox1.Text) && !string.IsNullOrWhiteSpace(richTextBox1.Text) &&
                    !string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text) &&
                    !string.IsNullOrEmpty(maskedTextBox1.Text) && !string.IsNullOrWhiteSpace(maskedTextBox1.Text) &&
                    !string.IsNullOrEmpty(maskedTextBox2.Text) && !string.IsNullOrWhiteSpace(maskedTextBox2.Text))//если строка не пустая
                {
                    date = maskedTextBox1.Text + " " + maskedTextBox2.Text;

                    if (Convert.ToDateTime(date) > DateTime.Now)
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                        {
                            connection.Open();

                            string query = "UPDATE [Reminding] SET [Header]=@Header, [Description]=@Description, [Date]=@Date, [DateNow]=@DateNow, [Importance]=@Importance, [Sound]=@Sound,[Edit]=@Edit WHERE [Id]=@Id";

                            SQLiteCommand command = new SQLiteCommand(query, connection);

                            command.Parameters.AddWithValue("Id", numberID);

                            command.Parameters.AddWithValue("Description", richTextBox1.Text);

                            command.Parameters.AddWithValue("Header", textBox1.Text);

                            date = maskedTextBox1.Text + " " + maskedTextBox2.Text;

                            command.Parameters.AddWithValue("Date", Convert.ToDateTime(date));

                            command.Parameters.AddWithValue("DateNow", DateTime.Now);

                            command.Parameters.AddWithValue("Importance", comboBox1.Text);//добавить проверку на те цвета которые в комбобоксе

                            command.Parameters.AddWithValue("Sound", sound);

                            command.Parameters.AddWithValue("Edit", true);

                            command.ExecuteNonQuery();

                            this.Text = $"Редактирование напоминания «{textBox1.Text}»";

                            refresh.Start();

                            //chk.Stop();

                            mainMenu frm = new mainMenu();

                            frm.checkArray();

                            label1.Text = "Напоминание отредактировано " + DateTime.Now;

                            MessageBox.Show("Напоминание успешно отредактировано!");

                            this.Close();
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);//1-сообщение,2-заголовок
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO [DeletedReminders] (Header, Description, Date, DateNow, Importance, Sound, Edit)VALUES(@Header, @Description, @Date, @DateNow, @Importance, @Sound, @Edit)";

                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    command.Parameters.AddWithValue("Id", numberID);

                    command.Parameters.AddWithValue("Description", text);

                    command.Parameters.AddWithValue("Header", header);

                    date = maskedTextBox1.Text + " " + maskedTextBox2.Text;

                    command.Parameters.AddWithValue("Date", date);

                    command.Parameters.AddWithValue("DateNow", dateCreated);

                    command.Parameters.AddWithValue("Importance", importance);

                    command.Parameters.AddWithValue("Sound", sound);

                    command.Parameters.AddWithValue("Edit", edit);

                    command.ExecuteNonQuery();

                    query = "DELETE FROM [Reminding] WHERE [Id]=@Id";

                    command = new SQLiteCommand(query, connection);

                    command.Parameters.AddWithValue("Id", numberID);

                    command.ExecuteNonQuery();

                    MessageBox.Show("Напоминание перемещено в корзину!");

                    refresh.Start();

                    mainMenu frm = new mainMenu();

                    frm.checkArray();

                    this.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
