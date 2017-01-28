using PropertyChanged;
using System.Collections.ObjectModel;

namespace eve_probe
{
    [ImplementPropertyChanged]
    public class MainWindowModel
    {
        public ObservableCollection<Packet> packets { get; set; }

        public string injectorHex { get; set; }
        public bool copyEnabled { get; set; } = false;

        public string pauseText { get; set; } = "Pause";
        public bool isPaused { get; set; } = false;
    }
}
