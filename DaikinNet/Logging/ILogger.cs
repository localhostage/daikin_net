namespace DaikinNet;

public interface ILogger
{
    void Debug(string message);
    void Error(string message, Exception e);
}