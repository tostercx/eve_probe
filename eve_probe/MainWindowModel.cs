using PropertyChanged;
using System.Collections.ObjectModel;

namespace eve_probe
{
    [ImplementPropertyChanged]
    public class MainWindowModel
    {
        public ObservableCollection<Packet> packets { get; set; }

        public string rawHex { get; set; }
        public string cryptedHex { get; set; }
        public string objectText { get; set; }

        public string injectorHex { get; set; }
    }
}
