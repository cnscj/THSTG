using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using FairyGUI.Utils;
using UnityEngine;
using XLibrary.Package;

/*
 * GComponent 1615行插入 THGame.UI.UIPropertyManager.GetInstance().RegisterGObject(this,key,value);
 * 
 * */
namespace THGame.UI
{
    public class UIPropertyManager : Singleton<UIPropertyManager>
    {
        public class CustomData
        {
            private Dictionary<string, string> m_propertys;

            public void AddProperty(string key, string value, bool isReplace = true)
            {
                var dict = GetPropertys();
                if (dict.ContainsKey(key))
                {
                    if(!isReplace)
                    {
                        return;
                    }
                }
                dict[key] = value;
            }

            public string GetProperty(string key)
            {
                if (m_propertys == null)
                    return string.Empty;

                if (m_propertys.TryGetValue(key,out var value))
                {
                    return value;
                }

                return string.Empty;
            }

            public void Clear()
            {
                m_propertys?.Clear();
            }

            private Dictionary<string, string> GetPropertys()
            {
                m_propertys = m_propertys ?? new Dictionary<string, string>();
                return m_propertys;
            }
        }
        //
        private Dictionary<string, CustomData> m_masterDict;

        public bool IsEnabled { get; set; }
        public bool IsUseId { get; set; }
        public bool IsRemoveFree { get; set; }

        public UIPropertyManager()
        {
            IsEnabled = true;
            IsRemoveFree = true;
        }
        public void RegisterGObject(GObject gObj)
        {
            if(!IsEnabled)
                return;

            if (gObj == null)
                return;

            var masterKey = GetMasterKey(gObj);
            var masterDict = GetMasterDict();
            if (!masterDict.ContainsKey(masterKey))
            {
                gObj.onAddedToStage.Add(OnAddedToStage);
                if (IsRemoveFree) gObj.onRemovedFromStage.Add(OnRemovedFromStage);
                masterDict[masterKey] = new CustomData();
            }
            return;
        }

        public void ParseGObject(GObject gObj)
        {
            if (!IsEnabled)
                return;

            if (gObj == null)
                return;

            var parentGobj = gObj.parent;
            if (parentGobj == null)
                return;

            var masterKey = GetMasterKey(gObj);

            PackageItem contentItem = parentGobj.packageItem;
            ByteBuffer buffer = contentItem.rawData;
            //就在父类里,但是取不到
            buffer.Seek(0, 2);
            int childCount = buffer.ReadShort();
            buffer.Seek(0, 2);
            buffer.Skip(2);
            for (int i = 0; i < childCount; i++)
            {
                int nextPos = buffer.ReadShort();
                nextPos += buffer.position;
                int beginPos = buffer.position;

                //TODO:
                //buffer.Skip(2);

                buffer.Seek(beginPos, 4);
                buffer.Skip(2);
                //
                if (buffer.version >= 2)
                {
                    int cnt = buffer.ReadShort();
                    for (int j = 0; j < cnt; j++)
                    {
                        string target = buffer.ReadS();
                        int propertyId = buffer.ReadShort();
                        string value = buffer.ReadS();
                        GObject obj = parentGobj.GetChildByPath(target);
                        if (obj == null)
                        {
                            AddProperty(masterKey, target, value);
                        }
                    }
                }
            }
        }

        public void RegisterGObject(GObject gObj, string key, string value)
        {
            if (!IsEnabled)
                return;

            if (gObj == null)
                return;

            var masterKey = GetMasterKey(gObj);
            var masterDict = GetMasterDict();
            if (!masterDict.ContainsKey(masterKey))
            {
                if (IsRemoveFree) gObj.onRemovedFromStage.Add(OnRemovedFromStage);
            }
            AddProperty(masterKey, key, value);
        }

        public void AddProperty(string master, string key, string value, bool isReplace = true)
        {
            if (string.IsNullOrEmpty(master))
                return;

            if (string.IsNullOrEmpty(key))
                return;

            var masterDict = GetMasterDict();
            CustomData customData = null;
            if (!masterDict.TryGetValue(master, out customData))
            {
                customData = new CustomData();
                masterDict[master] = customData;
            }

            customData.AddProperty(key, value, isReplace);
        }

        public string GetProperty(string master, string key)
        {
            if (string.IsNullOrEmpty(master))
                return string.Empty;

            if (string.IsNullOrEmpty(key))
                return string.Empty;

            if (m_masterDict == null)
                return string.Empty;

            if (m_masterDict.TryGetValue(master, out var customData))
            {
                return customData.GetProperty(key);
            }

            return string.Empty;
        }

        public string GetProperty(GObject gObj, string key)
        {
            var masterKey = GetMasterKey(gObj);
            return GetProperty(masterKey, key);
        }

        public void RemoveMaster(string master)
        {
            if (string.IsNullOrEmpty(master))
                return;

            if (m_masterDict == null)
                return;

            if (m_masterDict.TryGetValue(master, out var customData))
            {
                m_masterDict.Remove(master);
            }
        }
        public void RemoveMaster(GObject gObj)
        {
            var masterKey = GetMasterKey(gObj);
            RemoveMaster(masterKey);
        }

        public void Clear()
        {
            m_masterDict?.Clear();
        }

        private void OnAddedToStage(EventContext context)
        {
            var gObj = context.sender as GObject;
            if (gObj == null)
                return;

            var masterKey = GetMasterKey(gObj);
            ParseGObject(gObj);
        }

        private void OnRemovedFromStage(EventContext context)
        {
            var gObj = context.sender as GObject;
            if (gObj == null)
                return;

            var masterKey = GetMasterKey(gObj);
            RemoveMaster(masterKey);
        }

        private string GetMasterKey(GObject gObj)
        {
            if (gObj == null)
                return string.Empty;

            if (IsUseId) return string.Format("{0}", gObj.id);
            else return string.Format("{0}", gObj.GetHashCode());
        }

        private Dictionary<string, CustomData> GetMasterDict()
        {
            m_masterDict = m_masterDict ?? new Dictionary<string, CustomData>();
            return m_masterDict;
        }
    }

}
