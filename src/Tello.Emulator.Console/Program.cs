using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Tello.Emulator.SDKV2;

namespace Tello.EmulatorConsole
{
    internal class Log : ILog
    {
        public void Write(string message)
        {
            Console.Write(message);
            Debug.Write(message);
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Tello SDK V2.0 Emulator");

            byte[] videoData = null;
            var videoFile = new FileStream("./AppData/tello.video", FileMode.Open);
            try
            {
                videoData = new byte[videoFile.Length];
                videoFile.Read(videoData, 0, videoData.Length);
            }
            finally
            {
                videoFile.Close();
            }

            Sample[] sampleDefs = null;
            var sampleDefsFile = new FileStream("./AppData/tello.samples.json", FileMode.Open);
            try
            {
                var sampleDefsData = new byte[sampleDefsFile.Length];
                sampleDefsFile.Read(sampleDefsData, 0, sampleDefsData.Length);
                var sampleDefsJson = Encoding.UTF8.GetString(sampleDefsData);
                sampleDefs = JsonConvert.DeserializeObject<Sample[]>(sampleDefsJson);
            }
            finally
            {
                sampleDefsFile.Close();
            }

            var drone = new Drone(new Log(), videoData, sampleDefs);
            drone.PowerOn();

            Console.WriteLine("press any key to quit");
            var key = Console.ReadKey();

            drone.PowerOff();
        }
    }
}
