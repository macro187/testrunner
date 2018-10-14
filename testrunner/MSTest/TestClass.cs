using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TestRunner.Infrastructure;

namespace TestRunner.MSTest
{

    public class TestClass
    {

        internal static TestClass TryCreate(Type type)
        {
            Guard.NotNull(type, nameof(type));
            if (!type.HasCustomAttribute(TestClassAttribute.TryCreate)) return null;
            var testClass = new TestClass(type);
            if (testClass.TestMethods.Count == 0) return null;
            return testClass;
        }


        TestClass(Type testClass)
        {
            Type = testClass;
            IsIgnored = Type.HasCustomAttribute(IgnoreAttribute.TryCreate);
            FindTestMethods();
            FindAssemblyInitializeMethod();
            FindAssemblyCleanupMethod();
            FindClassInitializeMethod();
            FindClassCleanupMethod();
            FindTestInitializeMethod();
            FindTestCleanupMethod();
            FindTestContextSetter();
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "This is the most appropriate name")]
        public Type Type
        {
            get;
            private set;
        }


        public bool IsIgnored
        {
            get;
            private set;
        }


        public ICollection<TestMethod> TestMethods
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


        public MethodInfo ClassInitializeMethod
        {
            get;
            private set;
        }


        public MethodInfo ClassCleanupMethod
        {
            get;
            private set;
        }


        public MethodInfo TestInitializeMethod
        {
            get;
            private set;
        }


        public MethodInfo TestCleanupMethod
        {
            get;
            private set;
        }


        /// <summary>
        /// Setter method for the test class' <c>TestContext</c> property OR <c>null</c> if it doesn't have one OR
        /// <c>null</c> if it is read-only
        /// </summary>
        ///
        public MethodInfo TestContextSetter
        {
            get;
            private set;
        }


        public string Name
        {
            get
            {
                return Type.Name;
            }
        }
        

        public string FullName
        {
            get
            {
                return Type.FullName;
            }
        }


        void FindTestMethods()
        {
            TestMethods =
                new ReadOnlyCollection<TestMethod>(
                    Type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .Select(m => TestMethod.TryCreate(m))
                        .Where(m => m != null)
                        .ToList());
        }


        void FindAssemblyInitializeMethod()
        {
            var methods =
                Type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.HasCustomAttribute(AssemblyInitializeAttribute.TryCreate))
                    .ToList();

            if (methods.Count > 1)
                throw new UserException(
                    $"[TestClass] {Type.FullName} contains more than one [AssemblyInitialize] method");

            if (methods.Count == 0)
                return;
            
            AssemblyInitializeMethod = methods[0];
        }


        void FindAssemblyCleanupMethod()
        {
            var methods =
                Type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.HasCustomAttribute(AssemblyCleanupAttribute.TryCreate))
                    .ToList();

            if (methods.Count > 1)
                throw new UserException(
                    $"[TestClass] {Type.FullName} contains more than one [AssemblyCleanup] method");

            if (methods.Count == 0)
                return;
            
            AssemblyCleanupMethod = methods[0];
        }


        void FindClassInitializeMethod()
        {
            var methods =
                Type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.HasCustomAttribute(ClassInitializeAttribute.TryCreate))
                    .ToList();

            if (methods.Count > 1)
                throw new UserException(
                    $"[TestClass] {Type.FullName} contains more than one [ClassInitialize] method");

            if (methods.Count == 0)
                return;
            
            ClassInitializeMethod = methods[0];
        }


        void FindClassCleanupMethod()
        {
            var methods =
                Type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.HasCustomAttribute(ClassCleanupAttribute.TryCreate))
                    .ToList();

            if (methods.Count > 1)
                throw new UserException(
                    $"[TestClass] {Type.FullName} contains more than one [ClassCleanup] method");

            if (methods.Count == 0)
                return;
            
            ClassCleanupMethod = methods[0];
        }


        void FindTestInitializeMethod()
        {
            var methods =
                Type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.HasCustomAttribute(TestInitializeAttribute.TryCreate))
                    .ToList();

            if (methods.Count > 1)
                throw new UserException(
                    $"[TestClass] {Type.FullName} contains more than one [TestInitialize] method");

            if (methods.Count == 0)
                return;
            
            TestInitializeMethod = methods[0];
        }


        void FindTestCleanupMethod()
        {
            var methods =
                Type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.HasCustomAttribute(TestCleanupAttribute.TryCreate))
                    .ToList();

            if (methods.Count > 1)
                throw new UserException(
                    $"[TestClass] {Type.FullName} contains more than one [TestCleanup] method");

            if (methods.Count == 0)
                return;
            
            TestCleanupMethod = methods[0];
        }


        void FindTestContextSetter()
        {
            var property = Type.GetProperty("TestContext", BindingFlags.Instance | BindingFlags.Public);
            if (property == null) return;

            var type = property.PropertyType.FullName;
            if (type != "Microsoft.VisualStudio.TestTools.UnitTesting.TestContext") return;

            var setter = property.SetMethod;
            if (setter == null) return;

            TestContextSetter = setter;
        }

    }

}
