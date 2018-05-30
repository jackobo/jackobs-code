using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace GGPMockBootstrapper.Models
{
    public static class InstalledBrowsersProvider
    {
        
        public static BrowserInfo[] GetBrowsers()
        {
            var browsers = new List<BrowserInfo>();
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                RegistryKey webClientsRootKey = hklm.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");
                if (webClientsRootKey != null)
                    foreach (var subKeyName in webClientsRootKey.GetSubKeyNames())
                        if (webClientsRootKey.OpenSubKey(subKeyName) != null)
                            if (webClientsRootKey.OpenSubKey(subKeyName).OpenSubKey("shell") != null)
                                if (webClientsRootKey.OpenSubKey(subKeyName).OpenSubKey("shell").OpenSubKey("open") != null)
                                    if (webClientsRootKey.OpenSubKey(subKeyName).OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command") != null)
                                    {
                                        string commandLineUri = (string)webClientsRootKey.OpenSubKey(subKeyName).OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command").GetValue(null);
                                        if (string.IsNullOrEmpty(commandLineUri))
                                            continue;
                                        commandLineUri = commandLineUri.Trim("\"".ToCharArray());

                                        browsers.Add(new BrowserInfo((string)webClientsRootKey.OpenSubKey(subKeyName).GetValue(null),
                                                                            commandLineUri));
                                        
                                    }
            }

            return browsers.ToArray();

            
        }


        public static void OpeneWithDefaultBrowser(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        public static void OpenWithChromeIfPossible(string url)
        {
            var chrome = GetBrowsers().FirstOrDefault(b => b.Name.ToLowerInvariant().Contains("chrome"));

            if (chrome != null)
                chrome.OpenUrl(url);
            else
                OpeneWithDefaultBrowser(url);
        }
    }

    public class BrowserInfo
    {
        public BrowserInfo(string name, string executable)
        {
            this.Name = name;
            this.Executable = executable;
        }

        public static readonly BrowserInfo DefaultBrowser = new BrowserInfo("Default browser", string.Empty);

        public string Executable { get; private set; }
        public string Name { get; private set; }


        public override bool Equals(object obj)
        {
            var theOther = obj as BrowserInfo;

            if (theOther == null)
                return false;

            return this.Name == theOther.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void OpenUrl(string url)
        {
            if (string.IsNullOrEmpty(this.Executable))
                InstalledBrowsersProvider.OpeneWithDefaultBrowser(url);
            else
                System.Diagnostics.Process.Start(this.Executable, url);
        }


       
    }


}
