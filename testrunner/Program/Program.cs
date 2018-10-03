using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TestRunner.Infrastructure;
using TestRunner.Runners;
using TestRunner.Events;
using EventHandler = TestRunner.Events.EventHandler;

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
            EventHandler.Append(new TimingEventHandler());
            EventHandler.Append(new TestContextEventHandler());
            EventHandler.Append(new DefaultOutputEventHandler());

            try
            {
                return Main3(args);
            }

            //
            // Handle user-facing errors
            //
            catch (UserException ue)
            {
                EventHandler.First.ProgramUserErrorEvent(ue);
                return 1;
            }

            //
            // Handle internal errors
            //
            catch (Exception e)
            {
                EventHandler.First.ProgramInternalErrorEvent(e);
                return 1;
            }
        }


        static int Main3(string[] args)
        {
            //
            // Parse arguments
            //
            ArgumentParser.Parse(args);
            if (!ArgumentParser.Success)
            {
                EventHandler.First.ProgramUsageEvent(ArgumentParser.GetUsage());
                throw new UserException(ArgumentParser.ErrorMessage);
            }

            if (ArgumentParser.Help)
            {
                EventHandler.First.ProgramUsageEvent(ArgumentParser.GetUsage());
                return 0;
            }

            //
            // Parent process: Print the program banner and invoke TestRunner --inproc child processes for each
            // <testfile> specified on the command line
            //
            if (!ArgumentParser.InProc)
            {
                Banner();
                bool success = true;
                foreach (var testFile in ArgumentParser.TestFiles)
                {
                    var exitCode = 
                        ProcessExtensions.ExecuteDotnet(
                            ProgramPath,
                            "--inproc \"" + testFile + "\"")
                        .ExitCode;

                    if (exitCode != 0) success = false;
                }
                return success ? 0 : 1;
            }

            //
            // Child process: Run the tests in the specified <testfile>
            //
            else
            {
                return TestAssemblyRunner.Run(ArgumentParser.TestFiles[0]) ? 0 : 1;
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

            EventHandler.First.ProgramBannerEvent(
                $"{name} v{version}",
                copyright);
        }

    }
}
