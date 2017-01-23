using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace eveMarshal.Extended
{
    public class CIndexedRowset : ExtendedObject
    {
        string columnName;
        DBRowDescriptor descriptor;
        Dictionary<PyRep, PyRep> rows;

        /*
        * [PyObjectEx Type2]
        *   header:
        *     [PyTuple 1]
        *       [PyToken "carbon.common.script.sys.crowset.CIndexedRowset"]
        *     [PyDict]
        *       Key=header
        *       Value=[DBRowDescriptor]
        *       key=columnName
        *       value=[PyString columnName]
        *   dict:
        *     rows
        * create with: DBResultToCIndexedRowset()
        */
        public CIndexedRowset(PyDict dict, Dictionary<PyRep, PyRep> nRows)
        {
            rows = nRows;
            if(rows == null)
            {
                rows = new Dictionary<PyRep, PyRep>();
            }
            descriptor = dict.Get("header") as DBRowDescriptor;
            if (descriptor == null)
            {
                throw new InvalidDataException("CIndexedRowSet: Invalid DBRowDescriptor.");
            }
            PyRep name = dict.Get("columnName");
            if(name == null)
            {
                throw new InvalidDataException("CIndexedRowSet: Could not find index name.");
            }
            columnName = name.StringValue;
        }

        public override string dump(string prefix)
        {
            string pfx1 = prefix + PrettyPrinter.Spacer;
            string pfx2 = pfx1 + PrettyPrinter.Spacer;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("[CIndexedRowSet]" + PrettyPrinter.PrintRawData(this));
            builder.AppendLine(pfx1 + "index: " + columnName);
            builder.AppendLine(pfx1 + descriptor.dump(pfx1).TrimEnd('\r', '\n'));
            builder.AppendLine(pfx1 + "Rows:");
            foreach (var item in rows)
            {
                PrettyPrinter.Print(builder, pfx2 + "Key:", item.Key);
                PrettyPrinter.Print(builder, pfx2, item.Value);
            }
            return builder.ToString();
        }

    }
}
