using IceCoffee.Common.Extensions;
using LSTY.Sdtd.ServerAdmin.Services.Abstractions;

namespace LSTY.Sdtd.ServerAdmin.Services.Core
{
    public class FunctionGroup : IDisposable
    {
        public required Dictionary<string, IFunction> Functions { get; set; }
        public required SharedState SharedState { get; set; }
        public required ChatCommandProcessor ChatCommandProcessor { get; set; }

        public void Dispose()
        {
            foreach (var function in Functions.Values)
            {
                function.TryDispose();
            }

            SharedState.TryDispose();
            ChatCommandProcessor.TryDispose();
        }
    }
}
