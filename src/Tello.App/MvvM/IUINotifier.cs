namespace Tello.App.MvvM
{
    public interface IUINotifier
    {
        void Error(string message, string title = "error");
        int Input(string message, string title = "user input required");
    }
}
