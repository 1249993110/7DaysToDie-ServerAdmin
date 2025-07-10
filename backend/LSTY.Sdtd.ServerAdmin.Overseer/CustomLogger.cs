using LSTY.Sdtd.ServerAdmin.Shared.Constants;
using System.Diagnostics;
using UnityEngine;

namespace LSTY.Sdtd.ServerAdmin.Overseer
{
    internal static class CustomLogger
    {
        internal const string Prefix = $"[{Common.CompanyName}] ";

        private enum LogType
        {
            Error,
            Warning,
            Log
        }

        [Conditional("DEBUG")]
        private static void WriteToConsole(string message, LogType logType)
        {
            string logTypeString = logType switch
            {
                LogType.Error => "ERR",
                LogType.Warning => "WRN",
                LogType.Log => "INF",
                _ => "INF"
            };
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {logTypeString}] " + message);
        }

        [Conditional("RELEASE")]
        private static void WriteToSdtdServer(string message, LogType logType)
        {
            switch (logType)
            {
                case LogType.Error:
                    Log.Error(message);
                    break;
                case LogType.Warning:
                    Log.Warning(message);
                    break;
                case LogType.Log:
                default:
                    Log.Out(message);
                    break;
            }
            SdtdConsole.Instance.Output(message);
        }

        public static void Error(string message)
        {
            message = Prefix + message;

            WriteToConsole(message, LogType.Error);
            WriteToSdtdServer(message, LogType.Error);
        }

        public static void Error(string message, params object[] args)
        {
            message = Prefix + message;
            message = string.Format(message, args);

            WriteToConsole(string.Format(message, args), LogType.Error);
            WriteToSdtdServer(message, LogType.Error);
        }

        public static void Error(Exception exception, string message)
        {
            message = Prefix + message + Environment.NewLine + exception;

            WriteToConsole(message, LogType.Error);
            WriteToSdtdServer(message, LogType.Error);
        }

        public static void Error(Exception exception, string message, params object[] args)
        {
            message = Prefix + message + Environment.NewLine + exception;
            message = string.Format(message, args);

            WriteToConsole(message, LogType.Error);
            WriteToSdtdServer(message, LogType.Error);
        }

        public static void Info(string message)
        {
            message = Prefix + message;

            WriteToConsole(message, LogType.Log);
            WriteToSdtdServer(message, LogType.Log);
        }

        public static void Info(string message, params object[] args)
        {
            message = Prefix + message;
            message = string.Format(message, args);

            WriteToConsole(message, LogType.Log);
            WriteToSdtdServer(message, LogType.Log);
        }

        public static void Warn(string message)
        {
            message = Prefix + message;

            WriteToConsole(message, LogType.Warning);
            WriteToSdtdServer(message, LogType.Warning);
        }

        public static void Warn(string message, params object[] args)
        {
            message = Prefix + message;
            message = string.Format(message, args);

            WriteToConsole(message, LogType.Warning);
            WriteToSdtdServer(message, LogType.Warning);
        }

        public static void Warn(Exception exception, string message)
        {
            message = Prefix + message + Environment.NewLine + exception;

            WriteToConsole(message, LogType.Warning);
            WriteToSdtdServer(message, LogType.Warning);
        }

        public static void Warn(Exception exception, string message, params object[] args)
        {
            message = Prefix + message + Environment.NewLine + exception;
            message = string.Format(message, args);

            WriteToConsole(message, LogType.Warning);
            WriteToSdtdServer(message, LogType.Warning);
        }
    }
}