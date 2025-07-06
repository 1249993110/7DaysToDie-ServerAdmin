using LSTY.Sdtd.ServerAdmin.Shared.Constants;

namespace LSTY.Sdtd.ServerAdmin.Overseer
{
    internal static class CustomLogger
    {
        internal const string Prefix = $"[{Common.CompanyName}] ";

        private static readonly bool _isDebug = System.Diagnostics.Debugger.IsAttached;

        private static void WriteToConsole(string message)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] " + message);
        }

        public static void Error(string message)
        {
            message = Prefix + message;

            if (_isDebug)
            {
                WriteToConsole(message);
            }
            else
            {
                Log.Error(message);
                SdtdConsole.Instance.Output(message);
            }
        }

        public static void Error(string message, params object[] args)
        {
            message = Prefix + message;

            if (_isDebug)
            {
                WriteToConsole(string.Format(message, args));
            }
            else
            {
                Log.Error(message, args);
                SdtdConsole.Instance.Output(message, args);
            }
        }

        public static void Error(Exception exception, string message)
        {
            message = Prefix + message + Environment.NewLine + exception;

            if (_isDebug)
            {
                WriteToConsole(message);
            }
            else
            {
                Log.Error(message);
                SdtdConsole.Instance.Output(message);
            }
        }

        public static void Error(Exception exception, string message, params object[] args)
        {
            message = Prefix + message + Environment.NewLine + exception;

            if (_isDebug)
            {
                WriteToConsole(string.Format(message, args));
            }
            else
            {
                Log.Error(message, args);
                SdtdConsole.Instance.Output(message, args);
            }
        }

        public static void Info(string message)
        {
            message = Prefix + message;

            if (_isDebug)
            {
                WriteToConsole(message);
            }
            else
            {
                Log.Out(message);
                SdtdConsole.Instance.Output(message);
            }
        }

        public static void Info(string message, params object[] args)
        {
            message = Prefix + message;

            if (_isDebug)
            {
                WriteToConsole(string.Format(message, args));
            }
            else
            {
                Log.Out(message, args);
                SdtdConsole.Instance.Output(message, args);
            }
        }

        public static void Warn(string message)
        {
            message = Prefix + message;

            if(_isDebug)
            {
                WriteToConsole(message);
            }
            else
            {
                Log.Warning(message);
                SdtdConsole.Instance.Output(message);
            }
        }

        public static void Warn(string message, params object[] args)
        {
            message = Prefix + message;

            if(_isDebug)
            {
                WriteToConsole(string.Format(message, args));
            }
            else
            {
                Log.Warning(message, args);
                SdtdConsole.Instance.Output(message, args);
            }
        }

        public static void Warn(Exception exception, string message)
        {
            message = Prefix + message + Environment.NewLine + exception;

            if(_isDebug)
            {
                WriteToConsole(message);
            }
            else
            {
                Log.Warning(message);
                SdtdConsole.Instance.Output(message);
            }
        }

        public static void Warn(Exception exception, string message, params object[] args)
        {
            message = Prefix + message + Environment.NewLine + exception;

            if(_isDebug)
            {
                WriteToConsole(string.Format(message, args));
            }
            else
            {
                Log.Warning(message, args);
                SdtdConsole.Instance.Output(message, args);
            }
        }
    }
}