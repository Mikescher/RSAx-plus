// @Date : 15th July 2012
// @Author : Arpan Jati (arpan4017@yahoo.com; arpan4017@gmail.com)
// @Application : RSAx Test Application

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RSA_Algorithm
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
            Application.Run(new RSAxTestForm());
        }
    }
}
