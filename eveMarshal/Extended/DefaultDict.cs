using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eveMarshal.Extended
{
    public class DefaultDict : ExtendedObject
    {
        public Dictionary<PyRep, PyRep> Dictionary { get; private set; }

        /*
        * [PyObjectEx Type1]
        *   header:
        *     [PyToken "collections.defaultdict"]
        *     [PyTuple 1]
        *       [PyToken __builtin__.set]
        *   dict:
        *     Dictionary
        * create with: new DefaultDict();
        */
        public DefaultDict(Dictionary<PyRep, PyRep> dict)
        {
            Dictionary = dict;
        }

        public override string dump(string prefix)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("[DefaultDict]");
            if (Dictionary != null)
            {
                string pfx1 = prefix + PrettyPrinter.Spacer;
                string pfx2 = pfx1 + PrettyPrinter.Spacer;
                string pfx3 = pfx1 + PrettyPrinter.Spacer + PrettyPrinter.Spacer;
                builder.AppendLine(pfx1 + "Dictionary:");
                foreach (var kvp in Dictionary)
                {
                    PrettyPrinter.Print(builder, pfx2 + "Key:", kvp.Key);
                    if (kvp.Value == null)
                    {
                        builder.AppendLine(pfx2 + "==Value: <nullptr>");
                    }
                    else
                    {
                        builder.AppendLine(pfx2 + "==Value:" + kvp.Value.dump(pfx3).TrimEnd('\r', '\n'));
                    }
                }
            }
            return builder.ToString();
        }
    }
}
