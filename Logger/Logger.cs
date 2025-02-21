using System;
using UnityEngine;

namespace Bubu.UnityUtils
{
    public class Logger
    {
        public LogLevel LogLevel { get; set; }

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel;

        public void Log(LogLevel logLevel, string message)
        {
            if (!IsEnabled(logLevel)) return;

            Debug.Log($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] [{logLevel.ToString().ToUpper()}] {message}");
        }
    }
}
