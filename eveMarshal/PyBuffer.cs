using System;
using System.IO;
using System.Text;
using System.Web;

namespace eveMarshal
{

    public class PyBuffer : PyRep
    {
        public byte[] Data { get; private set; }

        public PyBuffer()
            : base(PyObjectType.Buffer)
        {
            Data = null;
        }

        public PyBuffer(byte[] data)
            : base(PyObjectType.Buffer)
        {
            Data = data;
        }

        public PyBuffer(string data)
            : base(PyObjectType.Buffer)
        {
            Data = Encoding.ASCII.GetBytes(data);
        }

        public override void Decode(Unmarshal context, MarshalOpcode op, BinaryReader source)
        {
            var size = source.ReadSizeEx();
            Data = source.ReadBytes((int)size);
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            output.WriteOpcode(MarshalOpcode.Buffer);
            output.WriteSizeEx(Data.Length);
            output.Write(Data);
        }

        public override string ToString()
        {
            return "<" + BitConverter.ToString(Data) + ">";
        }

        public override string dump(string prefix)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(prefix + "[PyBuffer " + Data.Length + " bytes]" + PrettyPrinter.PrintRawData(this));
            if(Data[0] == Unmarshal.HeaderByte || Data[0] == Unmarshal.ZlibMarker)
            {
                string pfx1 = prefix + PrettyPrinter.Spacer;
                Unmarshal un = new Unmarshal();
                PyRep rep = un.Process(Data);
                if(rep != null)
                {
                    if(Data[0] == Unmarshal.ZlibMarker)
                    {
                        builder.AppendLine("<compressed-data>");
                    }
                    builder.AppendLine(pfx1 + rep.dump(pfx1));
                }
            }
            return builder.ToString();
        }

        public override string dumpJSON()
        {
            string ret = "{\"type\":" + HttpUtility.JavaScriptStringEncode(this.GetType().Name, true) +
                ",\"name\":" + HttpUtility.JavaScriptStringEncode(Convert.ToBase64String(Data), true);
            return ret + "}";
        }

    }

}