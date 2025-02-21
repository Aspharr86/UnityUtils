namespace Bubu.UnityUtils
{
    /// <remarks>
    /// <para> Reference: https://learn.microsoft.com/zh-tw/dotnet/api/microsoft.extensions.logging.loggerextensions?view=net-8.0-pp </para>
    /// </remarks>
    public static class LoggerExtensions
    {
        public static void LogTrace(this Logger logger, string message)
        {
            logger.Log(LogLevel.Trace, message);
        }

        public static void LogDebug(this Logger logger, string message)
        {
            logger.Log(LogLevel.Debug, message);
        }

        public static void LogInformation(this Logger logger, string message)
        {
            logger.Log(LogLevel.Information, message);
        }

        public static void LogWarning(this Logger logger, string message)
        {
            logger.Log(LogLevel.Warning, message);
        }

        public static void LogError(this Logger logger, string message)
        {
            logger.Log(LogLevel.Error, message);
        }

        public static void LogCritical(this Logger logger, string message)
        {
            logger.Log(LogLevel.Critical, message);
        }
    }
}
