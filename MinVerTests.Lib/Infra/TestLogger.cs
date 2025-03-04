using System;
using System.Collections.Generic;
using System.Linq;
using MinVer.Lib;

namespace MinVerTests.Lib.Infra
{
    internal class TestLogger : ILogger
    {
#if NET6_0_OR_GREATER
        private readonly List<LogMessage> messages = new();
#else
        private readonly List<LogMessage> messages = new List<LogMessage>();
#endif

        public bool IsTraceEnabled => true;

        public bool IsDebugEnabled => true;

        public bool IsInfoEnabled => true;

        public bool IsWarnEnabled => true;

        public IEnumerable<LogMessage> Messages => this.messages;

        public bool Trace(string message)
        {
            this.messages.Add(new LogMessage(LogLevel.Trace, message, 0));
            return true;
        }

        public bool Debug(string message)
        {
            this.messages.Add(new LogMessage(LogLevel.Debug, message, 0));
            return true;
        }

        public bool Info(string message)
        {
            this.messages.Add(new LogMessage(LogLevel.Info, message, 0));
            return true;
        }

        public bool Warn(int code, string message)
        {
            this.messages.Add(new LogMessage(LogLevel.Warn, message, 0));
            return true;
        }

        public override string ToString() => string.Join(Environment.NewLine, this.Messages.Select(message => message.ToString()));
    }
}
