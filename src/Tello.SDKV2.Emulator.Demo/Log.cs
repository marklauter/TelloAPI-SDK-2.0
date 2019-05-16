using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Tello.Simulator.SDKV2.Demo
{
    public static class Log
    {
        private static readonly object _gate = new object();

        public static void WriteLine(string msg,
            ConsoleColor consoleColor = ConsoleColor.White,
            bool addSeperator = true,
            [CallerFilePath]string callerPath = null,
            [CallerMemberName]string callerMemberName = null,
            [CallerLineNumber]int callerLineNumber = 0)
        {
            var fileName = System.IO.Path.GetFileName(callerPath);
            var output = $"{DateTime.Now.ToString("HH:mm:ss zzzz")}: {fileName}::{callerMemberName} [{callerLineNumber}] - {msg}";

            lock (_gate)
            {
                Console.ForegroundColor = consoleColor;
                Console.WriteLine(output);
                Debug.WriteLine(output);

                if (addSeperator)
                {
                    Console.WriteLine("--------------------------------");
                    Debug.WriteLine("--------------------------------");
                }

                Console.ResetColor();
            }
        }
    }
}
