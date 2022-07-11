using Microsoft.Extensions.Logging; // ILoggerProvider, ILogger, LogLevel
using static System.Console;   

namespace Packt.Shared;

// ILoggerProvider Summary:
//     Creates a new Microsoft.Extensions.Logging.ILogger instance.
public class ConsoleLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        // we could have different Logger implemantations for different categoryName values but we only have one
        return new ConsoleLogger();
    }
    // if your logger uses unmanaged resources, then you can release them here
    public void Dispose()
    {

    }

    // ILogger Summary:
    //     Writes a log entry.
    public class ConsoleLogger : ILogger
    {
        // if your logger uses unmanaged resources, you can return the class that implements IDisposable here
        // BeginScope Summary:
        //     Checks if the given logLevel is enabled.
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        // IsEnabled Summary:
        //     Checks if the given logLevel is enabled.
        public bool IsEnabled(LogLevel logLevel)
        {
            // to avoid overlogging, you can filter on the log level
            switch(logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Information:
                case LogLevel.None:
                    return false;
                case LogLevel.Debug:
                case LogLevel.Warning:
                case LogLevel.Error:
                case LogLevel.Critical:
                default:
                    return true;
            };
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
        {
            // Log the level and event indentifier
            Write($"Level: {logLevel}, Event Id {eventId.Id}");

            // only output the state or exception if it exists
            if(state != null)
            {
                Write($", State: {state}");
            }

            if(exception != null)
            {
                Write($", Exception : {exception.Message}");
            }
            WriteLine();
        }

    }
}