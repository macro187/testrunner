using System;
#if NETCOREAPP2_0
using System.Collections.Generic;
#else
using System.Collections;
#endif
using System.Reflection;
using System.Reflection.Emit;
using TestRunner.Infrastructure;

namespace TestRunner.MSTest
{

    /// <summary>
    /// Provides a dynamically created <see cref="Microsoft.VisualStudio.TestTools.UnitTesting.TestContext"/> instance
    /// that behaves as a proxy to <see cref="TestRunner.MSTest.TestContext"/>
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
            var testContextType = GetTestContextType();
            var typeBuilder = GetProxyTypeBuilder(testContextType);
            
            BuildProxyProperty(typeBuilder, testContextType, "CurrentTestOutcome", GetUnitTestOutcomeType());
            BuildProxyProperty(typeBuilder, testContextType, "DataConnection", typeof(System.Data.Common.DbConnection));
            BuildProxyProperty(typeBuilder, testContextType, "DataRow", typeof(System.Data.DataRow));
            BuildProxyProperty(typeBuilder, testContextType, "DeploymentDirectory", typeof(string));
            BuildProxyProperty(typeBuilder, testContextType, "FullyQualifiedTestClassName", typeof(string));
            #if NETCOREAPP2_0
            BuildProxyProperty(typeBuilder, testContextType, "Properties", typeof(IDictionary<string, object>));
            #else
            BuildProxyProperty(typeBuilder, testContextType, "Properties", typeof(IDictionary));
            #endif
            BuildProxyProperty(typeBuilder, testContextType, "ResultsDirectory", typeof(string));
            BuildProxyProperty(typeBuilder, testContextType, "TestDeploymentDir", typeof(string));
            BuildProxyProperty(typeBuilder, testContextType, "TestDir", typeof(string));
            BuildProxyProperty(typeBuilder, testContextType, "TestLogsDir", typeof(string));
            BuildProxyProperty(typeBuilder, testContextType, "TestName", typeof(string));
            BuildProxyProperty(typeBuilder, testContextType, "TestResultsDirectory", typeof(string));
            BuildProxyProperty(typeBuilder, testContextType, "TestRunDirectory", typeof(string));
            BuildProxyProperty(typeBuilder, testContextType, "TestRunResultsDirectory", typeof(string));

            BuildProxyMethod(typeBuilder, testContextType, "AddResultFile", null, typeof(string));
            BuildProxyMethod(typeBuilder, testContextType, "BeginTimer", null, typeof(string));
            BuildProxyMethod(typeBuilder, testContextType, "EndTimer", null, typeof(string));
            BuildProxyMethod(typeBuilder, testContextType, "WriteLine", null, typeof(string));
            BuildProxyMethod(typeBuilder, testContextType, "WriteLine", null, typeof(string), typeof(object[]));

            return Activator.CreateInstance(GetTypeFromTypeBuilder(typeBuilder));
        }


        static void BuildProxyMethod(
            TypeBuilder typeBuilder,
            Type baseType,
            string name,
            Type returnType,
            params Type[] parameterTypes)
        {
            parameterTypes = parameterTypes ?? new Type[0];

            var baseMethod = baseType.GetMethod(name, parameterTypes);
            if (baseMethod == null) return;
            if (baseMethod.ReturnType.FullName != (returnType?.FullName ?? "System.Void")) return;

            var method = typeBuilder.DefineMethod(
                name,
                MethodAttributes.Public |
                MethodAttributes.Virtual | 
                MethodAttributes.HideBySig,
                returnType,
                parameterTypes);

            var target = typeof(TestContext).GetMethod(name, parameterTypes);
            if (target == null) throw new Exception("Target TestContext method " + name + " not found");

            var il = method.GetILGenerator();
            if (parameterTypes.Length > 0) il.Emit(OpCodes.Ldarg_0);
            if (parameterTypes.Length > 1) il.Emit(OpCodes.Ldarg_1);
            if (parameterTypes.Length > 2) il.Emit(OpCodes.Ldarg_2);
            if (parameterTypes.Length > 3) il.Emit(OpCodes.Ldarg_3);
            if (parameterTypes.Length > 4) throw new NotSupportedException();
                
            il.Emit(OpCodes.Call, target);
            il.Emit(OpCodes.Ret);
        }


        static void BuildProxyProperty(TypeBuilder typeBuilder, Type baseType, string name, Type type)
        {
            var getterName = "get_" + name;

            var baseGetter = baseType.GetMethod(getterName);
            if (baseGetter == null) return;
            if (baseGetter.ReturnType.FullName != (type?.FullName ?? "System.Void")) return;

            var getter = typeBuilder.DefineMethod(
                getterName,
                MethodAttributes.Public |
                MethodAttributes.Virtual | 
                MethodAttributes.HideBySig |
                MethodAttributes.SpecialName,
                type,
                Type.EmptyTypes);

            var target = typeof(TestContext).GetProperty(name).GetMethod;
            if (target == null) throw new Exception("Target TestContext property " + name + " getter not found");

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


        static TypeBuilder GetProxyTypeBuilder(Type testContextType)
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
                    testContextType);
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
            var type =
                GetMSTestExtensionsAssembly()
                    .GetType("Microsoft.VisualStudio.TestTools.UnitTesting.TestContext", false);

            if (type == null)
                throw new UserException("No TestContext type found in linked MSTest DLL");
            
            return type;
        }


        static Type GetUnitTestOutcomeType()
        {
            var type =
                GetMSTestAssembly()
                    .GetType("Microsoft.VisualStudio.TestTools.UnitTesting.UnitTestOutcome", false);

            if (type == null)
                throw new UserException("No UnitTestOutcome type found in linked MSTest DLL");
            
            return type;
        }


        static Assembly GetMSTestAssembly()
        {
            Assembly assembly = null;
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = a.GetName().Name;

                // Old-style MSTest
                if (name == "Microsoft.VisualStudio.QualityTools.UnitTestFramework")
                {
                    assembly = a;
                    break;
                }

                // New-style MSTest
                if (name == "Microsoft.VisualStudio.TestPlatform.TestFramework")
                {
                    assembly = a;
                    break;
                }
            }

            if (assembly == null)
                throw new UserException("Test DLL doesn't appear to be linked to an MSTest DLL");

            return assembly;
        }


        static Assembly GetMSTestExtensionsAssembly()
        {
            Assembly assembly = null;
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = a.GetName().Name;

                // In old-style MSTest everything is in one assembly
                if (name == "Microsoft.VisualStudio.QualityTools.UnitTestFramework")
                {
                    assembly = a;
                    break;
                }

                // In new-style MSTest some stuff is in a separate .Extensions assembly
                if (name == "Microsoft.VisualStudio.TestPlatform.TestFramework")
                {
                    assembly = Assembly.LoadFrom(a.Location.Substring(0, a.Location.Length - 4) + ".Extensions.dll");
                    break;
                }
            }

            if (assembly == null)
                throw new UserException("Test DLL doesn't appear to be linked to an MSTest DLL");

            return assembly;
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
