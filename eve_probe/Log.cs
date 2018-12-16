namespace eve_probe
{
    static class Log
    {
        public static MainWindowModel vm { get; set; } = null;

        public static void log(string txt)
        {
            if (vm != null) vm.logText += txt + "\n";
        }
    }
}
