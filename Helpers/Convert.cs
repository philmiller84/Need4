namespace Helpers
{
    public class Convert
    {
        public static Google.Protobuf.WellKnownTypes.Int32Value ToInt32Value(int Value)
        {
            return new Google.Protobuf.WellKnownTypes.Int32Value { Value = System.Convert.ToInt32(Value) };
        }
    }
}
