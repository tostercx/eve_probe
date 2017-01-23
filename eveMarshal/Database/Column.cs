namespace eveMarshal.Database
{

    public class Column
    {
        public string Name { get; private set; }
        public FieldType Type { get; private set; }
        public string Token { get; private set; }

        public Column(string name, FieldType type)
        {
            Name = name;
            Type = type;
            Token = "";
        }
        public Column(string name, string token)
        {
            Name = name;
            Type = FieldType.Token;
            Token = token;
        }
    }

}