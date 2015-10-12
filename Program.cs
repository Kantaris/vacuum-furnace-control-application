using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FurnaceController
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainGUI mgui = new MainGUI();
            Application.Run(mgui);
        }
    }
}
