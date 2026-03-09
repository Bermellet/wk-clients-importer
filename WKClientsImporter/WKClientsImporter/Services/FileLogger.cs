using System;
using System.IO;
using System.Text;
using WKClientsImporter.Interfaces;

namespace WKClientsImporter.Services
{
    public class FileLogger : ILogger, IDisposable
    {
        private readonly string _logPath;
        private readonly object _sync = new object();
        private StreamWriter _writer;

        public FileLogger()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var logsDir = Path.Combine(baseDir, "Logs");
            if (!Directory.Exists(logsDir))
            {
                Directory.CreateDirectory(logsDir);
            }

            _logPath = Path.Combine(logsDir, $"app_{DateTime.Now:yyyyMMdd}.log");
            _writer = new StreamWriter(new FileStream(_logPath, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                AutoFlush = true
            };
        }

        public void LogInfo(string message) => Write("INFO", message, null);
        public void LogWarning(string message) => Write("WARN", message, null);
        public void LogError(string message, Exception ex = null) => Write("ERROR", message, ex);

        private void Write(string level, string message, Exception ex)
        {
            try
            {
                lock (_sync)
                {
                    var sb = new StringBuilder();
                    sb.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff} [{1}] {2}", DateTime.Now, level, message);
                    if (ex != null)
                    {
                        sb.AppendLine();
                        sb.AppendLine(ex.ToString());
                    }
                    _writer.WriteLine(sb.ToString());
                }
            }
            catch
            {
                // Nunca lanzar desde el logger — falla silenciosa (no queremos romper la app por logging).
            }
        }

        public void Dispose()
        {
            lock (_sync)
            {
                _writer?.Dispose();
                _writer = null;
            }
        }
    }
}