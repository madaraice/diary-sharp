using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using static delGlobalVars;
using System.Windows.Forms;

namespace Kursach
{
    public partial class DelRem : Form
    {
        private Timer refresh;

        public DelRem(Timer timer)
        {
            refresh = timer;

            InitializeComponent();

            this.Text = $"Удаленное напоминание «{header}»";

            currentTime.Start();
        }

        private void currentTime_Tick(object sender, EventArgs e)
        {
            label2.Text = "Текущее время: " + Convert.ToString(DateTime.Now);
        }

        private void DelRem_Load(object sender, EventArgs e)
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

                            string query = "INSERT INTO [Reminding] (Header, Description, Date, DateNow, Importance, Sound, Edit)VALUES(@Header, @Description, @Date, @DateNow,@Importance, @Sound, @Edit)";

                            SQLiteCommand command = new SQLiteCommand(query, connection);

                            command.Parameters.AddWithValue("Id", numberID);

                            command.Parameters.AddWithValue("Description", richTextBox1.Text);

                            command.Parameters.AddWithValue("Header", textBox1.Text);

                            //date = maskedTextBox1.Text + " " + maskedTextBox2.Text;

                            command.Parameters.AddWithValue("Date", Convert.ToDateTime(date));

                            command.Parameters.AddWithValue("DateNow", DateTime.Now);

                            command.Parameters.AddWithValue("Importance", comboBox1.Text);//добавить проверку на те цвета которые в комбобоксе

                            command.Parameters.AddWithValue("Sound", sound);

                            command.Parameters.AddWithValue("Edit", true);

                            command.ExecuteNonQuery();

                            query = "DELETE FROM [DeletedReminders] WHERE [Id]=@Id";

                            command = new SQLiteCommand(query, connection);

                            command.Parameters.AddWithValue("Id", numberID);

                            command.ExecuteNonQuery();

                            refresh.Start();

                            mainMenu frm = new mainMenu();

                            frm.checkArray();

                            MessageBox.Show("Напоминание успешно восстановлено!");

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

                    string query = "DELETE FROM [DeletedReminders] WHERE [Id]=@Id";

                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    command.Parameters.AddWithValue("Id", numberID);

                    command.ExecuteNonQuery();

                    MessageBox.Show("Напоминание удалено!");

                    refresh.Start();

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
