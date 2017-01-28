using System.IO;
using System.Web;

namespace eveMarshal
{
    
    public class PyBool : PyRep
    {
        public bool Value { get; set; }

        public PyBool() : base(PyObjectType.Bool)
        {
        }

        public PyBool(bool val) : this()
        {
            Value = val;
        }

        public override void Decode(Unmarshal context, MarshalOpcode op, BinaryReader source)
        {
            switch (op)
            {
                case MarshalOpcode.BoolTrue:
                    Value = true;
                    break;

                case MarshalOpcode.BoolFalse:
                    Value = false;
                    break;
            }
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            switch (Value)
            {
                case true:
                    output.WriteOpcode(MarshalOpcode.BoolTrue);
                    break;

                case false:
                    output.WriteOpcode(MarshalOpcode.BoolFalse);
                    break;
            }
        }

        public override string ToString()
        {
            return "<" + Value + ">";
        }
        public override string dump(string prefix)
        {
            return "[PyBool " + Value + "]";
        }

        public override string dumpJSON()
        {
            string ret = "{\"type\":" + HttpUtility.JavaScriptStringEncode(this.GetType().Name, true) + ",\"value\":" + Value.ToString().ToLower();
            return ret + "}";
        }
    }

}