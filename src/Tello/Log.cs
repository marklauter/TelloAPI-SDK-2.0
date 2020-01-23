// <copyright file="Log.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Tello
{
    public static class Log
    {
        public static void WriteLine(
            string msg,
            bool addSeperator = true,
            [CallerFilePath]string callerPath = null,
            [CallerMemberName]string callerMemberName = null,
            [CallerLineNumber]int callerLineNumber = 0)
        {
            var fileName = System.IO.Path.GetFileName(callerPath);
            var output = $"{DateTime.Now.ToString("HH:mm:ss zzzz")}: {fileName}::{callerMemberName} [{callerLineNumber}] - {msg}";
            Debug.WriteLine(output);
            if (addSeperator)
            {
                Debug.WriteLine("--------------------------------");
            }
        }
    }
}
