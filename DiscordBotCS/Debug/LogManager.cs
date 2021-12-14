using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;

namespace DiscordBotCS.Debug
{
    public class LogManager
    {
        #region Members
        private string filename;
        private StreamWriter file;
        #endregion

        public LogManager(string path = @"logs\DiscordBot - [DT].txt")
        {
            path = path.Replace("[DT]", DateTime.Now.ToString("MMddyyyymmssfff"));
            var directories = path.Split('\\');

            // Create directories if they don't exist
            var directory = Directory.GetCurrentDirectory();
            foreach (var dir in directories.Take(directories.Length - 1))
            {
                if (!Directory.Exists(Path.Combine(directory, dir)))
                {
                    Directory.CreateDirectory(Path.Combine(directory, dir));
                }
                directory = dir;
            }

            filename = Path.Combine(Directory.GetCurrentDirectory(), path);
            file = new StreamWriter(filename);
        }

        ~LogManager()
        {
            file.Close();
        }

        public async Task Log(string message, string type = "LOG")
        {
            var dateTime = DateTime.Now;
            string logMessage = $"{dateTime.ToString("MM/dd/yyyy H:mm:ss.fff")} [{type}] -> {message}";

            await file.WriteLineAsync(logMessage);
        }
        
        public Task LogDiscordMessage(LogMessage log)
        {
            var dateTime = DateTime.Now;
            string logMessage = $"{dateTime.ToString("MM/dd/yyyy H:mm:ss.fff")} [DISCORD] -> {log.ToString()}";

            Console.WriteLine(logMessage);
            file.WriteLine(logMessage);
            return Task.CompletedTask;
        }
    }
}
