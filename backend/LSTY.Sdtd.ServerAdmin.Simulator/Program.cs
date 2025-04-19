using LSTY.Sdtd.ServerAdmin.Overseer;
using LSTY.Sdtd.ServerAdmin.Overseer.RpcServer;
using System.Reflection;

namespace LSTY.Sdtd.ServerAdmin.Simulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press 'exit' to stop the process.");

            var modMain = new ModMain();

            var mod = new Mod()
            {
                Name = nameof(Simulator),
                Path = AppContext.BaseDirectory,
            };
            modMain.InitMod(mod, debug: true);

            //RpcServerManager.JsonRpcServer.Stop();

            //RpcServerManager.JsonRpcServer.Dispose();

            while (true)
            {
                var input = Console.ReadLine();
                if (input == "exit")
                {
                    break;
                }
            }

            Console.ReadKey();
        }
    }
}
