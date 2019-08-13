using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TestRunner.Infrastructure;
using TestRunner.Runners;
using TestRunner.Events;
using System.Collections.Generic;
using TestRunner.Results;
using TestRunner.EventHandlers;
using System.Linq;

namespace TestRunner.Program
{
    static class Program
    {

        static readonly string ProgramPath = Assembly.GetExecutingAssembly().Location;
        static readonly string ProgramName = Path.GetFileName(ProgramPath);


        /// <summary>
        /// 1. Run the program and exit assertively killing any background threads
        /// </summary>
        ///
        [STAThread]
        static void Main(string[] args)
        {
            Environment.Exit(Main2(args));
        }


        /// <summary>
        /// 2. Set up required event handlers
        /// </summary>
        ///
        static int Main2(string[] args)
        {
            EventHandlerPipeline.Append(new MethodResultEventHandler());
            EventHandlerPipeline.Append(new TestResultEventHandler());
            EventHandlerPipeline.Append(new TestClassResultEventHandler());
            EventHandlerPipeline.Append(new TestAssemblyResultEventHandler());
            EventHandlerPipeline.Append(new TestContextEventHandler());
            return Main3(args);
        }


        /// <summary>
        /// 3. Set up error handlers
        /// </summary>
        ///
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Required to handle unexpected exceptions")]
        static int Main3(string[] args)
        {
            try
            {
                return Main4(args);
            }
            catch (UserException ue)
            {
                HandleUserException(ue);
                return 1;
            }
            catch (Exception e)
            {
                HandleInternalException(e);
                return 1;
            }
        }


        /// <summary>
        /// 4. Parse arguments and take action
        /// </summary>
        ///
        static int Main4(string[] args)
        {
            ArgumentParser.Parse(args);

            switch(ArgumentParser.OutputFormat)
            {
                case OutputFormats.Human:
                    EventHandlerPipeline.Append(new HumanOutputEventHandler());
                    break;
                case OutputFormats.Machine:
                    EventHandlerPipeline.Append(new MachineOutputEventHandler());
                    break;
                default:
                    throw new Exception($"Unrecognised <outputformat> from parser {ArgumentParser.OutputFormat}");
            }

            if (!ArgumentParser.Success)
            {
                throw ArgumentParseError();
            }

            if (ArgumentParser.Help)
            {
                return Help();
            }

            if (ArgumentParser.InProc)
            {
                return InProc();
            }

            return Main5();
        }


        /// <summary>
        /// 5. Run test file(s)
        /// </summary>
        //
        static int Main5()
        {
            Banner();
            EventHandlerPipeline.Raise(new TestRunBeginEvent() {});
            bool success = true;
            foreach (var testFile in ArgumentParser.TestFiles)
            {
                if (!Reinvoke(testFile)) success = false;
            }
            EventHandlerPipeline.Raise( new TestRunEndEvent() { Result = new TestRunResult() { Success = success } });
            return success ? 0 : 1;
        }


        /// <summary>
        /// Reinvoke testrunner to run an individual test file in its own process
        /// </summary>
        ///
        static bool Reinvoke(string testFile)
        {
            var exitCode =
                ProcessExtensions.ExecuteDotnet(
                    ProgramPath,
                    $"--inproc --outputformat machine \"{testFile}\"",
                    (proc, line) => {
                        var e = MachineReadableEventSerializer.TryDeserialize(line);
                        EventHandlerPipeline.Raise(
                            e ??
                            new StandardOutputEvent() {
                                ProcessId = proc.Id,
                                Message = line,
                            });
                    },
                    (proc, line) => {
                        EventHandlerPipeline.Raise(
                            new ErrorOutputEvent() {
                                ProcessId = proc.Id,
                                Message = line,
                            });
                    });

            return exitCode == 0;
        }


        /// <summary>
        /// --help: Print out brief program usage information
        /// </summary>
        ///
        static int Help()
        {
            Banner();
            Usage();
            return 0;
        }


        /// <summary>
        /// --inproc: Run an individual test file in-process
        /// </summary>
        ///
        static int InProc()
        {
            var eventHandler = new ResultAccumulatingEventHandler();
            using (EventHandlerPipeline.Append(eventHandler))
            {
                TestAssemblyRunner.Run(ArgumentParser.TestFiles[0]);
            }
            return eventHandler.TestAssemblyResults.Last().Success ? 0 : 1;
        }


        /// <summary>
        /// Handle argument parse error
        /// </summary>
        ///
        static UserException ArgumentParseError()
        {
            Banner();
            Usage();
            return new UserException(ArgumentParser.ErrorMessage);
        }


        /// <summary>
        /// Print program name, version, and copyright banner
        /// </summary>
        ///
        static void Banner()
        {
            var name = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName;
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            var copyright = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).LegalCopyright;

            EventHandlerPipeline.Raise(
                new ProgramBannerEvent() {
                    Lines = new[]{
                        $"{name} v{version}",
                        copyright,
                    },
                });
        }


        /// <summary>
        /// Print program usage information
        /// </summary>
        ///
        static void Usage()
        {
            EventHandlerPipeline.Raise(new ProgramUsageEvent() { Lines = ArgumentParser.GetUsage() });
        }


        /// <summary>
        /// Handle user-facing error
        /// </summary>
        ///
        static void HandleUserException(UserException ue)
        {
            EventHandlerPipeline.Raise(new ProgramUserErrorEvent() { Message = ue.Message });
        }


        /// <summary>
        /// Handle internal TestRunner error
        /// </summary>
        ///
        static void HandleInternalException(Exception e)
        {
            EventHandlerPipeline.Raise(
                new ProgramInternalErrorEvent() {
                    Exception = new ExceptionInfo(e)
                });

            if (e is ReflectionTypeLoadException rtle)
            {
                foreach (var le in rtle.LoaderExceptions)
                {
                    EventHandlerPipeline.Raise(
                        new ProgramInternalErrorEvent() {
                            Exception = new ExceptionInfo(le)
                        });
                }
            }
        }

    }
}
