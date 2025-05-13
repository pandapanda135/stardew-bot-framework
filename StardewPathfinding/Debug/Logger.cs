using StardewModdingAPI;

namespace StardewPathfinding.Debug;

public class Logger
{
    private static IMonitor? Monitor;

    public static void SetMonitor(IMonitor monitor)
    {
        if (Logger.Monitor == null)
        {
            Logger.Monitor = monitor;
        }
    }

    public static void Log(string message, LogLevel level = LogLevel.Info)
    {
        Logger.Monitor?.Log(message, level);
    }
    
    public static void Trace(string message)
    {
        Logger.Monitor?.Log(message, LogLevel.Trace);
    }

    public static void Debug(string message)
    {
        Logger.Monitor?.Log(message, LogLevel.Debug);
    }

    public static void Info(string message)
    {
        Logger.Monitor?.Log(message, LogLevel.Info);
    }

    public static void Warning(string message)
    {
        Logger.Monitor?.Log(message,LogLevel.Warn);
    }

    public static void Error(string message)
    {
        Logger.Monitor?.Log(message, LogLevel.Error);
    }
}