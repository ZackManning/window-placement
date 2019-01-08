using System;
using System.Diagnostics;
using System.IO;

namespace SharedUtilties
{
    public class Logger
    {
        protected string logFilePath;

        public Logger(string filePath)
        {
            logFilePath = filePath;
        }

        public void Write(string text)
        {
            using (var logWriter = new StreamWriter(logFilePath, true))
            {
                text = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF")}: {text}";
                logWriter.Write(text);
                Debug.Write(text);
            }
        }

        public void WriteLine(string text)
        {
            Write($"{text}\r\n");
        }
    }
}
