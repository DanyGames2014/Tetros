using System;

namespace Tetros
{
    public enum LogLevel
    {
        INFO = 0,
        WARNING = 1,
        ERROR = 2,
        FATAL = 3,
    }

    public class Logger
    {
        ErrorLang errorLangClass;
        public string[] errorLang;

        public Logger()
        {
            errorLangClass = new();
            errorLang = errorLangClass.errorLang;
        }

        // Info
        public void WriteInfo(string info_message)
        {
            WriteLine(info_message, LogLevel.INFO);
        }

        public void WriteInfo(string info_message, ConsoleColor consoleColor)
        {
            WriteLine(info_message, LogLevel.INFO, consoleColor);
        }

        // Warn
        public void WriteWarn(string warn_message)
        {
            WriteLine(warn_message, LogLevel.WARNING);
        }

        public void WriteWarn(int errorCode)
        {
            try
            {
                WriteLine(errorLang[errorCode],LogLevel.WARNING);
            }
            catch (Exception e)
            {
                WriteError(e);
            }
            
        }

        // Error
        public void WriteError(string error_message)
        {
            WriteLine(error_message, LogLevel.ERROR);
        }

        public void WriteError(int errorCode)
        {
            try
            {
                WriteError(errorLang[errorCode]);
            }
            catch (Exception e)
            {
                WriteError(e);
            }
        }

        public void WriteError(Exception e)
        {
            WriteError(e.Message);
        }

        public void WriteError(string error_message, Exception e)
        {
            WriteError(error_message + Environment.NewLine + e.Message);
        }

        // General
        public void WriteLine(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void WriteLine(string message, LogLevel level)
        {
            switch (level)
            {
                case LogLevel.INFO:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("[INFO] ");
                    break;

                case LogLevel.WARNING:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[WARN] ");
                    break;

                case LogLevel.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[ERROR] ");
                    break;

                case LogLevel.FATAL:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("[FATAL] ");
                    break;
            }
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void WriteLine(string message, LogLevel level, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            switch (level)
            {
                case LogLevel.INFO:
                    Console.Write("[INFO] ");
                    break;

                case LogLevel.WARNING:
                    Console.Write("[WARN] ");
                    break;

                case LogLevel.ERROR:
                    Console.Write("[ERROR] ");
                    break;

                case LogLevel.FATAL:
                    Console.Write("[FATAL] ");
                    break;
            }

            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
