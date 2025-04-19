namespace LSTY.Sdtd.ServerAdmin.Shared.Exceptions
{
    public class JsonRpcCallException : Exception
    {
        public JsonRpcCallException(string message) : base(message)
        {
        }
        public JsonRpcCallException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
