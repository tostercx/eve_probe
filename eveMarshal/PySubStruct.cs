using System.IO;
using System.Text;

namespace eveMarshal
{

    public class PySubStruct : PyRep
    {
        public PyRep Definition { get; set; }

        public PySubStruct()
            : base(PyObjectType.SubStruct)
        {
            
        }

        public override void Decode(Unmarshal context, MarshalOpcode op, BinaryReader source)
        {
            Definition = context.ReadObject(source);
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            output.WriteOpcode(MarshalOpcode.SubStruct);
            Definition.Encode(output);
        }

        public override string dump(string prefix)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("[PySubStruct]");
            PrettyPrinter.Print(builder, prefix + PrettyPrinter.Spacer, Definition);
            return builder.ToString();
        }

    }

}