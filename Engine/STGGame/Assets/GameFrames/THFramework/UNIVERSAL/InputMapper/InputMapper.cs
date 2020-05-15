using UnityEngine;
using System.Collections.Generic;
using XLibrary.Package;
using System;
using XLibrary;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace THGame
{
    public class InputMapper : MonoSingleton<InputMapper>
    {
        public static readonly string SECTION_KEY = "KEYBOARD";
        //记录按键状态
        [System.Serializable]
        public class KeyPair
        {
            public int behaviour;
            public List<KeyCode> keycodes = new List<KeyCode>();
        }
        enum EKeyStatus
        {
            At = 2 ^ 0,
            Up = 2 ^ 1,
            Down = 2 ^ 2,
        }
        public List<KeyPair> keyList = new List<KeyPair>();

        private Dictionary<int, short> m_keyStatus = new Dictionary<int, short>();                         //按键状态
        private Dictionary<int, bool> m_keyResult = new Dictionary<int, bool>();

        public bool IsAtBehaviour(int behaviour)
        {
            if (m_keyStatus.ContainsKey(behaviour))
            {
                short status = m_keyStatus[behaviour];
                return ((status & (int)EKeyStatus.At) > 0);
            }
            return false;
        }

        public bool IsBehaving(int behaviour)
        {
            if (m_keyStatus.ContainsKey(behaviour))
            {
                short status = m_keyStatus[behaviour];
                return ((status & (int)EKeyStatus.Down) > 0);
            }
            return false;
        }

        public bool IsBehaved(int behaviour)
        {
            if (m_keyStatus.ContainsKey(behaviour))
            {
                short status = m_keyStatus[behaviour];
                return ((status & (int)EKeyStatus.Up) > 0);
            }
            return false;
        }


        public bool HasBindKey(KeyCode code)
        {
            //是否存在按键冲突
            foreach (var pair in keyList)
            {
                if (pair.keycodes.Contains(code))
                {
                    return true;
                }
            }
            return false;
        }

        public bool BindKey(int behaviour, KeyCode code, bool isReplace = false)
        {
            //是否存在按键冲突
            foreach (var pair in keyList)
            {
                if (pair.keycodes.Contains(code))
                {
                    if(isReplace)
                    {
                        pair.keycodes.Remove(code);
                    }
                    else
                    {
                        return false;
                    }
                    
                }
            }

            //没有按键冲突
            foreach(var pair in keyList)
            {
                if (pair.behaviour == behaviour)
                {
                    pair.keycodes.Add(code);
                    return true;
                }
            }

            KeyPair keyPair = new KeyPair();
            keyPair.behaviour = behaviour;
            keyPair.keycodes = new List<KeyCode>();
            keyPair.keycodes.Add(code);
            keyList.Add(keyPair);

            return true;
        }

        public void UnbindKey(int behaviour, KeyCode code)
        {
            foreach (var pair in keyList)
            {
                if (pair.behaviour == behaviour)
                {
                    if (pair.keycodes.Contains(code))
                    {
                        pair.keycodes.Remove(code);
                    }
                }
            }
        }

        public void UnbindKeys(int behaviour)
        {
            foreach (var pair in keyList)
            {
                if (pair.behaviour == behaviour)
                {
                    pair.keycodes.Clear();
                }
            }
        }

        public void UnBindAllKeys()
        {
            keyList.Clear();
        }

        protected void Update()
        {
            //把所有的按键记录到输入组件中
            foreach (var keyPair in keyList)
            {
                short status = 0x0;
                for (int i = 0; i < 3; i++) m_keyResult[i] = false;
                foreach (var keyCode in keyPair.keycodes)
                {
                    m_keyResult[0] = m_keyResult[0] | Input.GetKey(keyCode);
                    m_keyResult[1] = m_keyResult[1] | Input.GetKeyDown(keyCode);
                    m_keyResult[2] = m_keyResult[2] | Input.GetKeyUp(keyCode);
                }
                status = m_keyResult[0] ? (short)(status | ((int)EKeyStatus.At)): status;
                status = m_keyResult[1] ? (short)(status | ((int)EKeyStatus.Down)) : status;
                status = m_keyResult[2] ? (short)(status | ((int)EKeyStatus.Up)) : status;
                m_keyStatus[keyPair.behaviour] = status;
            }
        }

        public void LoadFromFile(string path)
        {
            IniParser parser = new IniParser();
            parser.Open(path);

            Dictionary<string, string> dectionary = null;
            parser.ReadAllValues(SECTION_KEY, dectionary);
            parser.Close();

            if (dectionary != null)
            {
                Dictionary<int, List<KeyCode>> tmpMap = new Dictionary<int, List<KeyCode>>();
                foreach (var pair in dectionary)
                {
                    var keyCodeStr = pair.Key;
                    var keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), keyCodeStr);
                    var keyValue = int.Parse(pair.Value);

                    List<KeyCode> codeList = null;
                    if (!tmpMap.TryGetValue(keyValue, out codeList))
                    {
                        codeList = new List<KeyCode>();
                        tmpMap.Add(keyValue, codeList);
                    }
                    codeList.Add(keyCode);
                }

                keyList.Clear();
                foreach (var pair in tmpMap)
                {
                    KeyPair keyPair = new KeyPair();
                    keyPair.behaviour = pair.Key;
                    keyPair.keycodes = pair.Value;

                    keyList.Add(keyPair);
                }
            }
        }

        public void SaveToFile(string path)
        {
            IniParser parser = new IniParser();
            parser.Open(path);

            foreach (var pair in keyList)
            {
                var keyValue = pair.behaviour;
                foreach (var keycode in pair.keycodes)
                {
                    var keyCodeStr = Enum.GetName(typeof(KeyCode), keycode);
                    parser.WriteValue(SECTION_KEY, keyCodeStr, keyValue);
                }
            }
            parser.Close();
        }

#if UNITY_EDITOR
        [ContextMenu("LoadFromFile")]
        private void LoadFromFileEdiotr()
        {
            //顺便加载到编辑器
            var path = EditorUtility.OpenFilePanel(
                "Overwrite with INI",
                "",
                "ini");
            if (!string.IsNullOrEmpty(path))
            {
                LoadFromFile(path);
            }
        }

        [ContextMenu("SaveToFile")]
        private void SaveToFileEdiotr()
        {
            var path = EditorUtility.SaveFilePanel(
            "Save file as INI",
            "",
            "keyboard",
            "ini");
            if (!string.IsNullOrEmpty(path))
            {
                SaveToFile(path);
            }
        }
#endif
    }

}
