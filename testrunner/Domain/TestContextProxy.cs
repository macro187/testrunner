using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using TestRunner.Infrastructure;

namespace TestRunner.Domain
{

    /// <summary>
    /// Provides a dynamically created <see cref="Microsoft.VisualStudio.TestTools.UnitTesting.TestContext"/> instance
    /// that behaves as a proxy to <see cref="TestRunner.Domain.TestContext"/>
    /// </summary>
    ///
    static class TestContextProxy
    {
        
        /// <summary>
        /// The <see cref="Microsoft.VisualStudio.TestTools.UnitTesting.TestContext"/> instance
        /// </summary>
        ///
        public static object Proxy
        {
            get
            {
                if (_proxy == null) _proxy = BuildProxy();
                return _proxy;
            }
        }

        static object _proxy;


        static object BuildProxy()
        {
            var typeBuilder = GetProxyTypeBuilder();
            
            BuildProxyProperty(typeBuilder, "CurrentTestOutcome", typeof(int));
            BuildProxyProperty(typeBuilder, "DataConnection", typeof(System.Data.Common.DbConnection));
            BuildProxyProperty(typeBuilder, "DataRow", typeof(System.Data.DataRow));
            BuildProxyProperty(typeBuilder, "DeploymentDirectory", typeof(string));
            BuildProxyProperty(typeBuilder, "FullyQualifiedTestClassName", typeof(string));
            BuildProxyProperty(typeBuilder, "Properties", typeof(IDictionary));
            BuildProxyProperty(typeBuilder, "ResultsDirectory", typeof(string));
            BuildProxyProperty(typeBuilder, "TestDeploymentDir", typeof(string));
            BuildProxyProperty(typeBuilder, "TestDir", typeof(string));
            BuildProxyProperty(typeBuilder, "TestLogsDir", typeof(string));
            BuildProxyProperty(typeBuilder, "TestName", typeof(string));
            BuildProxyProperty(typeBuilder, "TestResultsDirectory", typeof(string));
            BuildProxyProperty(typeBuilder, "TestRunDirectory", typeof(string));
            BuildProxyProperty(typeBuilder, "TestRunResultsDirectory", typeof(string));

            BuildProxyMethod(typeBuilder, "AddResultFile", null, typeof(string));
            BuildProxyMethod(typeBuilder, "BeginTimer", null, typeof(string));
            BuildProxyMethod(typeBuilder, "EndTimer", null, typeof(string));
            BuildProxyMethod(typeBuilder, "WriteLine", null, typeof(string));
            BuildProxyMethod(typeBuilder, "WriteLine", null, typeof(string), typeof(object[]));

            return Activator.CreateInstance(GetTypeFromTypeBuilder(typeBuilder));
        }


        static void BuildProxyMethod(
            TypeBuilder typeBuilder,
            string name,
            Type returnType,
            params Type[] parameterTypes)
        {
            parameterTypes = parameterTypes ?? new Type[0];

            var method = typeBuilder.DefineMethod(
                name,
                MethodAttributes.Public |
                MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | 
                MethodAttributes.HideBySig,
                returnType,
                parameterTypes);

            var target = typeof(TestContext).GetMethod(name, parameterTypes);

            var il = method.GetILGenerator();
            if (parameterTypes.Length > 0) il.Emit(OpCodes.Ldarg_0);
            if (parameterTypes.Length > 1) il.Emit(OpCodes.Ldarg_1);
            if (parameterTypes.Length > 2) il.Emit(OpCodes.Ldarg_2);
            if (parameterTypes.Length > 3) il.Emit(OpCodes.Ldarg_3);
            if (parameterTypes.Length > 4) throw new NotSupportedException();
                
            il.Emit(OpCodes.Call, target);
            il.Emit(OpCodes.Ret);
        }


        static void BuildProxyProperty(TypeBuilder typeBuilder, string name, Type type)
        {
            var getter = typeBuilder.DefineMethod(
                "get_" + name,
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | 
                MethodAttributes.HideBySig,
                type,
                Type.EmptyTypes);

            var target = typeof(TestContext).GetProperty(name).GetMethod;

            var il = getter.GetILGenerator();
            il.Emit(OpCodes.Call, target);
            il.Emit(OpCodes.Ret);

            var property = typeBuilder.DefineProperty(
                name,
                PropertyAttributes.None,
                type,
                null);
            
            property.SetGetMethod(getter);
        }


        static TypeBuilder GetProxyTypeBuilder()
        {
            return GetProxyAssemblyBuilder()
                .DefineDynamicModule("MainModule")
                .DefineType(
                    "TestContextProxy",
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    GetTestContextType());
        }


        static AssemblyBuilder GetProxyAssemblyBuilder()
        {
            #if NETCOREAPP2_0
                return AssemblyBuilder.DefineDynamicAssembly(
                    new AssemblyName("TestContextProxyAssembly"),
                    AssemblyBuilderAccess.Run);
            #else
                return AppDomain.CurrentDomain.DefineDynamicAssembly(
                    new AssemblyName("TestContextProxyAssembly"),
                    AssemblyBuilderAccess.Run);
            #endif
        }


        static Type GetTestContextType()
        {
            Assembly assembly = null;
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = a.GetName().Name;

                // Old-style MSTest: TestContext is in the main assembly
                if (name == "Microsoft.VisualStudio.QualityTools.UnitTestFramework")
                {
                    assembly = a;
                    break;
                }

                // New-style MSTest: TestContext is in a separate .Extensions assembly
                if (name == "Microsoft.VisualStudio.TestPlatform.TestFramework")
                {
                    assembly = Assembly.LoadFrom(a.Location.Substring(0, a.Location.Length - 4) + ".Extensions.dll");
                    break;
                }
            }

            if (assembly == null)
                throw new UserException("Test DLL doesn't appear to be linked to an MSTest DLL");

            var type = assembly.GetType("Microsoft.VisualStudio.TestTools.UnitTesting.TestContext", false);

            if (type == null)
                throw new UserException("No TestContext type found in linked MSTest DLL");
            
            return type;
        }


        static Type GetTypeFromTypeBuilder(TypeBuilder typeBuilder)
        {
            #if NETCOREAPP2_0
                return typeBuilder.CreateTypeInfo().AsType();
            #else
                return typeBuilder.CreateType();
            #endif
        }

    }
}
