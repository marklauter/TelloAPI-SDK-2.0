namespace Tello.App.MvvM
{
    public interface IUserNotifier
    {
        void Error(string message, string title = "error");
        int Input(string message, string title = "user input required");
    }
}
