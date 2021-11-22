using System;
using System.Configuration;

namespace MEIRMANOVA_HW7_DELEGATES
{
   public static class Configuration
    {
        public static string GetDirectory()
        {
            return ConfigurationManager.AppSettings.Get("Directory") ?? SetDirectory();
        }

        public static int GetWaitingInterval()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings.Get("WaitingInterval"), out int result))
                result = SetWaitingInterval();
            return result;
        }

        private static string SetDirectory()
        {
            while (true)
            {
                Console.WriteLine("Введите название папки для документов");
                string Directory = Console.ReadLine();
                if (Directory.Length == 0)
                {
                    Console.WriteLine("Наименование папки не может быть пустым!");
                    continue;
                }

                AddUpdateAppSettings("Directory", Directory);
                return Directory;
            }
        }

        private static int SetWaitingInterval()
        {
            while (true)
            {
                Console.WriteLine("Введите время ожидания документов (в млсек)");
                if (!int.TryParse(Console.ReadLine(), out int waitingInterval) || waitingInterval == 0)
                {
                    Console.WriteLine("Время ожидания не может быть меньше или равно 0");
                    continue;
                }
                AddUpdateAppSettings("WaitingInterval", waitingInterval.ToString());

                return waitingInterval;
            }

        }
        static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

    }
}
