using DaikinNet;

public class ConsoleLogger : ILogger
{
    public void Debug(string message)
    {
        Console.WriteLine($"DEBUG: {message}");
    }

    public void Error(string message, Exception e)
    {
        Console.WriteLine($"ERROR: {message}");
        Console.WriteLine($"EXCEPTION: {e.Message}");
    }
}