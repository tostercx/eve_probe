using eveMarshal.Extended;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace eveMarshal
{

    public class Unmarshal
    {
        public const byte SaveMask = 0x40;
        public const byte UnknownMask = 0x80;
        public const byte HeaderByte = 0x7E;

        // not a real magic since zlib just doesn't include one..
        public const byte ZlibMarker = 0x78;
        public const byte OpcodeMask = 0x3F;

        public bool DebugMode { get; set; }

        public Dictionary<int, int> SavedElementsMap { get; private set; }
        public PyRep[] SavedElements { get; private set; }

        private int _currentSaveIndex;

        public bool analizeInput = true;
        public StringBuilder unknown = new StringBuilder();

        public PyRep Process(byte[] data)
        {
            if (data == null)
                return null;
            if (data[0] == ZlibMarker)
                data = Zlib.Decompress(data);
            return Process(new BinaryReader(new MemoryStream(data), Encoding.ASCII));
        }

        private PyRep Process(BinaryReader reader)
        {
            var magic = reader.ReadByte();
            if (magic != HeaderByte)
            {
                throw new InvalidDataException("Invalid magic, expected: " + HeaderByte + " read: " + magic);
            }
            var saveCount = reader.ReadUInt32();

            if (saveCount > 0)
            {
                var currentPos = reader.BaseStream.Position;
                reader.BaseStream.Seek(-saveCount * 4, SeekOrigin.End);
                SavedElementsMap = new Dictionary<int, int>((int)saveCount);
                for (int i = 0; i < saveCount; i++)
                {
                    var index = reader.ReadInt32();
                    if (index < 0)
                        throw new InvalidDataException("Bogus map data in marshal stream");
                    SavedElementsMap.Add(i, index);
                }
                SavedElements = new PyRep[saveCount];
                reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);
            }

            return ReadObject(reader);
        }

        private PyRep CreateAndDecode<T>(BinaryReader reader, MarshalOpcode op) where T : PyRep, new()
        {
            // -1 for the opcode
            var ret = new T { RawOffset = reader.BaseStream.Position - 1 };
            ret.Decode(this, op, reader);
            if (PyRep.EnableInspection)
            {
                var postOffset = reader.BaseStream.Position;
                reader.BaseStream.Seek(ret.RawOffset, SeekOrigin.Begin);
                ret.RawSource = reader.ReadBytes((int)(postOffset - ret.RawOffset));
            }

            return ret;
        }

        public PyRep ReadObject(BinaryReader reader)
        {
            var header = reader.ReadByte();
            //bool flagUnknown = (header & UnknownMask) > 0;
            bool flagSave = (header & SaveMask) > 0;
            var opcode = (MarshalOpcode)(header & OpcodeMask);
            int saveIndex = 0;
            if (flagSave)
            {
                // Get save index now.
                // If there are nested saves the indexes will be wrong if we wait.
                saveIndex = SavedElementsMap[_currentSaveIndex++];
            }
            PyRep ret;
            //Console.WriteLine("OPCODE: "+opcode);
            switch (opcode)
            {
                case MarshalOpcode.SubStruct:
                    ret = CreateAndDecode<PySubStruct>(reader, opcode);
                    break;

                case MarshalOpcode.BoolFalse:
                case MarshalOpcode.BoolTrue:
                    ret = CreateAndDecode<PyBool>(reader, opcode);
                    break;

                case MarshalOpcode.None:
                    ret = CreateAndDecode<PyNone>(reader, opcode);
                    break;
                case MarshalOpcode.Token:
                    ret = CreateAndDecode<PyToken>(reader, opcode);
                    break;
                case MarshalOpcode.Real:
                case MarshalOpcode.RealZero:
                    ret = CreateAndDecode<PyFloat>(reader, opcode);
                    break;
                case MarshalOpcode.IntegerLongLong:
                    ret = CreateAndDecode<PyLongLong>(reader, opcode);
                    break;
                case MarshalOpcode.IntegerSignedShort:
                case MarshalOpcode.IntegerByte:
                case MarshalOpcode.IntegerMinusOne:
                case MarshalOpcode.IntegerOne:
                case MarshalOpcode.IntegerZero:
                case MarshalOpcode.IntegerLong:
                    ret = CreateAndDecode<PyInt>(reader, opcode);
                    break;
                case MarshalOpcode.IntegerVar:
                    ret = CreateAndDecode<PyIntegerVar>(reader, opcode);
                    break;
                case MarshalOpcode.Buffer:
                    ret = CreateAndDecode<PyBuffer>(reader, opcode);
                    break;
                case MarshalOpcode.StringEmpty:
                case MarshalOpcode.StringChar:
                case MarshalOpcode.StringShort:
                case MarshalOpcode.StringTable:
                case MarshalOpcode.StringLong:
                case MarshalOpcode.WStringEmpty:
                case MarshalOpcode.WStringUCS2:
                case MarshalOpcode.WStringUCS2Char:
                case MarshalOpcode.WStringUTF8:
                    ret = CreateAndDecode<PyString>(reader, opcode);
                    break;
                case MarshalOpcode.Tuple:
                case MarshalOpcode.TupleOne:
                case MarshalOpcode.TupleTwo:
                case MarshalOpcode.TupleEmpty:
                    ret = CreateAndDecode<PyTuple>(reader, opcode);
                    break;
                case MarshalOpcode.List:
                case MarshalOpcode.ListOne:
                case MarshalOpcode.ListEmpty:
                    ret = CreateAndDecode<PyList>(reader, opcode);
                    break;
                case MarshalOpcode.Dict:
                    ret = CreateAndDecode<PyDict>(reader, opcode);
                    break;
                case MarshalOpcode.Object:
                    ret = CreateAndDecode<PyObject>(reader, opcode);
                    break;
                case MarshalOpcode.ChecksumedStream:
                    ret = CreateAndDecode<PyChecksumedStream>(reader, opcode);
                    break;
                case MarshalOpcode.SubStream:
                    ret = CreateAndDecode<PySubStream>(reader, opcode);
                    break;
                case MarshalOpcode.SavedStreamElement:
                    uint index = reader.ReadSizeEx();
                    ret = SavedElements[index - 1];
                    break;
                case MarshalOpcode.ObjectEx1:
                case MarshalOpcode.ObjectEx2:
                    ret = CreateAndDecode<PyObjectEx>(reader, opcode);
                    break;
                case MarshalOpcode.PackedRow:
                    ret = CreateAndDecode<PyPackedRow>(reader, opcode);
                    break;
                default:
                    throw new InvalidDataException("Failed to marshal " + opcode);
            }

            if (flagSave)
            {
                if (saveIndex == 0)
                {
                    // This only seams to occure in GPSTransport packets when the server shuts down.
                    saveIndex = 1;
                }
                if (saveIndex > 0)
                {
                    SavedElements[saveIndex - 1] = ret;
                }
            }

            if (DebugMode)
            {
                Console.WriteLine("Offset: " + ret.RawOffset + " Length: " + ret.RawSource.Length + " Opcode: " + opcode + " Type: " + ret.Type + " Result: " + ret);
                Console.WriteLine(Utility.HexDump(ret.RawSource));
                Console.ReadLine();
            }

            if (analizeInput)
            {
                return analyse(ret);
            }
            return ret;
        }

        public static T Process<T>(byte[] data) where T : class
        {
            var un = new Unmarshal();
            return un.Process(data) as T;
        }

        private PyRep analyse(PyRep obj)
        {
            try
            {
                if (obj is PyObjectEx)
                {
                    bool usedList, usedDict;
                    PyRep res = obj;
                    PyObjectEx ex = obj as PyObjectEx;
                    if (!ex.IsType2)
                    {
                        res = analyseType1(ex, out usedList, out usedDict);
                    }
                    else
                    {
                        res = analyseType2(ex, out usedList, out usedDict);
                    }
                    if (res != obj)
                    {
                        if (!usedList)
                        {
                            if (ex.List != null && ex.List.Count > 0)
                            {
                                unknown.AppendLine("Unused List item in " + res.GetType());
                            }
                        }
                        if (!usedDict)
                        {
                            if (ex.Dictionary != null && ex.Dictionary.Count > 0)
                            {
                                unknown.AppendLine("Unused dictionary item in " + res.GetType());
                            }
                        }
                    }
                    return res;
                }
            }
            catch (InvalidDataException)
            {
                return obj;
            }
            return obj;
        }

        private PyRep analyseType1(PyObjectEx obj, out bool usedList, out bool usedDict)
        {
            usedDict = false;
            usedList = false;
            // Type1
            PyTuple headerTuple = obj.Header as PyTuple;
            if (headerTuple != null && headerTuple.Items.Count > 1)
            {
                int headerCount = headerTuple.Items.Count;
                PyToken token = headerTuple.Items[0] as PyToken;
                if (token != null)
                {
                    PyTuple tuple1 = null;
                    int tuple1Count = 0;
                    if (headerCount > 1)
                    {
                        tuple1 = headerTuple.Items[1] as PyTuple;
                        if (tuple1 != null)
                        {
                            if (headerCount == 3 && token.Token == "eveexceptions.UserError")
                            {
                                return new UserError(tuple1, headerTuple.Items[2] as PyDict);
                            }
                            tuple1Count = tuple1.Items.Count;
                            if (tuple1Count == 0)
                            {
                                if (headerCount == 3 && token.Token == "carbon.common.script.net.machoNetExceptions.WrongMachoNode")
                                {
                                        return new WrongMachoNode(headerTuple.Items[2] as PyDict);
                                }
                            }
                            if (tuple1Count == 1)
                            {
                                if (token.Token == "blue.DBRowDescriptor")
                                {
                                    return new DBRowDescriptor(headerTuple);
                                }
                                PyRep item1 = tuple1.Items[0];
                                if (item1 != null)
                                {
                                    if (token.Token == "__builtin__.set")
                                    {
                                        return new BuiltinSet(item1 as PyList);
                                    }
                                    if (headerCount == 2 && token.Token == "collections.defaultdict")
                                    {
                                            PyToken tupleToken = item1 as PyToken;
                                            if (tupleToken.Token == "__builtin__.set")
                                            {
                                                usedDict = true;
                                                return new DefaultDict(obj.Dictionary);
                                            }
                                    }
                                    if (token.Token == "carbon.common.script.net.objectCaching.CacheOK")
                                    {
                                        if (item1.StringValue == "CacheOK")
                                        {
                                            return new CacheOK();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (token.Token == "carbon.common.script.net.GPSExceptions.GPSTransportClosed")
                    {
                        return obj;
                    }
                    unknown.AppendLine("Unknown or malformed token: " + token.Token);
                }
            }
            return obj;
        }

        private PyRep analyseType2(PyObjectEx obj, out bool usedList, out bool usedDict)
        {
            usedDict = false;
            usedList = false;
            // type 2
            PyTuple headerTuple = obj.Header as PyTuple;
            if (headerTuple != null && headerTuple.Items.Count > 1)
            {
                int headerCount = headerTuple.Items.Count;
                PyDict dict = headerTuple.Items[1] as PyDict;
                PyToken token = null;
                PyTuple tokenTuple = headerTuple.Items[0] as PyTuple;
                if (tokenTuple != null && tokenTuple.Items.Count == 1)
                {
                    token = tokenTuple.Items[0] as PyToken;
                }
                if (token != null)
                {
                    if(headerCount != 2)
                    {
                        unknown.AppendLine("PyObjectEx Type2: headerCount=" + headerCount + " token: " + token.Token);
                    }
                    if (headerCount == 2 && token.Token == "carbon.common.script.sys.crowset.CRowset")
                    {
                        usedList = true;
                        if (dict.Dictionary.Count > 1)
                        {
                            unknown.AppendLine("PyObjectEx Type2: Extra parameters in dict for CRowset");
                        }
                        return new CRowSet(dict, obj.List);
                    }
                    if (headerCount == 2 && token.Token == "carbon.common.script.sys.crowset.CIndexedRowset")
                    {
                        usedDict = true;
                        if(dict.Dictionary.Count > 2)
                        {
                            unknown.AppendLine("PyObjectEx Type2: Extra parameters in dict for CIndexedRowset");
                        }
                        return new CIndexedRowset(dict, obj.Dictionary);
                    }
                    if (token.Token == "eve.common.script.dogma.effect.BrainEffect")
                    {
                        return obj;
                    }
                    if (token.Token == "industry.job.Location")
                    {
                        return obj;
                    }
                    if (token.Token == "eve.common.script.sys.rowset.RowDict")
                    {
                        return obj;
                    }
                    if (token.Token == "carbon.common.script.sys.crowset.CFilterRowset")
                    {
                        return obj;
                    }
                    if (token.Token == "eve.common.script.sys.rowset.RowList")
                    {
                        return obj;
                    }
                    if (token.Token == "eve.common.script.util.pagedCollection.PagedResultSet")
                    {
                        return obj;
                    }
                    if (token.Token == "shipskins.storage.LicensedSkin")
                    {
                        return obj;
                    }
                    if (token.Token == "seasons.common.challenge.Challenge")
                    {
                        return obj;
                    }
                    unknown.AppendLine("Unknown Token type 2: " + token.Token);
                }
            }
            return obj;
        }
    }

    }