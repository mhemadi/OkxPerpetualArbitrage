namespace OkxPerpetualArbitrage.Persistance.Configurations
{
    public static class SqlDataTypes
    {
        public static readonly string VeryLongString = "VARCHAR(1000)";
        public static readonly string LongString = "VARCHAR(100)";
        public static readonly string MediumString = "VARCHAR(50)";
        public static readonly string ShortString = "VARCHAR(10)";
        public static readonly string Money = "DECIMAL(14, 2)";
        public static readonly string Instrument = "DECIMAL(12, 6)";
        public static readonly string Date = "DATETIME";
    }
}
