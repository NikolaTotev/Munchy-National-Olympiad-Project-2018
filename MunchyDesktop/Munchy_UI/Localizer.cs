using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Threading;

namespace Munchy_UI
{
    public static class Localizer
    {
        /// <summary>  
        /// Get current culture info name base on previously saved setting if any,  
        /// otherwise get from OS language  
        /// </summary>  
        /// <returns></returns>  
        public static string GetCurrentCultureName()
        {
            var cultureName = CultureInfo.CurrentUICulture.Name;
            return cultureName;
        }
        /// <summary>  
        /// Set language based on previously save language setting,  
        /// otherwise set to OS lanaguage  
        /// </summary>  
        public static void SetDefaultLanguage(FrameworkElement element)
        {
            SetLanguageResourceDictionary(element, GetLocXAMLFilePath(GetCurrentCultureName()));
        }
        /// <summary>  
        /// Dynamically load a Localization ResourceDictionary from a file  
        /// </summary>  
        /// <param name="localeCode">The ISO locale string, e.g. en-US, bg-BG, etc.</param>
        public static void SwitchLanguage(FrameworkElement element, string localeCode)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(localeCode);
            SetLanguageResourceDictionary(element, GetLocXAMLFilePath(localeCode));
        }
        /// <summary>  
        /// Returns the path to the ResourceDictionary file based on the language character string.  
        /// </summary>  
        /// <param name="localeCode"></param>  
        /// <returns></returns>  
        public static string GetLocXAMLFilePath(string localeCode)
        {
            string locXamlFile = "MainWindow." + localeCode + ".xaml";
            string directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return Path.Combine(directory, "i18n", locXamlFile);
        }
        /// <summary>  
        /// Sets or replaces the ResourceDictionary by dynamically loading  
        /// a Localization ResourceDictionary from the file path passed in.  
        /// </summary>  
        /// <param name="resourceFile"></param>  
        private static void SetLanguageResourceDictionary(FrameworkElement element, String resourceFile)
        {
            if (File.Exists(resourceFile))
            {
                // Read in ResourceDictionary File  
                var languageDictionary = new ResourceDictionary();
                languageDictionary.Source = new Uri(resourceFile);
                // Remove any previous Localization dictionaries loaded  
                int langDictId = -1;
                for (int i = 0; i < element.Resources.MergedDictionaries.Count; i++)
                {
                    var md = element.Resources.MergedDictionaries[i];
                    // Make sure your Localization ResourceDictionarys have the ResourceDictionaryName  
                    // key and that it is set to a value starting with "Loc-".  
                    if (md.Contains("ResourceDictionaryName"))
                    {
                        if (md["ResourceDictionaryName"].ToString().StartsWith("Loc-"))
                        {
                            langDictId = i;
                            break;
                        }
                    }
                }
                if (langDictId == -1)
                {
                    // Add in newly loaded Resource Dictionary  
                    element.Resources.MergedDictionaries.Add(languageDictionary);
                }
                else
                {
                    // Replace the current langage dictionary with the new one  
                    element.Resources.MergedDictionaries[langDictId] = languageDictionary;
                }
            }
            else
            {
                MessageBox.Show("'" + resourceFile + "' not found.");
            }
        }
    }
}
