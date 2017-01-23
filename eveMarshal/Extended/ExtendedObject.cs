using System;
using System.IO;

namespace eveMarshal.Extended
{
    public abstract class ExtendedObject : PyRep
    {
        public ExtendedObject() : base(PyObjectType.Extended)
        {
        }

        public override void Decode(Unmarshal context, MarshalOpcode op, BinaryReader source)
        {
            throw new InvalidOperationException("Function Not Implemented.");
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            throw new InvalidOperationException("Function Not Implemented.");
        }

        /*
        * Helper functions.
        */

        /*
        * [PyTuple 2]
        *   [PyInt 0]
        *   [PyTuple 2]
        *     [PyInt 1]
        *     response
        */
        public static PyRep getZeroOne(PyTuple payload)
        {
            if (payload.Items.Count != 2)
            {
                throw new InvalidDataException("zeroOne: Invalid tuple size expected 2 got" + payload.Items.Count);
            }
            if (!payload.Items[0].isIntNumber)
            {
                throw new InvalidDataException("zeroOne: Invalid integer object got " + payload.Items[0].Type);
            }
            long zero = payload.Items[0].IntValue;
            if (zero != 0)
            {
                throw new InvalidDataException("zeroOne: Invalid integer value expected 0 got " + zero);
            }
            PyRep temp = payload.Items[1];
            PyTuple tuple = temp as PyTuple;
            if (tuple == null)
            {
                throw new InvalidDataException("zeroOne: Invalid tuple got " + ((temp == null) ? "nullptr" : temp.Type.ToString()));
            }
            if (tuple.Items.Count != 2)
            {
                throw new InvalidDataException("zeroOne: Invalid tuple size expected 2 got" + tuple.Items.Count);
            }
            if (!tuple.Items[0].isIntNumber)
            {
                throw new InvalidDataException("zeroOne: Invalid integer object got " + tuple.Items[0].Type);
            }
            long one = tuple.Items[0].IntValue;
            if (one != 1)
            {
                throw new InvalidDataException("zeroOne: Invalid integer value expected 1 got " + one);
            }
            return tuple.Items[1];
        }
        /*
        * [PyTuple 1]
        *   [PyTuple 2]
        *     [PyInt zeroOne]
        *     [PySubStream]
        *       response
        */
        public static PyRep getZeroSubStream(PyTuple payload, out long zeroOne)
        {
            if (payload.Items.Count != 1)
            {
                throw new InvalidDataException("zeroSubstream: Invalid tuple size expected 1 got" + payload.Items.Count);
            }
            PyRep temp = payload.Items[0];
            PyTuple tuple = temp as PyTuple;
            if (tuple == null)
            {
                throw new InvalidDataException("zeroSubstream: Invalid tuple got " + ((temp == null) ? "nullptr" : temp.Type.ToString()));
            }
            if (tuple.Items.Count != 2)
            {
                throw new InvalidDataException("zeroSubstream: Invalid tuple size expected 2 got" + tuple.Items.Count);
            }
            if (!tuple.Items[0].isIntNumber)
            {
                throw new InvalidDataException("zeroSubstream: Invalid integer object got " + tuple.Items[0].Type);
            }
            zeroOne = tuple.Items[0].IntValue;
            if (zeroOne != 0 && zeroOne != 1)
            {
                throw new InvalidDataException("zeroSubstream: Invalid integer value expected 0 got " + zeroOne);
            }
            if (!(tuple.Items[1] is PySubStream))
            {
                throw new InvalidDataException("zeroSubstream: No PySubStreeam.");
            }
            PySubStream sub = tuple.Items[1] as PySubStream;
            if (sub != null)
            {
                return sub.Data;
            }
            return null;
        }

    }
}
