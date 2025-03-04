using MinVer.Lib;

namespace MinVerTests.Lib.Infra
{
    internal class NullLogger : ILogger
    {
        public static readonly NullLogger Instance =
#if NET
            new();
#else
            new NullLogger();
#endif

        private NullLogger()
        {
        }

        public bool IsTraceEnabled => false;

        public bool IsDebugEnabled => false;

        public bool IsInfoEnabled => false;

        public bool IsWarnEnabled => false;

        public bool Trace(string message) => false;

        public bool Debug(string message) => false;

        public bool Info(string message) => false;

        public bool Warn(int code, string message) => false;
    }
}
