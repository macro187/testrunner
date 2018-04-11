using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using static TestRunner.Infrastructure.ConsoleExtensions;

namespace TestRunner.Program
{

    static class ConfigFileSwitcher
    {

        /// <summary>
        /// Switch to using a specified assembly .config file (if present)
        /// </summary>
        ///
        static public void SwitchTo(string configPath)
        {
            if (!File.Exists(configPath)) return;

            #if NET461
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", configPath);

            //
            // The following hackery forces the new config file to take effect
            //
            // See http://stackoverflow.com/questions/6150644/change-default-app-config-at-runtime/6151688#6151688
            //
            var initStateField =
                typeof(ConfigurationManager).GetField("s_initState", BindingFlags.NonPublic | BindingFlags.Static);
            if (initStateField != null)
            {
                initStateField.SetValue(null, 0);
            }

            var configSystemField =
                typeof(ConfigurationManager).GetField("s_configSystem", BindingFlags.NonPublic | BindingFlags.Static);
            if (configSystemField != null)
            {
                configSystemField.SetValue(null, null);
            }

            var clientConfigPathsType =
                typeof(ConfigurationManager)
                    .Assembly
                    .GetTypes()
                    .FirstOrDefault(x => x.FullName == "System.Configuration.ClientConfigPaths");
            var currentField =
                clientConfigPathsType != null
                    ? clientConfigPathsType.GetField("s_current", BindingFlags.NonPublic | BindingFlags.Static)
                    : null;
            if (currentField != null)
            {
                currentField.SetValue(null, null);
            }

            WriteLine();
            WriteLine("Configuration File:");
            WriteLine(configPath);

            if (Type.GetType("Mono.Runtime") != null)
            {
                WriteLine();
                WriteLine("WARNING: Running on Mono, configuration file will probably not take effect");
                WriteLine("See https://bugzilla.xamarin.com/show_bug.cgi?id=15741");
            }
            #endif
        }
        
    }
}
