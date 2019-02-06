using GsmComm.GsmCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USSDMonitor.Classes
{
    public class GSMConnection
    {
        public static int baudRate = 115200;
        public static int timeout = 1000;
        public static String port = "COM1";
        public static GsmCommMain gsmComm = new GsmCommMain(port, baudRate, timeout);
        public static IProtocol protocol;
    }
}
