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
    public partial class mainMenu : Form
    {
        public mainMenu()
        {
            InitializeComponent();

            this.BackgroundImage = new Bitmap(@"bg1.png");

            this.BackgroundImageLayout = ImageLayout.Stretch;

            this.MaximumSize = new Size(942, 627);
            this.MinimumSize    = new Size(942, 627);

            flowLayoutPanel1.BackColor = Color.Transparent;
            flowLayoutPanel2.BackColor = Color.Transparent;
            flowLayoutPanel3.BackColor = Color.Transparent;
            flowLayoutPanel4.BackColor = Color.Transparent;
            label1.BackColor = Color.Transparent;
            label2.BackColor = Color.Transparent;
            label3.BackColor = Color.Transparent;

            Color bg = ColorTranslator.FromHtml("#fffee9");
            menuStrip1.BackColor = bg;

            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel2.AutoScroll = true;
            flowLayoutPanel4.AutoScroll = true;

            button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button1.BackColor = System.Drawing.Color.Transparent;
            button1.FlatAppearance.BorderSize = 0;

        }

        private int countDateArr = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            getCountNotes();
            printNotes();
            getCountReminding();
            printReminding();
            missingReminding();
            //refreshRemindings.Start();
        }

        public void missingReminding()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {

                    int i = 0;

                    connection.Open();

                    string query = "SELECT [Date], [Id] FROM [Reminding]";

                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    using (SQLiteDataReader sqlReader = command.ExecuteReader())
                    {
                        while (sqlReader.Read())
                        {
                            if (Convert.ToDateTime(sqlReader["Date"]) < DateTime.Now)
                            {
                                missedEvent frm = new missedEvent();

                                ID = Convert.ToInt32(sqlReader["Id"]);

                                frm.ShowDialog();

                                if (del)
                                {
                                    query = "INSERT INTO [DeletedReminders] (Header, Description, Date, DateNow, Importance, Sound, Edit)VALUES(@Header, @Description, @Date, @DateNow, @Importance, @Sound, @Edit)";

                                    command = new SQLiteCommand(query, connection);

                                    command.Parameters.AddWithValue("Description", text);

                                    command.Parameters.AddWithValue("Header", header);

                                    //date = maskedTextBox1.Text + " " + maskedTextBox2.Text;

                                    command.Parameters.AddWithValue("Date", date);

                                    command.Parameters.AddWithValue("DateNow", dateCreated);

                                    command.Parameters.AddWithValue("Importance", importance);

                                    command.Parameters.AddWithValue("Sound", sound);

                                    command.Parameters.AddWithValue("Edit", edit);

                                    command.ExecuteNonQuery();

                                    query = "DELETE FROM [Reminding] WHERE [Id]=@Id";

                                    command = new SQLiteCommand(query, connection);

                                    command.Parameters.AddWithValue("Id", ID);

                                    command.ExecuteNonQuery();

                                    del = false;

                                    MessageBox.Show("Напоминание перемещено в корзину!");

                                    refreshRemindings.Start();

                                    continue;
                                }
                                else if (edt)
                                {
                                    query = "UPDATE [Reminding] SET [Date]=@Date WHERE [Id]=@Id";

                                    command = new SQLiteCommand(query, connection);

                                    command.Parameters.AddWithValue("Id", ID);

                                    command.Parameters.AddWithValue("Date", tempDate);

                                    command.ExecuteNonQuery();

                                    refreshRemindings.Start();
                                }
                            }

                            dates[i] = (DateTime)sqlReader["Date"];

                            countDateArr++;

                            i++;
                        }
                    }
                    
                    checkArray();
                    currentRem.Start();
                    //timer2.Start(); - перенесено в начало проги
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void minElement()
        {
            minDate = dates[0];

            for (int i = 0; i < countDateArr; i++)
            {
                if (minDate > dates[i])
                {
                    minDate = dates[i];

                    if (minDate < DateTime.Now)
                    {
                        //Сделать повторный запрос к БД
                        MessageBox.Show($"Ого, вы нашли баг в программе. Запишите событие {minDate} на бумажку и сообщите о баге на почту vlad.grebenyuk.00@mail.ru");

                        //Идет подробный вывод текста и предложение установить новое событие или перенести
                        //Вместо удаления эл массива удалить запись в БД
                        dates[i] = dates[i + 1];

                        i--;

                        countDateArr--;

                    }

                }
                if (minDate < DateTime.Now)
                {
                    MessageBox.Show($"Ого, вы нашли баг в программе. Запишите событие {minDate} на бумажку и сообщите о баге на почту vlad.grebenyuk.00@mail.ru");

                    //Идет подробный вывод текста и предложение установить новое событие или перенести
                    //Вместо удаления эл массива удалить запись в БД
                    dates[i] = dates[i + 1];

                    i--;

                    countDateArr--;
                }
            }
        }

        public void checkArray()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                int i = 0;

                countDateArr = 0;

                string query = "SELECT [Date], [Id] FROM [Reminding]";

                SQLiteCommand command = new SQLiteCommand(query, connection);

                using (SQLiteDataReader sqlReader = command.ExecuteReader())
                {
                    //var minTemp = dates[0];
                    minDate = Convert.ToDateTime("11.11.3333");
                    //minDate = dates[0];

                    while (sqlReader.Read())
                    {
                        if (dates[i] != Convert.ToDateTime(sqlReader["Date"]))
                        {
                            dates[i] = Convert.ToDateTime(sqlReader["Date"]);
                        }

                        if (minDate > dates[i])
                        {
                            minDate = dates[i];

                            //minDate = minTemp;

                            minID = Convert.ToInt32(sqlReader["ID"]);
                        }

                        if(minDate == dates[i])
                        {
                            minID = Convert.ToInt32(sqlReader["ID"]);
                        }

                        i++;

                        countDateArr++;
                    }
                }
            }

            //minElement();
        }

        public void getCountNotes()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM [Notes]";

                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    command.CommandType = CommandType.Text;

                    count = Convert.ToInt32(command.ExecuteScalar());
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

                    string query = "SELECT COUNT(*) FROM [Reminding]";

                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    command.CommandType = CommandType.Text;

                    countRem = Convert.ToInt32(command.ExecuteScalar());
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

                    string query = "SELECT * FROM [Notes]";

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

                    for (int i = 0; i < count; i++)
                    {
                        string nameBut = numId[i] + "_IDbutton";

                        arrBut[i] = new Button();
                        arrBut[i].Name = nameBut;

                        switch (colorBut[i])
                        {
                            case "Красный":
                                //arrBut[i].BackColor = System.Drawing.Color.Red;
                                arrBut[i].BackColor = Color.FromArgb(255, 145, 145);
                                //Color red = ColorTranslator.FromHtml("#F9C8C8");
                                //arrBut[i].BackColor = red;
                                break;
                            case "Зеленый":
                                //arrBut[i].BackColor = System.Drawing.Color.Lime;//SpringGreen, LimeGreen
                                //arrBut[i].BackColor = Color.FromArgb(189, 255, 171);
                                Color green = ColorTranslator.FromHtml("#D6FFDA");
                                arrBut[i].BackColor = green;
                                break;
                            case "Синий":
                                //arrBut[i].BackColor = System.Drawing.Color.DeepSkyBlue;//RoyalBlue,CornflowerBlue,DeepSkyBlue
                                arrBut[i].BackColor = Color.FromArgb(199, 234, 255);
                                break;
                            case "Желтый":
                                //arrBut[i].BackColor = System.Drawing.Color.Gold;//Gold, Yellow
                                //arrBut[i].BackColor = Color.FromArgb(255, 249, 138);
                                Color yel = ColorTranslator.FromHtml("#FFFFD6");
                                arrBut[i].BackColor = yel;
                                break;
                            case "Розовый":
                                //arrBut[i].BackColor = System.Drawing.Color.Pink;
                                arrBut[i].BackColor = Color.FromArgb(255, 204, 204);
                                break;
                            default:
                                arrBut[i].BackColor = System.Drawing.Color.Transparent;
                                break;
                        }
                        arrBut[i].FlatAppearance.BorderSize = 0;
                        arrBut[i].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                        arrBut[i].Font = new Font("Comic Sans MS", 9, FontStyle.Regular);
                        arrBut[i].Height = 50;
                        arrBut[i].Width = 200;
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

                    string query = "SELECT * FROM [Notes] WHERE [Id]='" + numberID + "'";

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

            EditNote frm = new EditNote(refreshNotes);
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

                    string query = "SELECT * FROM [Reminding] WHERE [Id]='" + numberID + "'";

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

            EditReminding frm = new EditReminding(refreshRemindings);//, currentRem);

            frm.ShowDialog();
        }

        private void новаяЗаметкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewNote frm = new NewNote(refreshNotes);
            frm.ShowDialog();
        }

        public void refreshNotes_Tick(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            getCountNotes();
            printNotes();

            //ТУТ НАДО ПОМЕНЯТЬ ИБО ФОРМА ПОИСКА!!!!
            for (int i = 0; i < n; i++)
            {
                flowLayoutPanel4.Controls.Clear();
            }
            n = 0;

            textBox1.Text = "";

            repeat_word = "random_text";

            refreshNotes.Stop();
        }

        public void printReminding()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM [Reminding]";

                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    using (SQLiteDataReader sqlReader = command.ExecuteReader())
                    {
                        int i = 0;

                        while (sqlReader.Read())
                        {
                            nameHeaderRem[i] = Convert.ToString(sqlReader["Header"]);

                            numIdRem[i] = Convert.ToInt32(sqlReader["Id"]);

                            i++;
                        }
                    }

                    for (int i = 0; i < countRem; i++)
                    {
                        string nameBut = numIdRem[i] + "_IDbutton";

                        arrButRem[i] = new Button();
                        arrButRem[i].Name = nameBut;

                        arrButRem[i].FlatAppearance.BorderSize = 0;
                        arrButRem[i].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                        arrButRem[i].Font = new Font("Comic Sans MS", 9, FontStyle.Regular);
                        arrButRem[i].Height = 50;
                        arrButRem[i].Width = 200;
                        arrButRem[i].Text = nameHeaderRem[i];
                        arrButRem[i].Click += new EventHandler(infOutputRem);
                        flowLayoutPanel2.Controls.Add(arrButRem[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void новоеНапоминаниеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewReminding frm = new NewReminding(refreshRemindings);

            frm.ShowDialog();
        }

        private void refreshRemindings_Tick(object sender, EventArgs e)
        {
            flowLayoutPanel2.Controls.Clear();
            getCountReminding();
            printReminding();
            //missingReminding();
            checkArray();

            refreshRemindings.Stop();
        }

        private void currentRem_Tick(object sender, EventArgs e)
        {
            int symbol_index = Convert.ToString(minDate).IndexOf(' ');

            string DMY = Convert.ToString(minDate).Substring(0, symbol_index);

            int tempCount = Convert.ToString(minDate).Count();

            string HMS = Convert.ToString(minDate).Substring(symbol_index, tempCount - symbol_index);

            DMY = DMY.Replace(" ", "");

            HMS = HMS.Replace(" ", "");
            
            if ((DateTime.Now.Hour >= 0) && (DateTime.Now.Hour <= 9))
            {
                HMS = $"0{HMS}";;
            }

            if ((DMY == DateTime.Now.Day.ToString("00") + "." + DateTime.Now.Month.ToString("00") + "." + DateTime.Now.Year.ToString("0000")) && (HMS == DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00")))
            {
                try
                {
                    currentEvent frm = new currentEvent(refreshRemindings);

                    frm.ShowDialog();

                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        if (del)
                        {
                            string query = "INSERT INTO [DeletedReminders] (Header, Description, Date, DateNow, Importance, Sound, Edit)VALUES(@Header, @Description, @Date, @DateNow, @Importance, @Sound, @Edit)";

                            SQLiteCommand command = new SQLiteCommand(query, connection);

                            command.Parameters.AddWithValue("Description", text);

                            command.Parameters.AddWithValue("Header", header);

                            //date = maskedTextBox1.Text + " " + maskedTextBox2.Text;

                            command.Parameters.AddWithValue("Date", date);

                            command.Parameters.AddWithValue("DateNow", dateCreated);

                            command.Parameters.AddWithValue("Importance", importance);

                            command.Parameters.AddWithValue("Sound", sound);

                            command.Parameters.AddWithValue("Edit", edit);

                            command.ExecuteNonQuery();

                            query = "DELETE FROM [Reminding] WHERE [Id]=@Id";

                            command = new SQLiteCommand(query, connection);

                            command.Parameters.AddWithValue("Id", minID);

                            command.ExecuteNonQuery();

                            del = false;

                            MessageBox.Show("Напоминание перемещено в корзину!");

                            checkArray();
                        }
                        else if (edt)
                        {
                            string query = "UPDATE [Reminding] SET [Date]=@Date WHERE [Id]=@Id";

                            SQLiteCommand command = new SQLiteCommand(query, connection);

                            command.Parameters.AddWithValue("Id", minID);

                            command.Parameters.AddWithValue("Date", tempDate);

                            command.ExecuteNonQuery();

                            checkArray();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
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

                            string query = "SELECT * FROM [Notes] WHERE [Header]='" + textBox1.Text + "'";

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
                                arrSearchBut[i].Width = 120;
                                arrSearchBut[i].FlatAppearance.BorderSize = 0;
                                arrSearchBut[i].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                                arrSearchBut[i].Text = local_name_header[i];
                                arrSearchBut[i].Click += new EventHandler(infOutput);
                                flowLayoutPanel4.Controls.Add(arrSearchBut[i]);
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

        private void flowLayoutPanel3_DoubleClick(object sender, EventArgs e)
        {
            deletedFiles frm = new deletedFiles();
            frm.ShowDialog();
            refreshNotes.Start();
            refreshRemindings.Start();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutProgram frm = new aboutProgram();
            frm.ShowDialog();
        }

        private void разработчикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutAuthor frm = new aboutAuthor();
            frm.ShowDialog();
        }
    }
}


static class GlobalVars//сделать динамический массив
{
    public static string[] nameHeader = new string[100];
    public static string[] colorBut = new string[100];
    public static int[] numId = new int[100];

    public static string numberID;
    public static string text;
    public static string header;
    public static string color;
    public static string date;
    public static string dateCreated;
    public static string importance;
    public static string sound;
    public static bool edit = false;

    //сделать count определяемым до определения массива кнопок
    public static Button[] arrBut = new Button[100];
    public static Button[] arrButRem = new Button[100];
    public static string[] nameHeaderRem = new string[100];
    public static int[] numIdRem = new int[100];

    public static Button[] arrSearchBut = new Button[100];
    public static int n = 0;
    public static string[] nameButton = new string[100]; //Переменные для поиска по форме.
    public static string repeat_word = "";

    //-------------------------------------------------------------------

    public static int count;
    public static int countRem;
    public static DateTime minDate;
    public static DateTime[] dates = new DateTime[100];
    public static bool del = false;
    public static bool edt = false;
    public static bool cls = false;
    public static int ID = 0;
    public static int minID = 0;
    public static DateTime tempDate;
    public static string connectionString = "Data Source=Database1.db; Version=3";
}