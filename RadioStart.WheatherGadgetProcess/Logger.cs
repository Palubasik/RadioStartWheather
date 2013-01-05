using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RadioStart.WheatherGadgetProcess
{
    class Logger
    {
        private static object _lock = new object();
        public static void WriteEntry(string data)
        {
            try{
            lock (_lock) 
            {
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"log.txt"),String.Format("[{0}] - {1}",DateTime.Now,data));
            }
            }catch{}
        }
    }
}
