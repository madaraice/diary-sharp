using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursach
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            loadForm frm = new loadForm();
            DateTime endTime = DateTime.Now + TimeSpan.FromSeconds(7.9);//(7.94999);//(8-0.01);
            frm.Show();
            while (endTime > DateTime.Now)
            {
                Application.DoEvents();
            }
            frm.Close();
            frm.Dispose();
            Application.Run(new mainMenu());
        }
    }
}
