using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TestRunner.Infrastructure;

namespace TestRunner.Domain
{

    public class TestAssembly
    {

        public static TestAssembly TryCreate(Assembly assembly)
        {
            Guard.NotNull(assembly, nameof(assembly));
            var testAssembly = new TestAssembly(assembly);
            return testAssembly.TestClasses.Any() ? testAssembly : null;
        }


        TestAssembly(Assembly testAssembly)
        {
            Assembly = testAssembly;

            FindTestClasses();
            FindAssemblyInitializeMethod();
            FindAssemblyCleanupMethod();
        }


        public Assembly Assembly
        {
            get;
            private set;
        }


        public ICollection<TestClass> TestClasses
        {
            get;
            private set;
        }


        public MethodInfo AssemblyInitializeMethod
        {
            get;
            private set;
        }


        public MethodInfo AssemblyCleanupMethod
        {
            get;
            private set;
        }


        void FindTestClasses()
        {
            TestClasses =
                new ReadOnlyCollection<TestClass>(
                    Assembly.GetTypes()
                        .Select(t => TestClass.TryCreate(t))
                        .Where(t => t != null)
                        .ToList());
        }


        void FindAssemblyInitializeMethod()
        {
            var methods =
                TestClasses
                    .Select(c => c.AssemblyInitializeMethod)
                    .Where(m => m != null)
                    .ToList();

            if (methods.Count > 1)
                throw new UserException(
                    $"[TestAssembly] {Assembly.FullName} contains more than one [AssemblyInitialize] method");

            if (methods.Count == 0)
                return;
            
            AssemblyInitializeMethod = methods[0];
        }


        void FindAssemblyCleanupMethod()
        {
            var methods =
                TestClasses
                    .Select(c => c.AssemblyCleanupMethod)
                    .Where(c => c != null)
                    .ToList();

            if (methods.Count > 1)
                throw new UserException(
                    $"[TestAssembly] {Assembly.FullName} contains more than one [AssemblyCleanup] method");

            if (methods.Count == 0)
                return;
            
            AssemblyCleanupMethod = methods[0];
        }

    }

}
