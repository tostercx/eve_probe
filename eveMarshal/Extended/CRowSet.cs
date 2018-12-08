using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace eveMarshal.Extended
{
    public class CRowSet : ExtendedObject
    {
        public DBRowDescriptor descriptor;
        public List<PyRep> rows;

        /*
        * [PyObjectEx Type2]
        *   header:
        *     [PyTuple 1]
        *       [PyToken "carbon.common.script.sys.crowset.CRowset"]
        *     [PyDict]
        *       Key=header
        *       Value=[DBRowDescriptor]
        *   list:
        *     rows
        * create with: DBResultToCRowset
        */
        public CRowSet(PyDict dict, List<PyRep> list)
        {
            rows = list;
            if(rows == null)
            {
                rows = new List<PyRep>();
            }
            descriptor = dict.Get("header") as DBRowDescriptor;
            if (descriptor == null)
            {
                throw new InvalidDataException("CRowSet: Invalid DBRowDescriptor.");
            }
        }

        public override string dump(string prefix)
        {
            string pfx1 = prefix + PrettyPrinter.Spacer;
            string pfx2 = pfx1 + PrettyPrinter.Spacer;
            string pfx3 = pfx2 + PrettyPrinter.Spacer + PrettyPrinter.Spacer;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("[CRowSet]" + PrettyPrinter.PrintRawData(this));
            builder.AppendLine(pfx1 + descriptor.dump(pfx1).TrimEnd('\r', '\n'));
            builder.AppendLine(pfx1 + "Rows:");
            foreach (var item in rows)
            {
                PrettyPrinter.Print(builder, pfx2, item);
            }
            return builder.ToString();
        }

    }
}
