namespace eveMarshal
{

    public enum PyObjectType
    {
        Tuple,
        String,
        Bool,
        None,
        Token,
        Float,
        Long,
        Int,
        SignedShort,
        Byte,
        Buffer,
        List,
        Dict,
        ObjectData,
        SubStream,
        ChecksumedStream,
        IntegerVar,
        ObjectEx,
        PackedRow,
        SubStruct,
        RawData,

        Extended,
        Address,
        Packet
    }

    public enum PyAddressType
    {
        Node = 1,
        Client = 2,
        Broadcast = 4,
        Any = 8
    }

}