namespace LSTY.Sdtd.ServerAdmin.Shared.Exceptions
{
    public class RpcCallException : Exception
    {
        public RpcCallException(string message) : base(message)
        {
        }
        public RpcCallException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
