using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TestRunner.Infrastructure;
using static TestRunner.Events.EventHandler;

namespace TestRunner.Program
{
    static class Program
    {

        static readonly string ProgramPath = Assembly.GetExecutingAssembly().Location;
        static readonly string ProgramName = Path.GetFileName(ProgramPath);


        [STAThread]
        static void Main(string[] args)
        {
            //
            // Exit the program immediately, killing off any background threads
            //
            Environment.Exit(Main2(args));
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Required to handle unexpected exceptions")]
        static int Main2(string[] args)
        {
            try
            {
                return Main3(args);
            }

            //
            // Handle user-facing errors
            //
            catch (UserException ue)
            {
                DiagnosticEvent();
                DiagnosticEvent(ue.Message);
                return 1;
            }

            //
            // Handle internal errors
            //
            catch (Exception e)
            {
                DiagnosticEvent();
                DiagnosticEvent($"An internal error occurred in {ProgramName}:");
                DiagnosticEvent(ExceptionExtensions.FormatException(e));
                return 1;
            }
        }


        static int Main3(string[] args)
        {
            //
            // Route trace output to stdout
            //
            Trace.Listeners.Add(new TestRunnerTraceListener());

            //
            // Parse arguments
            //
            ArgumentParser.Parse(args);
            if (!ArgumentParser.Success)
            {
                DiagnosticEvent();
                DiagnosticEvent(ArgumentParser.GetUsage());
                DiagnosticEvent();
                DiagnosticEvent();
                DiagnosticEvent(ArgumentParser.ErrorMessage);
                return 1;
            }

            //
            // Parent process: Print the program banner and invoke TestRunner --inproc child processes for each
            // <testassembly> specified on the command line
            //
            if (!ArgumentParser.InProc)
            {
                Banner();
                bool success = true;
                foreach (var assemblyPath in ArgumentParser.AssemblyPaths)
                {
                    var exitCode = 
                        ProcessExtensions.ExecuteDotnet(
                            ProgramPath,
                            "--inproc \"" + assemblyPath + "\"")
                        .ExitCode;

                    if (exitCode != 0) success = false;
                }
                return success ? 0 : 1;
            }

            //
            // Child process: Run the tests in the specified <testassembly>
            //
            else
            {
                return TestAssemblyRunner.Run(ArgumentParser.AssemblyPaths[0]) ? 0 : 1;
            }
        }


        /// <summary>
        /// Print program information
        /// </summary>
        ///
        static void Banner()
        {
            var name = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName;
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            var copyright = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).LegalCopyright;

            BannerEvent(
                $"{name} v{version}",
                copyright);
        }

    }
}
