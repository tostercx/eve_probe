using System;
using System.IO;
using System.Text;

namespace eveMarshal
{
    public class PyAddress : PyRep
    {
        public PyAddressType addrType = PyAddressType.Any;
        public long addrID = 0;
        public long callID = 0;
        public string service = "";
        public string broadcastType = "";

        public PyAddress(PyRep args)
            : base(PyObjectType.Address)
        {
            if(args.Type != PyObjectType.ObjectData)
            {
                throw new InvalidDataException("PyAddress: Incorrect object type expected ObjectData got " + args.Type.ToString() + ".");
            }
            PyObject data = args as PyObject;
            if(data.Name != "carbon.common.script.net.machoNetAddress.MachoAddress")
            {
                throw new InvalidDataException("PyAddress: Unrecognized address name got " + data.Name + ".");
            }
            if (data.Arguments.Type != PyObjectType.Tuple)
            {
                throw new InvalidDataException("PyAddress: Address contents expected Tuple got " + data.Arguments.Type.ToString() + ".");
            }
            PyTuple tuple = data.Arguments as PyTuple;
            if(tuple.Items.Count < 3)
            {
                throw new InvalidDataException("PyAddress: Incorrect tupple size requires at least 3 got " + tuple.Items.Count + ".");
            }
            if (tuple.Items[0].Type != PyObjectType.Int)
            {
                throw new InvalidDataException("PyAddress: Address type expected Int got " + data.Arguments.Type.ToString() + ".");
            }
            addrType = (PyAddressType)tuple.Items[0].IntValue;
            if(addrType != PyAddressType.Any && tuple.Items.Count != 4)
            {
                throw new InvalidDataException("PyAddress: Incorrect tupple size requires 4 got " + tuple.Items.Count + ".");
            }
            switch (addrType)
            {
                case PyAddressType.Any:
                    service = tuple.Items[1].StringValue;
                    callID = tuple.Items[2].IntValue;
                    break;
                case PyAddressType.Node:
                    addrID = tuple.Items[1].IntValue;
                    service = tuple.Items[2].StringValue;
                    callID = tuple.Items[3].IntValue;
                    break;
                case PyAddressType.Client:
                    addrID = tuple.Items[1].IntValue;
                    callID = tuple.Items[2].IntValue;
                    service = tuple.Items[3].StringValue;
                    break;
                case PyAddressType.Broadcast:
                    service = tuple.Items[1].StringValue;
                    broadcastType = tuple.Items[3].StringValue;
                    break;
                default:
                    throw new InvalidDataException("PyAddress: Unknown address type got " + addrType + ".");
            }
            RawOffset = args.RawOffset;
            RawSource = args.RawSource;
        }

        public override string ToString()
        {
            string addr = addrType.ToString() + ":\n    callID=" + callID;
            switch(addrType)
            {
                case PyAddressType.Node:
                case PyAddressType.Client:
                    addr += "\n    Node=" + addrID + "\n    ServiceID='" + service + "'";
                    break;
                case PyAddressType.Any:
                    addr += "\n    ServiceID='" + service + "'";
                    break;
                case PyAddressType.Broadcast:
                    addr += addrType.ToString() + ":\n    Broadcast='" + service + "'\n    Broadcast Type='" + broadcastType + "'";
                    break;
                default:
                    addr = "<Unknown Address Type>";
                    break;
            }
            return addr;
        }

        public override void Decode(Unmarshal context, MarshalOpcode op, BinaryReader source)
        {
            throw new InvalidOperationException("Function Not Implemented.");
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            throw new InvalidOperationException("Function Not Implemented.");
        }

        public override string dump(string prefix)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[PyAddress " + addrType.ToString());
            string sep = ": ";
            if (addrType != PyAddressType.Broadcast)
            {
                if (callID > 0)
                {
                    builder.Append(sep + "callID=" + callID);
                    sep = ", ";
                }
                if (service != null && service.Length > 0)
                {
                    builder.Append(sep + "service='" + service + "'");
                    sep = ", ";
                }
            }
            switch (addrType)
            {
                case PyAddressType.Node:
                case PyAddressType.Client:
                    builder.Append(sep + "nodeID=" + addrID);
                    break;
                case PyAddressType.Broadcast:
                    builder.Append(sep + "broadcastType='" + broadcastType + "', idType='" + service + "'");
                    break;
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}
