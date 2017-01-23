using System;
using System.IO;
using System.Text;

namespace eveMarshal.Extended
{
    class PyCallStream : ExtendedObject
    {
        long zeroOne = 0;
        Int64 remoteObject = 0;
        string remoteObjectStr = "";
        string method = "";
        PyTuple arg_tuple = null;
        PyDict arg_dict = null;
        ExtendedObject extended = null;

        /*
        * [PyTuple 1]
        *   [PyTuple 2]
        *     [PyInt zeroOne]
        *     [PySubStream]
        *       [PyInt/PyString remoteObject/remoteObjectStr]
        *       [PyString method]
        *       [PyTuple arg_tuple]
        *       [PyDict arg_dict]
        */
        public PyCallStream(PyTuple payload)
        {
            long zeroOne;
            PyTuple call = getZeroSubStream(payload as PyTuple, out zeroOne) as PyTuple;
            if(call == null)
            {
                throw new InvalidDataException("PyCallStream: null payload.");
            }
            if(call.Items.Count != 4)
            {
                throw new InvalidDataException("PyCallStream: Invalid call tuple size, expected 4 got " + call.Items.Count);
            }
            if(call.Items[0].isIntNumber)
            {
                remoteObject = call.Items[0].IntValue;
            }
            else if(call.Items[0] is PyString)
            {
                remoteObjectStr = call.Items[0].StringValue;
            }
            else
            {
                throw new InvalidDataException("PyCallStream: Invalid remote object type, expected PyInt or PyString got" + call.Items[0].Type);
            }
            if (!(call.Items[1] is PyString))
            {
                throw new InvalidDataException("PyCallStream: Invalid method name, expected PyString got" + call.Items[1].Type);
            }
            method = call.Items[1].StringValue;
            arg_tuple = call.Items[2] as PyTuple;
            arg_dict = call.Items[3] as PyDict;
            if(arg_tuple == null)
            {
                throw new InvalidDataException("PyCallStream: Invalid argument tuple, expected PyTuple got" + call.Items[2].Type);
            }
            if(arg_dict == null && (call.Items[3] != null && !(call.Items[3] is PyNone)))
            {
                throw new InvalidDataException("PyCallStream: Invalid argument dict, expected PyDict or PyNone got" + call.Items[3].Type);
            }
            if(method == "MachoBindObject")
            {
                extended = new CallMachoBindObject(arg_tuple);
                arg_tuple = null;
            }
        }

        public override string dump(string prefix)
        {
            string pfx1 = prefix + PrettyPrinter.Spacer;
            string pfx2 = pfx1 + PrettyPrinter.Spacer;
            string pfx3 = pfx2 + PrettyPrinter.Spacer;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("[PyCallStream" + zeroOne +"]");
            if(remoteObject == 0)
            {
                builder.AppendLine(pfx1 + "remoteObject: '" + remoteObjectStr + "'");
            }
            else
            {
                builder.AppendLine(pfx1 + "remoteObject: " + remoteObject);
            }
            builder.AppendLine(pfx1 + "Method: '" + method + "'");
            builder.AppendLine(pfx1 + "Arguments:");
            if (extended != null)
            {
                PrettyPrinter.Print(builder, pfx2, extended);
            }
            else
            {
                PrettyPrinter.Print(builder, pfx2, arg_tuple);
            }
            if(arg_dict == null)
            {
                builder.AppendLine(pfx1 + "Named Arguments: None");
            }
            else
            {
                builder.AppendLine(pfx1 + "Named Arguments:");
                PrettyPrinter.Print(builder, pfx2, arg_dict);
            }
            return builder.ToString();
        }
    }
}
