using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskProTracker.Tests
{
    [CollectionDefinition("Logger Collection")]
    public class LoggerCollection : ICollectionFixture<LoggerFixture> { }

    public class LoggerFixture : IDisposable
    {
        public ILogger Logger { get; }

        public LoggerFixture()
        {
            Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.File("logs/testlog.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public void Dispose()
        {
            Log.CloseAndFlush();
        }
    }
}
