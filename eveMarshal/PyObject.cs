using System.Data;
using System.IO;
using System.Text;
using System.Web;

namespace eveMarshal
{
    
    public class PyObject : PyRep
    {
        public PyObject()
            : base(PyObjectType.ObjectData)
        {
            
        }

        public PyObject(string objectName, PyRep arguments)
            : base(PyObjectType.ObjectData)
        {
            Name = objectName;
            Arguments = arguments;
        }

        public string Name { get; set; }
        public PyRep Arguments { get; set; }

        public override void Decode(Unmarshal context, MarshalOpcode op, BinaryReader source)
        {
            var nameObject = context.ReadObject(source);
            if (nameObject.Type != PyObjectType.String)
                throw new DataException("Expected PyString");
            Name = (nameObject as PyString).Value;

            Arguments = context.ReadObject(source);
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            output.WriteOpcode(MarshalOpcode.Object);
            new PyString(Name).Encode(output);
            Arguments.Encode(output);
        }

        public override string dump(string prefix)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("[PyObject Name: " + Name + "]" + PrettyPrinter.PrintRawData(this));
            PrettyPrinter.Print(builder, prefix + PrettyPrinter.Spacer, Arguments);
            return builder.ToString();
        }

        public override string dumpJSON()
        {
            string ret = "{\"type\":" + HttpUtility.JavaScriptStringEncode(this.GetType().Name, true) + ",\"name\":" + HttpUtility.JavaScriptStringEncode(Name, true);
            if (Arguments != null)
                ret += ",\"arguments\":" + Arguments.dumpJSON();
            return ret + "}";
        }
    }

}