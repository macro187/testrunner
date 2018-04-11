using System;
using System.Linq;

namespace TestRunner.Infrastructure
{

    static class ConsoleExtensions
    {

        static public void WriteLine()
        {
            Console.Out.WriteLine();
        }


        static public void WriteLine(string value)
        {
            Console.Out.WriteLine(value);
        }


        static public void WriteLine(string format, object arg0)
        {
            Console.Out.WriteLine(format, arg0);
        }


        static public void WriteLine(string format, params object[] arg)
        {
            Console.Out.WriteLine(format, arg);
        }


        static public void WriteHeading(params string[] lines)
        {
            WriteHeading('=', lines);
        }


        static public void WriteSubheading(params string[] lines)
        {
            WriteHeading('-', lines);
        }


        static void WriteHeading(char ruleCharacter, params string[] lines)
        {
            if (lines == null) return;
            if (lines.Length == 0) return;

            var longestLine = lines.Max(line => line.Length);
            var rule = new string(ruleCharacter, longestLine);

            WriteLine();
            WriteLine(rule);
            foreach (var line in lines)
            {
                WriteLine(line);
            }
            WriteLine(rule);
        }

    }
}
