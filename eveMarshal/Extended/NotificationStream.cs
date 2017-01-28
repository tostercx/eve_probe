using System.IO;
using System.Text;
using System.Web;

namespace eveMarshal.Extended
{
    public class NotificationStream : ExtendedObject
    {
        private long zeroOne;
        public PyRep notice { get; private set; }

        public NotificationStream(PyRep payload)
        {
            PyTuple sub = getZeroSubStream(payload as PyTuple, out zeroOne) as PyTuple;
            PyRep args = sub;
            if (sub != null)
            {
                if (zeroOne == 0)
                {
                    args = getZeroOne(sub);
                }
            }
            if (args == null)
            {
                throw new InvalidDataException("NotificationStream: null args.");
            }
            notice = args;
        }

        public override string dump(string prefix)
        {
            string pfx1 = prefix + PrettyPrinter.Spacer;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("[NotificationStream " + (zeroOne == 0 ? "Tuple01" : "SubStream") + "]");
            PrettyPrinter.Print(builder, pfx1, notice);
            return builder.ToString();
        }

        public override string dumpJSON()
        {
            string ret = "{\"type\":" + HttpUtility.JavaScriptStringEncode(this.GetType().Name, true) +
                ",\"zeroOne\":" + zeroOne;
            if (notice != null)
                ret += ",\"notice\":" + notice.dumpJSON();
            return ret + "}";
        }
    }
}
