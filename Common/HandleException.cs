using System;
using System.IO;

namespace Common
{
    /// <summary>
    /// Centralized file-based exception logger.
    /// Every DAL method's catch block calls HandleException.FoundException(ex)
    /// before returning its fallback value (per rulebook Rule 7.3).
    /// The UI-facing counterpart is Models.GlobalModels.FoundExceptionUI(ex) in
    /// PharmacyApp, which calls into this logger and then shows a modal error dialog.
    /// </summary>
    public static class HandleException
    {
        private static readonly string LogDirectory =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        private static readonly object LockObject = new object();

        public static void FoundException(Exception ex)
        {
            if (ex == null) return;

            try
            {
                lock (LockObject)
                {
                    if (!Directory.Exists(LogDirectory))
                    {
                        Directory.CreateDirectory(LogDirectory);
                    }

                    string logFile = Path.Combine(LogDirectory, $"log_{DateTime.Now:yyyy-MM-dd}.txt");

                    string entry =
                        $"----------------------------------------{Environment.NewLine}" +
                        $"Timestamp : {DateTime.Now:yyyy-MM-dd HH:mm:ss}{Environment.NewLine}" +
                        $"Message   : {ex.Message}{Environment.NewLine}" +
                        $"Source    : {ex.Source}{Environment.NewLine}" +
                        $"StackTrace: {ex.StackTrace}{Environment.NewLine}";

                    if (ex.InnerException != null)
                    {
                        entry += $"Inner     : {ex.InnerException.Message}{Environment.NewLine}";
                    }

                    File.AppendAllText(logFile, entry);
                }
            }
            catch
            {
                // Logging must never itself throw and mask the original exception.
            }
        }
    }
}
