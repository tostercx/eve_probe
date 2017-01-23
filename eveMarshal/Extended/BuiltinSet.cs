using System;
using System.IO;
using eveMarshal;
using System.Text;

namespace eveMarshal.Extended
{
    class BuiltinSet : PyObjectEx
    {
        public PyList values { get; private set; }

        private static PyRep createHeader(PyList list)
        {
            PyTuple tuple = new PyTuple();
            tuple.Items.Add(new PyToken("__builtin__.set"));
            PyTuple tuple1 = new PyTuple();
            tuple.Items.Add(tuple1);
            if (list == null)
            {
                tuple1.Items.Add(new PyList());
            }
            else
            {
                tuple1.Items.Add(list);
            }
            return tuple;
        }

        public BuiltinSet(PyList list)
            : base(false, createHeader(list))
        {
            values = list;
            if(values == null)
            {
                throw new InvalidDataException("BuiltinSet: expected PyList, got nullptr.");
            }
        }

        public override string dump(string prefix)
        {
            string pfx1 = prefix + PrettyPrinter.Spacer;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("[BuiltinSet]" + PrettyPrinter.PrintRawData(this));
            foreach (var item in values)
            {
                PrettyPrinter.Print(builder, pfx1, item);
            }
            if(values.Items.Count == 0)
            {
                builder.AppendLine(pfx1 + "<Empty List>");
            }
            return builder.ToString();
        }

    }
}
