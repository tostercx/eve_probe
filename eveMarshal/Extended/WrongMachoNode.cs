using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace eveMarshal.Extended
{
    class WrongMachoNode : ExtendedObject
    {
        public Int64 correctNode;
        /*
        * [PyObjectEx Normal]
        *   Header:
        *     [PyTuple 3 items]
        *       [PyToken carbon.common.script.net.machoNetExceptions.WrongMachoNode]
        *       [PyTuple 0 items]
        *       [PyDict 1 kvp]
        *         Key:[PyString "payload"]
        *         ==Value:[PyInt correctNode]
        *   List:
        *   Dictionary:
        */
        public WrongMachoNode(PyDict obj)
        {
            if(obj == null)
            {
                throw new InvalidDataException("WrongMachoNode: null dictionary.");
            }
            if(!obj.Contains("payload"))
            {
                throw new InvalidDataException("WrongMachoNode: Could not find key 'payload'.");
            }
            if(obj.Dictionary.Count > 1)
            {
                throw new InvalidDataException("WrongMachoNode: Too many values in dictionary.");
            }
            PyRep value = obj.Get("payload");
            correctNode = value.IntValue;
        }

        public override string dump(string prefix)
        {
            return "[WrongMachoNode: correct node = " + correctNode + "]";
        }
    }
}
