using System;
using System.Collections.Generic;
using System.IO;
using eveMarshal.Database;
using System.Linq;
using System.Text;
using eveMarshal.Extended;

namespace eveMarshal
{

    public class PyPackedRow : PyRep
    {
        public DBRowDescriptor Descriptor { get; private set; }
        public byte[] RawData { get; private set; }

        public PyRep[] values { get; private set; }

        public PyPackedRow()
            : base(PyObjectType.PackedRow)
        {
            
        }

        public PyRep Get(string key)
        {
            //var col = Descriptor.Columns.Where(c => c.Name == key).FirstOrDefault();
            //return col == null ? null : col.Value;
            if (Descriptor == null)
            {
                return null;
            }
            int index = Descriptor.Columns.FindIndex(x => x.Name == key);
            return values[index];
        }

        public override void Decode(Unmarshal context, MarshalOpcode op, BinaryReader source)
        {
            PyRep obj = context.ReadObject(source);
            Descriptor = obj as DBRowDescriptor;
            if(Descriptor == null)
            {
                throw new InvalidDataException("PyPackedRow: Header must be DBRowDescriptor got " + obj.Type);
            }
            RawData = LoadZeroCompressed(source);

            if (!ParseRowData(context, source))
                 throw new InvalidDataException("Could not fully unpack PackedRow, stream integrity is broken");
        }

        private bool ParseRowData(Unmarshal context, BinaryReader source)
        {
            if (Descriptor == null)
            {
                return false;
            }

            values = new PyRep[Descriptor.Columns.Count];
            var sizeList = Descriptor.Columns.OrderByDescending(c => FieldTypeHelper.GetTypeBits(c.Type));
            var sizeSum = sizeList.Sum(c => FieldTypeHelper.GetTypeBits(c.Type));
            // align
            sizeSum = (sizeSum + 7) >> 3;
            var rawStream = new MemoryStream();
            // fill up
            rawStream.Write(RawData, 0, RawData.Length);
            for (int i = 0; i < (sizeSum - RawData.Length); i++)
            {
                rawStream.WriteByte(0);
            }
            rawStream.Seek(0, SeekOrigin.Begin);
            var reader = new BinaryReader(rawStream);

            int bitOffset = 0;
            foreach (var column in sizeList)
            {
                PyRep value = null;
                switch (column.Type)
                {
                    case FieldType.I8:
                    case FieldType.UI8:
                    case FieldType.CY:
                    case FieldType.FileTime:
                        value = new PyLongLong(reader.ReadInt64());
                        break;

                    case FieldType.I4:
                    case FieldType.UI4:
                        value = new PyInt(reader.ReadInt32());
                        break;

                    case FieldType.I2:
                    case FieldType.UI2:
                        value = new PyInt(reader.ReadInt16());
                        break;

                    case FieldType.I1:
                    case FieldType.UI1:
                        value = new PyInt(reader.ReadByte());
                        break;

                    case FieldType.R8:
                        value = new PyFloat(reader.ReadDouble());
                        break;

                    case FieldType.R4:
                        value = new PyFloat(reader.ReadSingle());
                        break;

                    case FieldType.Bytes:
                    case FieldType.Str:
                    case FieldType.WStr:
                        value = context.ReadObject(source);
                        break;

                    case FieldType.Bool:
                        {
                            if (7 < bitOffset)
                            {
                                bitOffset = 0;
                                reader.ReadByte();
                            }

                            var b = reader.ReadByte();
                            reader.BaseStream.Seek(-1, SeekOrigin.Current);
                            value = new PyInt((b >> bitOffset++) & 0x01);
                            break;
                        }

                    case FieldType.Token:
                        value = new PyToken(column.Token);
                        break;

                    default:
                        throw new Exception("No support for " + column.Type);
                }
                int index = Descriptor.Columns.FindIndex(x => x.Name == column.Name);
                values[index] = value;
            }

            return true;
        }

        private static byte[] LoadZeroCompressed(BinaryReader reader)
        {
            var ret = new List<byte>();
            uint packedLen = reader.ReadSizeEx();
            long max = reader.BaseStream.Position + packedLen;
            while (reader.BaseStream.Position < max)
            {
                var opcode = new ZeroCompressOpcode(reader.ReadByte());

                if (opcode.FirstIsZero)
                {
                    for (int n = 0; n < (opcode.FirstLength+1); n++)
                        ret.Add(0x00);
                }
                else
                {
                    int bytes = (int)Math.Min(8 - opcode.FirstLength, max - reader.BaseStream.Position);
                    for (int n = 0; n < bytes; n++)
                        ret.Add(reader.ReadByte());
                }

                if (opcode.SecondIsZero)
                {
                    for (int n = 0; n < (opcode.SecondLength+1); n++)
                        ret.Add(0x00);
                }
                else
                {
                    int bytes = (int)Math.Min(8 - opcode.SecondLength, max - reader.BaseStream.Position);
                    for (int n = 0; n < bytes; n++)
                        ret.Add(reader.ReadByte());
                }
            }
            return ret.ToArray();
        }

        protected override void EncodeInternal(BinaryWriter output)
        {
            throw new NotImplementedException();
        }

        public override string dump(string prefix)
        {
            StringBuilder builder = new StringBuilder();
            string pfx1 = prefix + PrettyPrinter.Spacer;
            builder.AppendLine("[PyPackedRow " + RawData.Length + " bytes]");
            if (Descriptor.Columns != null)
            {
                foreach (var column in Descriptor.Columns)
                {
                    int index = Descriptor.Columns.FindIndex(x => x.Name == column.Name);
                    PyRep value = values[index];
                    builder.AppendLine(pfx1 + "[\"" + column.Name + "\" => " + " [" + column.Type + "] " + value + "]");
                }
            }
            else
            {
                builder.AppendLine(pfx1 + "[Columns parsing failed!]");
            }
            return builder.ToString();
        }

    }

}