using System;

namespace Brick.MSTestRunner
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var facade = new Facade();

            if (!facade.ValidateArgs(args))
            {
                Environment.Exit(1);
            }

            var isAllPassed = facade.RunTests(args[0]);
            Environment.Exit(isAllPassed ? 0 : 1);
        }
    }
}
