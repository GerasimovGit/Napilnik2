using System;
using System.IO;

namespace NapilnikTasks
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Pathfinder consoleLogPathfinder = new Pathfinder(new ConsoleLogger());

            Pathfinder fileLogPathfinder = new Pathfinder(new FileLogger());

            Pathfinder conditionalFileLogPathfinder =
                new Pathfinder(new ConditionalLogger(new FileLogger(), new DayOfWeekCondition(DayOfWeek.Friday)));

            Pathfinder conditionalConsoleLogPathfinder =
                new Pathfinder(new ConditionalLogger(new ConsoleLogger(), new DayOfWeekCondition(DayOfWeek.Friday)));

            Pathfinder multiplyConsoleLogPathfinder = new Pathfinder(new MultiplyLogger(new ConsoleLogger(),
                new ConditionalLogger(new FileLogger(), new DayOfWeekCondition(DayOfWeek.Friday))));

            multiplyConsoleLogPathfinder.Find();

            Console.ReadKey();
        }
    }

    public interface ILogger
    {
        void Write(string message);
    }

    public interface ILoggerCondition
    {
        bool CanWrite(string message);
    }

    public class Pathfinder
    {
        private readonly ILogger _logger;

        public Pathfinder(ILogger logger)
        {
            _logger = logger;
        }

        public void Find()
        {
            _logger.Write("I Find!");
        }
    }

    public class ConsoleLogger : ILogger
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class FileLogger : ILogger
    {
        public void Write(string message)
        {
            File.WriteAllText("log.txt", message);
        }
    }

    public class ConditionalLogger : ILogger
    {
        private readonly ILoggerCondition _condition;
        private readonly ILogger _logger;

        public ConditionalLogger(ILogger logger, ILoggerCondition condition)
        {
            _logger = logger;
            _condition = condition;
        }

        public void Write(string message)
        {
            if (_condition.CanWrite(message))
                _logger.Write(message);
        }
    }

    public class DayOfWeekCondition : ILoggerCondition
    {
        private readonly DayOfWeek _targetDay;

        public DayOfWeekCondition(DayOfWeek targetDay)
        {
            _targetDay = targetDay;
        }

        public bool CanWrite(string message)
        {
            return DateTime.Now.DayOfWeek == _targetDay;
        }
    }

    public class MultiplyLogger : ILogger
    {
        private readonly ILogger[] _loggers;

        public MultiplyLogger(params ILogger[] loggers)
        {
            _loggers = loggers;
        }

        public void Write(string message)
        {
            foreach (ILogger logger in _loggers)
                logger.Write(message);
        }
    }
}