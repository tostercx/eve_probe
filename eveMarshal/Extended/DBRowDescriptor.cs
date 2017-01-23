using System;
using System.IO;
using System.Text;
using eveMarshal.Database;
using System.Collections.Generic;

namespace eveMarshal.Extended
{
    public class DBRowDescriptor : ExtendedObject
    {
        // These are constants?
        PyList keywords = null;
        public List<Column> Columns { get; private set; }

        /*
        * [PyObjectEx Type1]
        *   header:
        *     [PyToken "blue.DBRowDescriptor"]
        *     [PyTuple 1]
        *       [PyTuple columns.Count]
        *         columns as
        *           [PyTuple 2]
        *             [PyString "columnName"]
        *             [PyInt columnDBType]
        *     [PyList] (optional)
        *       keywords
        * create with: new DBRowDescriptor();
        */
        public DBRowDescriptor(PyTuple header)
                : base()
        {
            if (header == null)
            {
                throw new InvalidDataException("DBRowDescriptor: null header.");
            }
            if (header.Items.Count < 2)
            {
                throw new InvalidDataException("DBRowDescriptor: Wrong tuple size expected 2 got " + header.Items.Count);
            }
            if (header.Items.Count == 3 && header.Items[2] is PyList)
            {
                keywords = header.Items[2] as PyList;
            }
            PyTuple tuple = header.Items[1] as PyTuple;
            if (tuple == null)
            {
                throw new InvalidDataException("DBRowDescriptor: null tuple.");
            }
            if (tuple.Items.Count > 1)
            {
                throw new InvalidDataException("DBRowDescriptor: Wrong tuple size expected 1 got" + tuple.Items.Count);
            }
            PyTuple columns = tuple.Items[0] as PyTuple;
            if (columns == null)
            {
                throw new InvalidDataException("DBRowDescriptor: no columns.");
            }

            int columnCount = columns.Items.Count;
            if(keywords != null)
            {
                columnCount += keywords.Items.Count;
            }
            Columns = new List<Column>(columnCount);

            foreach (var obj in columns.Items)
            {
                PyTuple entry = obj as PyTuple;
                if (entry == null || entry.Items.Count < 2)
                {
                    continue;
                }
                PyString name = entry.Items[0] as PyString;
                if (name == null)
                {
                    continue;
                }

                Columns.Add(new Column(name.Value, (FieldType)entry.Items[1].IntValue));
            }
            if (keywords != null)
            {
                foreach (var obj in keywords.Items)
                {
                    PyTuple entry = obj as PyTuple;
                    if (entry == null || entry.Items.Count < 2)
                    {
                        continue;
                    }
                    PyString name = entry.Items[0] as PyString;
                    if (name == null)
                    {
                        continue;
                    }
                    PyToken token = entry.Items[1] as PyToken;
                    if (token == null)
                    {
                        continue;
                    }

                    Columns.Add(new Column(name.Value, token.Token));
                }
            }
        }

        public override string dump(string prefix)
        {
            string pfx1 = prefix + PrettyPrinter.Spacer;
            string pfx2 = pfx1 + PrettyPrinter.Spacer;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("[DBRowDescriptor]");
            if (Columns != null)
            {
                foreach (var column in Columns)
                {
                    if(column.Type == FieldType.Token)
                    {
                        continue;
                    }
                    int index = Columns.FindIndex(x => x.Name == column.Name);
                    builder.AppendLine(pfx1 + "[\"" + column.Name + "\" => " + " [" + column.Type + "] ]");
                }
            }
            else
            {
                builder.AppendLine(pfx1 + "[Columns parsing failed!]");
            }
            if(keywords != null)
            {
                builder.AppendLine(pfx1 + "keywords:");
                foreach (var obj in keywords.Items)
                {
                    PyTuple entry = obj as PyTuple;
                    if (entry == null || entry.Items.Count < 2)
                    {
                        builder.AppendLine(pfx2 + "<bad keyword>");
                        continue;
                    }

                    PyString name = entry.Items[0] as PyString;
                    if (name == null)
                    {
                        builder.AppendLine(pfx2 + "<bad keyword name>");
                        continue;
                    }

                    PyToken token = entry.Items[1] as PyToken;
                    if (token == null)
                    {
                        builder.AppendLine(pfx2 + "<bad keyword token>");
                        continue;
                    }
                    builder.AppendLine(pfx2 + "[\"" + name.Value + "\" => " + " '" + token.Token + "' ]");
                }
            }
            return builder.ToString();
        }

    }
}
