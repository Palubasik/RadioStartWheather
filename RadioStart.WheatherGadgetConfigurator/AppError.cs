using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace RadioStart.WheatherGadgetConfigurator
{
  public class AppError 
    {
      private static bool isError = false;
      public static bool IsError { get { return isError; } }
        public static void UnhandledThreadExceptionHandler(object sender, ThreadExceptionEventArgs e)
        {
            HandleUnhandledException(e.Exception);
        }

        public static void HandleUnhandledException(Exception e)
        {

            ErrorForm er = new ErrorForm();
            er.ErrorText = String.Format("Message: {0}\n\nStack: {1}\n\nInnerException: {2}\n\nType: {3}", e.Message, e.StackTrace,e.InnerException != null ? e.InnerException.Message : "Empty",
                e.GetType().ToString());
            isError = true;
            if (er.ShowDialog() == System.Windows.Forms.DialogResult.No) 
            {
                Application.Exit();
            }
        }
    }


}
