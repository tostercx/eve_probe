using System.Collections.Generic;
using System.IO;
using System.Text;

namespace eveMarshal
{

    public class PyObjectEx : PyRep
    {
        private const byte PackedTerminator = 0x2D;

        public bool IsType2 { get; private set; }
        public PyRep Header { get; private set; }
        public Dictionary<PyRep, PyRep> Dictionary { get; private set; }
        public List<PyRep> List { get; private set; }

        public PyObjectEx(bool isType2, PyRep header)
            : base(PyObjectType.ObjectEx)
        {
            IsType2 = isType2;
            Header = header;
            Dictionary = new Dictionary<PyRep, PyRep>();
        }

        public PyObjectEx()
            : base(PyObjectType.ObjectEx)
        {
            
        }

        public override void Decode(Unmarshal context, MarshalOpcode op, BinaryReader source)
        {
            if (op == MarshalOpcode.ObjectEx2)
            {
                IsType2 = true;
            }

            Dictionary = new Dictionary<PyRep, PyRep>();
            List = new List<PyRep>();
            Header = context.ReadObject(source);

            while (source.BaseStream.Position < source.BaseStream.Length)
            {
                var b = source.ReadByte();
                if (b == PackedTerminator)
                    break;
                source.BaseStream.Seek(-1, SeekOrigin.Current);
                List.Add(context.ReadObject(source));
            }

            while (source.BaseStream.Position < source.BaseStream.Length)
            {
                var b = source.ReadByte();
                if (b == PackedTerminator)
                    break;
                source.BaseStream.Seek(-1, SeekOrigin.Current);
                var key = context.ReadObject(source);
                var value = context.ReadObject(source);
                Dictionary.Add(key, value);
            }
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            output.WriteOpcode(IsType2 ? MarshalOpcode.ObjectEx2 : MarshalOpcode.ObjectEx1);
            Header.Encode(output);
            // list
            output.Write(PackedTerminator);
            // dict
            output.Write(PackedTerminator);
        }

        public override string dump(string prefix)
        {
            string pfx1 = prefix + PrettyPrinter.Spacer;
            string pfx2 = pfx1 + PrettyPrinter.Spacer;
            string pfx3 = pfx2 + PrettyPrinter.Spacer + PrettyPrinter.Spacer;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("[PyObjectEx " + (IsType2 ? "Type2" : "Normal") + "]" + PrettyPrinter.PrintRawData(this));
            builder.AppendLine(pfx1 + "Header:");
            PrettyPrinter.Print(builder, pfx2, Header);
            builder.AppendLine(pfx1 + "List:");
            foreach (var item in List)
            {
                PrettyPrinter.Print(builder, pfx2, item);
            }
            builder.AppendLine(pfx1 + "Dictionary:");
            foreach (var kvp in Dictionary)
            {
                PrettyPrinter.Print(builder, pfx2 + "Key:", kvp.Key);
                builder.AppendLine(pfx2 + "==Value:" + kvp.Value.dump(pfx3).TrimEnd('\r', '\n'));
                //PrettyPrinter.Print(builder, pfx2 + "==Value:", kvp.Value);
            }
            return builder.ToString();
        }

    }

}