using System;
using System.Text;
using System.Text.RegularExpressions;

namespace eveMarshal
{

    public static class PrettyPrinter
    {
        public const string Spacer = "    ";

        public static bool IsASCII(this string value)
        {
            // ASCII encoding replaces non-ascii with question marks, so we use UTF8 to see if multi-byte sequences are there
            return Encoding.UTF8.GetByteCount(value) == value.Length;
        }

        public static string Print(PyRep obj, int indention = 0)
        {
            var ret = new StringBuilder();
            Print(ret, "", obj);
            return ret.ToString();
        }

        public static void Print(StringBuilder builder, string prefix, PyRep obj)
        {
            if(obj == null)
            {
                builder.AppendLine(prefix + "<nullptr>");
                return;
            }
            string pfx1 = prefix + Spacer;
            string pfx2 = pfx1 + Spacer;
            builder.AppendLine(prefix + obj.dump(prefix).TrimEnd('\r', '\n'));// + PrintRawData(obj));
        }

        public static string PrintRawData(PyRep obj)
        {
            if (obj.RawSource == null)
                return "";
            return " [" + BitConverter.ToString(obj.RawSource, 0, obj.RawSource.Length > 8 ? 8 : obj.RawSource.Length) + "]";
        }

        public static string StringToHex(string str)
        {
            return BitConverter.ToString(Encoding.Default.GetBytes(str)).Replace("-", "");
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public static bool containsBinary(byte[] p)
        {
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] < (byte)32 || p[i] > (byte)126)
                    return true;
            }
            return false;
        }

    }

}
