using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace RadioStart.WheatherGadgetConfigurator
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
            Application.ThreadException += new ThreadExceptionEventHandler(AppError.UnhandledThreadExceptionHandler);
            Application.Run(new Form1());
        }
    }
}
