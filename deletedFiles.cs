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
    public partial class deletedFiles : Form
    {
        public deletedFiles()
        {
            InitializeComponent();

            tableLayoutPanel1.BackgroundImage = new Bitmap(@"delBg.png");
            tableLayoutPanel2.BackgroundImage = new Bitmap(@"delBg.png");
            //this.BackgroundImageLayout = ImageLayout.Stretch;

            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel2.AutoScroll = true;
            flowLayoutPanel1.BackColor = Color.Transparent;
            flowLayoutPanel2.BackColor = Color.Transparent;
            button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button1.BackColor = System.Drawing.Color.Transparent;
            button1.FlatAppearance.BorderSize = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == "Заметки")
            {
                flowLayoutPanel1.Controls.Clear();
                getCountNotes();
                printNotes();
            }
            else if (comboBox1.SelectedItem == "Напоминания")
            {
                flowLayoutPanel1.Controls.Clear();
                getCountReminding();
                printReminding();
            }
        }

        public void getCountNotes()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM [DeletedNotes]";

                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    command.CommandType = CommandType.Text;

                    delCountNote = Convert.ToInt32(command.ExecuteScalar());
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void getCountReminding()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM [DeletedReminders]";

                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    command.CommandType = CommandType.Text;

                    delCountRem = Convert.ToInt32(command.ExecuteScalar());
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void printNotes()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM [DeletedNotes]";

                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    using (SQLiteDataReader sqlReader = command.ExecuteReader())
                    {
                        int i = 0;

                        while (sqlReader.Read())
                        {
                            nameHeader[i] = Convert.ToString(sqlReader["Header"]);

                            numId[i] = Convert.ToInt32(sqlReader["Id"]);

                            colorBut[i] = Convert.ToString(sqlReader["Color"]);

                            i++;
                        }
                    }

                    for (int i = 0; i < delCountNote; i++)
                    {
                        string nameBut = numId[i] + "_IDbutton";

                        arrBut[i] = new Button();
                        arrBut[i].Name = nameBut;

                        switch (colorBut[i])
                        {
                            case "Красный":
                                arrBut[i].BackColor = Color.FromArgb(255, 145, 145);
                                break;
                            case "Зеленый":
                                Color green = ColorTranslator.FromHtml("#D6FFDA");
                                arrBut[i].BackColor = green;
                                break;
                            case "Синий":
                                arrBut[i].BackColor = Color.FromArgb(199, 234, 255);
                                break;
                            case "Желтый":
                                Color yel = ColorTranslator.FromHtml("#FFFFD6");
                                arrBut[i].BackColor = yel;
                                break;
                            case "Розовый":
                                arrBut[i].BackColor = Color.FromArgb(255, 204, 204);
                                break;
                            default:
                                arrBut[i].BackColor = System.Drawing.Color.Transparent;
                                break;
                        }
                        arrBut[i].FlatAppearance.BorderSize = 0;
                        arrBut[i].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                        arrBut[i].Font = new Font("Comic Sans MS", 9, FontStyle.Regular);
                        arrBut[i].Height = 60;
                        arrBut[i].Width = 320;
                        arrBut[i].Text = nameHeader[i];
                        arrBut[i].Click += new EventHandler(infOutput);
                        flowLayoutPanel1.Controls.Add(arrBut[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void infOutput(object sender, EventArgs e)
        {
            numberID = Convert.ToString((sender as Button).Name);//имя нажатой кнопки

            int symbol_index = numberID.IndexOf('_');

            numberID = numberID.Substring(0, symbol_index);

            numberID = numberID.Replace(" ", "");
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM [DeletedNotes] WHERE [Id]='" + numberID + "'";

                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    using (SQLiteDataReader sqlReader = command.ExecuteReader())
                    {
                        while (sqlReader.Read())
                        {
                            text = Convert.ToString(sqlReader["Text"]);

                            header = Convert.ToString(sqlReader["Header"]);

                            color = Convert.ToString(sqlReader["Color"]);

                            date = Convert.ToString(sqlReader["Date"]);
                            
                            edit = Convert.ToBoolean(sqlReader["Edit"]);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);//1-сообщение,2-заголовок
            }

            DelNote frm = new DelNote(refNote);
            frm.ShowDialog();
        }

        private void infOutputRem(object sender, EventArgs e)
        {
            numberID = Convert.ToString((sender as Button).Name);//имя нажатой кнопки

            int symbol_index = numberID.IndexOf('_');

            numberID = numberID.Substring(0, symbol_index);

            numberID = numberID.Replace(" ", "");
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM [DeletedReminders] WHERE [Id]='" + numberID + "'";

                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    using (SQLiteDataReader sqlReader = command.ExecuteReader())
                    {
                        while (sqlReader.Read())
                        {
                            text = Convert.ToString(sqlReader["Description"]);

                            header = Convert.ToString(sqlReader["Header"]);

                            dateCreated = Convert.ToString(sqlReader["DateNow"]);

                            importance = Convert.ToString(sqlReader["Importance"]);

                            sound = Convert.ToString(sqlReader["Sound"]);

                            date = Convert.ToString(sqlReader["Date"]);

                            edit = Convert.ToBoolean(sqlReader["Edit"]);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);//1-сообщение,2-заголовок
            }

            DelRem frm = new DelRem(refRem);//, currentRem);

            frm.ShowDialog();

            //currentRem.Start();
        }

        private void refNote_Tick(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            getCountNotes();
            printNotes();

            //ТУТ НАДО ПОМЕНЯТЬ ИБО ФОРМА ПОИСКА!!!!
            for (int i = 0; i < n; i++)
            {
                flowLayoutPanel2.Controls.Clear();
            }
            n = 0;

            textBox1.Text = "";

            repeat_word = "random_text";

            refNote.Stop();
        }

        private void refRem_Tick(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            getCountReminding();
            printReminding();

            for (int i = 0; i < n; i++)
            {
                flowLayoutPanel2.Controls.Clear();
            }
            n = 0;

            textBox1.Text = "";

            repeat_word = "random_text";

            refRem.Stop();
        }

        public void printReminding()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM [DeletedReminders]";

                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    using (SQLiteDataReader sqlReader = command.ExecuteReader())
                    {
                        int i = 0;

                        while (sqlReader.Read())
                        {
                            nameHeader[i] = Convert.ToString(sqlReader["Header"]);

                            numId[i] = Convert.ToInt32(sqlReader["Id"]);

                            i++;
                        }
                    }

                    for (int i = 0; i < delCountRem; i++)
                    {
                        string nameBut = numId[i] + "_IDbutton";

                        arrBut[i] = new Button();
                        arrBut[i].Name = nameBut;

                        //arrButRem[i].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                        //НИЖНИЕ 2 ПАРАМЕТРА МОЖНО УДАЛИТЬ
                        //arrBut[i].FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
                        //arrBut[i].FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
                        //!!!!!!!!!!!!!!!

                        arrBut[i].FlatAppearance.BorderSize = 0;
                        arrBut[i].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                        arrBut[i].Font = new Font("Comic Sans MS", 9, FontStyle.Regular);
                        arrBut[i].Height = 60;
                        arrBut[i].Width = 320;
                        arrBut[i].Text = nameHeader[i];
                        arrBut[i].Click += new EventHandler(infOutputRem);
                        flowLayoutPanel1.Controls.Add(arrBut[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == "Заметки")
            {

                if (repeat_word != textBox1.Text)
                {
                    for (int i = 0; i < n; i++)
                    {
                        arrSearchBut[i].Dispose();
                    }
                    n = 0;

                    if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text))
                    {
                        try
                        {
                            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                            {

                                connection.Open();

                                string[] local_name_header = new string[100];
                                int[] local_num_id = new int[100];
                                int i = 0;

                                repeat_word = textBox1.Text;

                                string query = "SELECT * FROM [DeletedNotes] WHERE [Header]='" + textBox1.Text + "'";

                                SQLiteCommand command = new SQLiteCommand(query, connection);
                                string[] tempColorBut = new string[100];
                                using (SQLiteDataReader sqlReader = command.ExecuteReader())
                                {
                                    while (sqlReader.Read())
                                    {
                                        local_name_header[i] = Convert.ToString(sqlReader["Header"]);
                                        local_num_id[i] = Convert.ToInt32(sqlReader["Id"]);
                                        tempColorBut[i] = Convert.ToString(sqlReader["Color"]);
                                        i++;
                                        n++;
                                    }
                                }
                                for (i = 0; i < n; i++)
                                {
                                    nameButton[i] = local_num_id[i] + "_localIDbutton";
                                    arrSearchBut[i] = new Button();

                                    switch (tempColorBut[i])
                                    {
                                        case "Красный":
                                            arrSearchBut[i].BackColor = Color.FromArgb(255, 145, 145);
                                            break;
                                        case "Зеленый":
                                            Color green = ColorTranslator.FromHtml("#D6FFDA");
                                            arrSearchBut[i].BackColor = green;
                                            break;
                                        case "Синий":
                                            arrSearchBut[i].BackColor = Color.FromArgb(199, 234, 255);
                                            break;
                                        case "Желтый":
                                            Color yel = ColorTranslator.FromHtml("#FFFFD6");
                                            arrSearchBut[i].BackColor = yel;
                                            break;
                                        case "Розовый":
                                            arrSearchBut[i].BackColor = Color.FromArgb(255, 204, 204);
                                            break;
                                        default:
                                            arrSearchBut[i].BackColor = System.Drawing.Color.Transparent;
                                            break;
                                    }
                                    arrSearchBut[i].Name = nameButton[i];
                                    arrSearchBut[i].Font = new Font("Comic Sans MS", 9, FontStyle.Regular);
                                    arrSearchBut[i].Height = 50;
                                    arrSearchBut[i].Width = 180;
                                    arrSearchBut[i].FlatAppearance.BorderSize = 0;
                                    arrSearchBut[i].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                                    arrSearchBut[i].Text = local_name_header[i];
                                    arrSearchBut[i].Click += new EventHandler(infOutput);
                                    flowLayoutPanel2.Controls.Add(arrSearchBut[i]);
                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);//1-сообщение,2-заголовок
                        }

                    }
                    else
                    {
                        MessageBox.Show("Поле поиска не должно быть пустым!");
                    }
                }
            }
            else if(comboBox1.SelectedItem == "Напоминания")
            {
                if (repeat_word != textBox1.Text)
                {
                    for (int i = 0; i < n; i++)
                    {
                        arrSearchBut[i].Dispose();
                    }
                    n = 0;

                    if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text))
                    {
                        try
                        {
                            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                            {

                                connection.Open();

                                string[] local_name_header = new string[100];
                                int[] local_num_id = new int[100];
                                int i = 0;

                                repeat_word = textBox1.Text;

                                string query = "SELECT * FROM [DeletedReminders] WHERE [Header]='" + textBox1.Text + "'";

                                SQLiteCommand command = new SQLiteCommand(query, connection);

                                using (SQLiteDataReader sqlReader = command.ExecuteReader())
                                {
                                    while (sqlReader.Read())
                                    {
                                        local_name_header[i] = Convert.ToString(sqlReader["Header"]);
                                        local_num_id[i] = Convert.ToInt32(sqlReader["Id"]);
                                        i++;
                                        n++;
                                    }
                                }
                                for (i = 0; i < n; i++)
                                {
                                    nameButton[i] = local_num_id[i] + "_localIDbutton";
                                    arrSearchBut[i] = new Button();

                                    arrSearchBut[i].Name = nameButton[i];
                                    arrSearchBut[i].Font = new Font("Comic Sans MS", 9, FontStyle.Regular);
                                    arrSearchBut[i].Height = 50;
                                    arrSearchBut[i].Width = 180;
                                    arrSearchBut[i].FlatAppearance.BorderSize = 0;
                                    arrSearchBut[i].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                                    arrSearchBut[i].Text = local_name_header[i];
                                    arrSearchBut[i].Click += new EventHandler(infOutputRem);
                                    flowLayoutPanel2.Controls.Add(arrSearchBut[i]);
                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);//1-сообщение,2-заголовок
                        }

                    }
                    else
                    {
                        MessageBox.Show("Поле поиска не должно быть пустым!");
                    }
                }
            }
        }
    }
}

static class delGlobalVars
{
    public static int delCountNote = 0, delCountRem = 0;
    public static string[] nameHeader = new string[100];
    public static string[] colorBut = new string[100];
    public static int[] numId = new int[100];
    public static string numberID, text, header, color, date, dateCreated, importance, sound;
    public static bool edit = false;
    public static int ID = 0;
    public static int minID = 0;
    public static Button[] arrBut = new Button[100];
    public static Button[] arrSearchBut = new Button[100];
    public static int n = 0;
    public static string[] nameButton = new string[100]; //Переменные для поиска по форме.
    public static string repeat_word = "";
    public static string connectionString = "Data Source=Database1.db; Version=3";
}