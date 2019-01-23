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
    public partial class DelNote : Form
    {
        private Timer refreshNote;

        public DelNote(Timer timer)
        {
            refreshNote = timer;

            InitializeComponent();

            this.Text = $"Удаленная заметка «{header}»";
        }

        private void DelNote_Load(object sender, EventArgs e)
        {
            if (edit)
            {
                label1.Text = $"Заметка отредактирована {date}";
            }
            else
            {
                label1.Text = $"Заметка создана {date}";
            }
            richTextBox1.Text = text;
            textBox1.Text = header;
            comboBox1.Text = color;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(richTextBox1.Text) && !string.IsNullOrWhiteSpace(richTextBox1.Text) &&
                    !string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text))//если строка не пустая
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        string query = "INSERT INTO [Notes] (Header, Text, Date, Color, Edit)VALUES(@Header, @Text, @Date, @Color, @Edit)";

                        SQLiteCommand command = new SQLiteCommand(query, connection);

                        command.Parameters.AddWithValue("Header", textBox1.Text);

                        command.Parameters.AddWithValue("Text", richTextBox1.Text);

                        command.Parameters.AddWithValue("Date", Convert.ToDateTime(date));

                        command.Parameters.AddWithValue("Color", comboBox1.Text);

                        command.Parameters.AddWithValue("Edit", true);

                        command.ExecuteNonQuery();

                        query = "DELETE FROM [DeletedNotes] WHERE [Id]=@Id";

                        command = new SQLiteCommand(query, connection);

                        command.Parameters.AddWithValue("Id", numberID);

                        command.ExecuteNonQuery();

                        refreshNote.Start();

                        MessageBox.Show("Заметка успешно восстановлена!");

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

                    string query = "DELETE FROM [DeletedNotes] WHERE [Id]=@Id";

                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    command.Parameters.AddWithValue("Id", numberID);

                    command.ExecuteNonQuery();

                    MessageBox.Show("Заметка успешно удалена!");

                    refreshNote.Start();

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
