using MessagePack;
using StreamJsonRpc;

namespace LSTY.Sdtd.ServerAdmin.Shared.Utilities
{
    public static class MessagePackFormatterHelper
    {
        /// <summary>
        ///
        /// </summary>
        public static MessagePackFormatter Create()
        {
            var options = MessagePackSerializerOptions.Standard
                    .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance)
                    .WithCompression(MessagePackCompression.Lz4BlockArray);
            var formatter = new MessagePackFormatter();
            formatter.SetMessagePackSerializerOptions(options);
            return formatter;
        }
    }
}
