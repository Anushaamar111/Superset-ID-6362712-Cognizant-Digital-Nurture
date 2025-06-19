using System;


public class Logger
{
    private static Logger _instance;
    
    private Logger()
    {
        Console.WriteLine("Logger initialized");
    }

    public static Logger GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Logger();
        }
        return _instance;
    }

    public void Log(string message)
    {
        Console.WriteLine($"[Log] {message}");
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Logger logger1 = Logger.GetInstance();
        logger1.Log("Logging from logger1");

        Logger logger2 = Logger.GetInstance();
        logger2.Log("Logging from logger2");

        Console.WriteLine(Object.ReferenceEquals(logger1, logger2)
            ? "Both loggers are the same instance (Singleton works)"
            : "Different instances (Singleton failed)");
    }
}
