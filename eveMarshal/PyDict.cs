using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace eveMarshal
{

    public class PyDict : PyRep
    {
        public Dictionary<PyRep, PyRep> Dictionary { get; private set; }
        
        public PyDict()
            : base(PyObjectType.Dict)
        {
            Dictionary = new Dictionary<PyRep, PyRep>();
        }
        
        public PyDict(Dictionary<PyRep, PyRep> dict)
            : base(PyObjectType.Dict)
        {
            Dictionary = dict;
        }

        public PyRep Get(string key)
        {
            var keyObject =
                Dictionary.Keys.Where(k => k.Type == PyObjectType.String && (k as PyString).Value == key).FirstOrDefault();
            return keyObject == null ? null : Dictionary[keyObject];
        }

        public void Set(string key, PyRep value)
        {
            var keyObject = Dictionary.Count > 0 ? Dictionary.Keys.Where(k => k.Type == PyObjectType.String && (k as PyString).Value == key).FirstOrDefault() : null;
            if (keyObject != null)
                Dictionary[keyObject] = value;
            else
                Dictionary.Add(new PyString(key), value);
        }

        public bool Contains(string key)
        {
            return Dictionary.Keys.Any(k => k.Type == PyObjectType.String && (k as PyString).Value == key);
        }

        public override void Decode(Unmarshal context, MarshalOpcode op, BinaryReader source)
        {
            var entries = source.ReadSizeEx();
            Dictionary = new Dictionary<PyRep, PyRep>((int)entries);
            for (uint i = 0; i < entries; i++)
            {
                var value = context.ReadObject(source);
                var key = context.ReadObject(source);
                Dictionary.Add(key, value);
            }
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            output.WriteOpcode(MarshalOpcode.Dict);
            output.WriteSizeEx(Dictionary.Count);
            foreach (var pair in Dictionary)
            {
                pair.Value.Encode(output);
                pair.Key.Encode(output);
            }
        }

        public PyRep this[PyRep key]
        {
            get
            {
                return Dictionary[key];
            }
            set
            {
                Dictionary[key] = value;
            }
        }

        public override PyRep this[string key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Set(key, value);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder("<\n");
            foreach (var pair in Dictionary)
                sb.AppendLine("\t" + pair.Key + " " + pair.Value);
            sb.Append(">");
            return sb.ToString();
        }

        public override string dump(string prefix)
        {
            StringBuilder builder = new StringBuilder();
            string pfx1 = prefix + PrettyPrinter.Spacer;
            string pfx2 = pfx1 + PrettyPrinter.Spacer + PrettyPrinter.Spacer;
            builder.AppendLine("[PyDict " + Dictionary.Count + " kvp]" + PrettyPrinter.PrintRawData(this));
            foreach (var kvp in Dictionary)
            {
                PrettyPrinter.Print(builder, pfx1 + "Key:", kvp.Key);
                if (kvp.Value == null)
                {
                    builder.AppendLine(pfx1 + "==Value: <nullptr>");
                }
                else
                {
                    builder.AppendLine(pfx1 + "==Value:" + kvp.Value.dump(pfx2).TrimEnd('\r', '\n'));
                }
            }
            return builder.ToString();
        }

        public override string dumpJSON()
        {
            string ret = "{\"type\":" + HttpUtility.JavaScriptStringEncode(this.GetType().Name, true) + ",\"items\":{";
            bool first = true;
            if (Items != null)
            {
                foreach (var item in Dictionary)
                {
                    ret += first ? "" : "," + HttpUtility.JavaScriptStringEncode(item.Key.StringValue, true) + ":";
                    if (item != null)
                        ret += item.Value.dumpJSON();
                    else
                        ret += "null";
                    first = false;
                }
            }
            return ret + "}}";
        }

    }

}