using LSTY.Sdtd.ServerAdmin.Overseer;

namespace LSTY.Sdtd.ServerAdmin.Mock
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press 'exit' to stop the process.");

            var modMain = new ModMain();

            var mod = new Mod()
            {
                Name = nameof(Mock),
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
