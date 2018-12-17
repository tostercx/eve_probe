using System;

namespace eve_probe
{
    static class Log
    {
        public static MainWindowModel vm { get; set; } = null;

        public static void log(string txt, bool timestamp = true)
        {
            if (vm != null) vm.logText += (timestamp ? "[" + DateTime.Now.ToString("HH:mm:ss") + "] " : "") + txt + "\n";
        }
    }
}
