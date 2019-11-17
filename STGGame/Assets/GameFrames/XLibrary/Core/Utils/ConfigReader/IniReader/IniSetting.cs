using System;
using System.IO;
using UnityEngine;

namespace XLibrary
{
    public static class IniSetting
    {
        private static string mINIFileName = Application.dataPath + "/../config.ini";

        public enum SectionName
        {
            config,//浏览
            config2,//共享内存
        }

        public static bool GetBool(string Key, SectionName section = SectionName.config, bool defaultValue = false)
        {
            bool value = defaultValue;
            string sectionName = section.ToString();

            Action<IniParser> action =
                (ini) =>
                {
                    if (!ini.IsKeyExists(sectionName, Key))
                        ini.WriteValue(sectionName, Key, defaultValue);
                    value = ini.ReadValue(sectionName, Key, defaultValue);
                };
            OpenAndReadINI(mINIFileName, action);
            Debug.LogWarning(Key + "  " + value);
            return value;
        }


        public static string GetString(string Key, SectionName section = SectionName.config, string defaultValue = "")
        {
            string value = defaultValue;
            string sectionName = section.ToString();

            Action<IniParser> action =
                (ini) =>
                {
                    if (!ini.IsKeyExists(sectionName, Key))
                        ini.WriteValue(sectionName, Key, defaultValue);
                    value = ini.ReadValue(sectionName, Key, defaultValue);
                };

            OpenAndReadINI(mINIFileName, action);
            return value;
        }

        public static float GetFloat(string Key, SectionName section = SectionName.config, float defaultValue = 0)
        {
            float value = defaultValue;
            string sectionName = section.ToString();

            Action<IniParser> action =
                (ini) =>
                {
                    if (!ini.IsKeyExists(sectionName, Key))
                        ini.WriteValue(sectionName, Key, defaultValue);
                    value = (float)ini.ReadValue(sectionName, Key, defaultValue);
                };
            OpenAndReadINI(mINIFileName, action);
            Debug.LogWarning(Key + "  " + value);
            return value;

        }

        public static void WriteString(string Key, SectionName section = SectionName.config, string defaultValue = "")
        {
            string sectionName = section.ToString();
            Action<IniParser> action =
                (ini) =>
                {
                    ini.WriteValue(sectionName, Key, defaultValue);
                };
            OpenAndReadINI(mINIFileName, action);
        }

        public static void WriteBool(string Key, SectionName section = SectionName.config, bool defaultValue = false)
        {
            string sectionName = section.ToString();
            Action<IniParser> action =
                (ini) =>
                {
                    ini.WriteValue(sectionName, Key, defaultValue);
                };
            OpenAndReadINI(mINIFileName, action);
        }
        public static void WriteFloat(string Key, SectionName section = SectionName.config, float defaultValue = 0)
        {
            string sectionName = section.ToString();
            Action<IniParser> action =
                (ini) =>
                {
                    ini.WriteValue(sectionName, Key, defaultValue);
                };
            OpenAndReadINI(mINIFileName, action);
        }

        public static int GetInt(string Key, SectionName section = SectionName.config, int defaultValue = 0)
        {
            int value = defaultValue;
            string sectionName = section.ToString();

            Action<IniParser> action =
                (ini) =>
                {
                    if (!ini.IsKeyExists(sectionName, Key))
                        ini.WriteValue(sectionName, Key, defaultValue);
                    value = ini.ReadValue(sectionName, Key, defaultValue);
                };
            OpenAndReadINI(mINIFileName, action);
            Debug.LogWarning(Key + "  " + value);
            return value;
        }

        static void OpenAndReadINI(string path, Action<IniParser> actionRead)
        {
            try
            {
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                    Debug.LogWarning("Create Success:" + path);
                }
                //else
                {
                    IniParser iniParser = new IniParser();
                    iniParser.Open(path);

                    actionRead(iniParser);

                    iniParser.Close();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("INIFile Not Found Or Create:\r\n" + path + "\r\n" + e);
            }
        }
    }
}
