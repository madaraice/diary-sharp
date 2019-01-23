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
    public partial class EditNote : Form
    {
        private Timer refreshNote;

        public EditNote(Timer timer)
        {
            refreshNote = timer;

            InitializeComponent();

            this.Text = $"Редактирование заметки «{header}»";
        }

        private void EditNote_Load(object sender, EventArgs e)
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

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(richTextBox1.Text) && !string.IsNullOrWhiteSpace(richTextBox1.Text) &&
                    !string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text))//если строка не пустая
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        string query = "UPDATE [Notes] SET [Header]=@Header, [Text]=@Text, [Date]=@Date, [Color]=@Color, [Edit]=@Edit WHERE [Id]=@Id";

                        SQLiteCommand command = new SQLiteCommand(query, connection);

                        command.Parameters.AddWithValue("Id", numberID);

                        command.Parameters.AddWithValue("Text", richTextBox1.Text);

                        command.Parameters.AddWithValue("Header", textBox1.Text);

                        command.Parameters.AddWithValue("Date", DateTime.Now);

                        command.Parameters.AddWithValue("Color", comboBox1.Text);//добавить проверку на те цвета которые в комбобоксе

                        command.Parameters.AddWithValue("Edit", true);

                        command.ExecuteNonQuery();

                        refreshNote.Start();

                        this.Text = $"Редактирование заметки «{textBox1.Text}»";

                        label1.Text = "Заметка отредактирована " + DateTime.Now;

                        MessageBox.Show("Заметка успешно отредактирована!");
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

                    string query = "INSERT INTO [DeletedNotes] (Header, Text, Date, Color, Edit)VALUES(@Header, @Text, @Date, @Color, @Edit)";

                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    command.Parameters.AddWithValue("Header", header);

                    command.Parameters.AddWithValue("Text", text);

                    command.Parameters.AddWithValue("Date", date);

                    command.Parameters.AddWithValue("Color", color);

                    command.Parameters.AddWithValue("Edit", edit);

                    command.ExecuteNonQuery();

                    query = "DELETE FROM [Notes] WHERE [Id]=@Id";

                    command = new SQLiteCommand(query, connection);

                    command.Parameters.AddWithValue("Id", numberID);

                    command.ExecuteNonQuery();

                    MessageBox.Show("Заметка перемещена в корзину!");

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
