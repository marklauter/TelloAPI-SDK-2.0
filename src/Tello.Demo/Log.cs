// <copyright file="Log.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Tello.Demo
{
    public static class Log
    {
        private static readonly object Gate = new object();

        public static void WriteLine(
            string msg,
            ConsoleColor consoleColor = ConsoleColor.White,
            bool addSeperator = true,
            [CallerFilePath]string callerPath = null,
            [CallerMemberName]string callerMemberName = null,
            [CallerLineNumber]int callerLineNumber = 0)
        {
            var fileName = System.IO.Path.GetFileName(callerPath);
            var output = $"{DateTime.Now.ToString("HH:mm:ss zzzz")}: {fileName}::{callerMemberName} [{callerLineNumber}] - {msg}";

            lock (Gate)
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
