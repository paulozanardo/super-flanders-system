using System;
using Microsoft.Win32;

namespace DescobrirNetFramework
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Get45Or451FromRegistry();
        }

        private static void Get45Or451FromRegistry()
        {
            using (
                RegistryKey ndpKey =
                    RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                        .OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {

                Console.WriteLine();
                Console.WriteLine();

                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    Console.WriteLine("Sua versão do .NET Framework é: " +
                                      CheckFor45DotVersion((int) ndpKey.GetValue("Release")));
                    Console.WriteLine();
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Versão 4.5 do .NET Framework ou posterior não foi detectado favor atualizar.");
                }
            }

            GetVersionFromEnvironment();
            GetVersionFromRegistry();

            Console.ReadKey();
        }

        private static void GetVersionFromEnvironment()
        {
            Console.WriteLine(
                "-------------------------------------------------------------------------------------------------------------- ");
            Console.WriteLine(
                "|Para as versões do .NET Framework 4, 4.5, 4.5.1 e 4.5.2 o Environment tem a forma 4.0.30319.xxxxx.           |");
            Console.WriteLine(
                "|Para o .NET Framework 4.6, ele tem a forma 4.0.30319.42000.                                                  |");
            Console.WriteLine(
                "-------------------------------------------------------------------------------------------------------------- ");
            Console.WriteLine();
            Console.WriteLine("Versão Environment: " + Environment.Version.ToString());


        }

        private static string CheckFor45DotVersion(int releaseKey)
        {
            if (releaseKey >= 393295)
            {
                return "4.6 ou posterior";
            }
            if ((releaseKey >= 379893))
            {
                return "4.5.2 ou posterior";
            }
            if ((releaseKey >= 378675))
            {
                return "4.5.1 ou posterior";
            }
            if ((releaseKey >= 378389))
            {
                return "4.5 ou posterior";
            }
            return "Nenhuma versão 4.5 ou posterior detectado";
        }

        private static void GetVersionFromRegistry()
        {
            Console.WriteLine();
            Console.WriteLine(".NET FRAMEWORK presentes na máquina:");
            // Opens the registry key for the .NET Framework entry.
            using (RegistryKey ndpKey =
                RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").
                    OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                // As an alternative, if you know the computers you will query are running .NET Framework 4.5 
                // or later, you can use:
                // using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, 
                // RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
                foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {

                        RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                        string name = (string) versionKey.GetValue("Version", "");
                        string sp = versionKey.GetValue("SP", "").ToString();
                        string install = versionKey.GetValue("Install", "").ToString();
                        if (install == "") //no install info, must be later.
                            Console.WriteLine(" - " + versionKeyName + "  " + name);
                        else
                        {
                            if (sp != "" && install == "1")
                            {
                                Console.WriteLine(" - "+versionKeyName + "  " + name + "  SP" + sp);
                            }

                        }
                        if (name != "")
                        {
                            continue;
                        }
                        foreach (string subKeyName in versionKey.GetSubKeyNames())
                        {
                            RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                            name = (string) subKey.GetValue("Version", "");
                            if (name != "")
                                sp = subKey.GetValue("SP", "").ToString();
                            install = subKey.GetValue("Install", "").ToString();
                            if (install == "") //no install info, must be later.
                                Console.WriteLine(" - " + versionKeyName + "  " + name);
                            else
                            {
                                if (sp != "" && install == "1")
                                {
                                    Console.WriteLine(" -  - " + subKeyName + "  " + name + "  SP" + sp);
                                }
                                else if (install == "1")
                                {
                                    Console.WriteLine(" -  -* " + subKeyName + "  " + name);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}


