using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace eveMarshal.Extended
{
    class ErrorResponse : ExtendedObject
    {
        public long causingMessageType = 0;
        public long errorCode = 0;
        public PyRep errorPayload = null;

        /*
        * [PyTuple 3 items]
        *   [PyInt causingMessageType]
        *   [PyInt ErrorCode]
        *   [PyTuple 1 items]
        *     [PySubStream 87 bytes]
        *       errorPayload
        * create with:
        *    throw PyException(errorPayload)
        */
        public ErrorResponse(PyTuple payload)
        {
            if (payload == null)
            {
                throw new InvalidDataException("ErrorResponse: null payload.");
            }
            if (payload.Items.Count != 3)
            {
                throw new InvalidDataException("ErrorResponse: Invalid tuple size expected 3 got" + payload.Items.Count);
            }
            causingMessageType = payload.Items[0].IntValue;
            errorCode = payload.Items[1].IntValue;
            PyTuple payloadTuple = payload.Items[2] as PyTuple;
            if(payloadTuple == null)
            {
                throw new InvalidDataException("ErrorResponse: Invalid type expected PyTuple got " + payload.Items[2].Type);
            }
            if (payloadTuple.Items.Count != 1)
            {
                throw new InvalidDataException("ErrorResponse: Invalid tuple size expected 1 got" + payload.Items.Count);
            }
            PySubStream sub = payloadTuple.Items[0] as PySubStream;
            if (sub == null)
            {
                throw new InvalidDataException("ErrorResponse: Invalid PySubStreeam.");
            }
            errorPayload = sub.Data;
        }

        public override string dump(string prefix)
        {
            string pfx1 = prefix + PrettyPrinter.Spacer;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("ErrorResponse: msgType=" + causingMessageType + " code=" + errorCode);
            PrettyPrinter.Print(builder, pfx1, errorPayload);
            return builder.ToString();
        }

    }
}
