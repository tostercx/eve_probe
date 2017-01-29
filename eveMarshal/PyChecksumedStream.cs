using System.IO;
using System.Text;

namespace eveMarshal
{

    public class PyChecksumedStream : PyRep
    {
        public uint Checksum { get; private set; }
        public PyRep Data { get; private set; }

        public PyChecksumedStream(PyRep data)
            : base(PyObjectType.ChecksumedStream)
        {
            Data = data;
        }

        public PyChecksumedStream()
            : base(PyObjectType.ChecksumedStream)
        {
            
        }

        public override void Decode(Unmarshal context, MarshalOpcode op, BinaryReader source)
        {
            Checksum = source.ReadUInt32();
            Data = context.ReadObject(source);
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            output.WriteOpcode(MarshalOpcode.ChecksumedStream);
            var ms = new MemoryStream();
            var tmp = new BinaryWriter(ms);
            Data.Encode(tmp);
            var data = ms.ToArray();
            Checksum = Adler32.Checksum(data);
            output.Write(Checksum);
            output.Write(data);
        }

        public override string dump(string prefix)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("[PyChecksumedStream Checksum: " + Checksum + "]");
            PrettyPrinter.Print(builder, prefix + PrettyPrinter.Spacer, Data);
            return builder.ToString();
        }

        public override string dumpJSON()
        {
            string ret = "{\"type\":" + HttpUtility.JavaScriptStringEncode(this.GetType().Name, true) + ",\"checksum\":" + Checksum;
            if (Data != null)
                ret += ",\"arguments\":" + Data.dumpJSON();
            return ret + "}";
        }
    }

}