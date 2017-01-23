using System.IO;

namespace eveMarshal
{

    public class PyNone : PyRep
    {
        
        public PyNone()
            : base(PyObjectType.None)
        {
            
        }

        public override void Decode(Unmarshal context, MarshalOpcode op, BinaryReader source)
        {
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            output.WriteOpcode(MarshalOpcode.None);
        }

        public override string dump(string prefix)
        {
            return "[PyNone]";
        }
    }

}