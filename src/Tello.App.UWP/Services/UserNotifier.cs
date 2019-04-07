using System;
using System.Diagnostics;
using Tello.App.MvvM;

namespace Tello.App.UWP.Services
{
    public class UserNotifier : IUserNotifier
    {
        public void Error(string message, string title = "error")
        {
            Debug.Write($"{DateTime.Now.TimeOfDay} - {title}: {message}");
        }

        public int Input(string message, string title = "user input required")
        {
            Debug.Write($"{DateTime.Now.TimeOfDay} - {title}: {message}");
            return 0;
        }
    }
}
