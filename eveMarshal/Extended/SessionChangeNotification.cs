using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace eveMarshal.Extended
{
    public class SessionChangeNotification : ExtendedObject
    {
        Int32 sessionID;
        Int32 clueless;
        PyDict Changes;
        List<Int32> nodesOfInterest = new List<Int32>();

        public SessionChangeNotification(PyTuple payload)
        {
            if (payload == null)
            {
                throw new InvalidDataException("SessionChangeNotification: null payload.");
            }
            PyTuple tup = payload as PyTuple;
            if (tup.Items.Count != 3)
            {
                throw new InvalidDataException("SessionChangeNotification: PyTuple length expected 3 got " + tup.Items.Count);
            }
            if (!(tup.Items[0].isIntNumber) || !(tup.Items[1] is PyTuple) || !(tup.Items[2] is PyList))
            {
                throw new InvalidDataException("SessionChangeNotification: Structure PyIntegerVar, PyTuple, PyList got " +
                    tup.Items[0].Type.ToString() + ", " + tup.Items[1].Type.ToString() + ", " + tup.Items[2].Type.ToString()
                    );
            }
            sessionID = (Int32)tup.Items[0].IntValue;
            PyList list = tup.Items[2] as PyList;
            foreach(var node in list.Items)
            {
                if(node == null)
                {
                    throw new InvalidDataException("SessionChangeNotification: List null entry.");
                }
                nodesOfInterest.Add((Int32)node.IntValue);
            }
            PyTuple tup1 = tup.Items[1] as PyTuple;
            if (tup1.Items.Count != 2)
            {
                throw new InvalidDataException("SessionChangeNotification: PyTuple length expected 2 got " + tup.Items.Count);
            }
            if (!(tup1.Items[0] is PyInt) || !(tup1.Items[1] is PyDict))
            {
                throw new InvalidDataException("SessionChangeNotification: Structure PyInt, PyDict got " +
                    tup.Items[0].Type.ToString() + ", " + tup.Items[1].Type.ToString()
                    );
            }
            Changes = tup1.Items[1] as PyDict;
            clueless = (Int32)tup1.Items[0].IntValue;
        }

        public override string dump(string prefix)
        {
            string pfx1 = prefix + PrettyPrinter.Spacer;
            string pfx2 = pfx1 + PrettyPrinter.Spacer;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("[SessionChangeNotification]");
            builder.AppendLine(pfx1 + "sessionID=" + sessionID);
            builder.AppendLine(pfx1 + "clueless=" + clueless);
            builder.AppendLine(pfx1 + "Nodes:");
            foreach(var node in nodesOfInterest)
            {
                builder.AppendLine(pfx2 + node);
            }
            builder.AppendLine(pfx1 + "Changes");
            if (Changes == null)
            {
                builder.AppendLine(pfx2 + "<nullptr>");
            }
            else {
                PrettyPrinter.Print(builder, pfx2, Changes);
            }
            return builder.ToString();
        }
    }
}
