using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using XLua;

namespace SEGame
{
    public class LuaBehaviour : MonoBehaviour
    {
        //因为值类型泛型的生成在IL2CPP下会有JIT异常，所以值类型必须要注册
        private static Dictionary<Type, Type> wrapTypeDict = new Dictionary<Type, Type>()
        {
            {typeof(int), typeof(ObjWrap<int>)},
            {typeof(long), typeof(ObjWrap<long>)},
            {typeof(float), typeof(ObjWrap<float>)},
            {typeof(double), typeof(ObjWrap<double>)},
            {typeof(bool), typeof(ObjWrap<bool>)},
            {typeof(string), typeof(ObjWrap<string>)},
            {typeof(Vector2), typeof(ObjWrap<Vector2>)},
            {typeof(Vector3), typeof(ObjWrap<Vector3>)},
            {typeof(Vector4), typeof(ObjWrap<Vector4>)},
            {typeof(Color), typeof(ObjWrap<Color>)},
            {typeof(Rect), typeof(ObjWrap<Rect>)},
            {typeof(Bounds), typeof(ObjWrap<Bounds>)},

            {typeof(List<int>), typeof(ObjWrap<List<int>>)},
            {typeof(List<string>), typeof(ObjWrap<List<string>>)},
            {typeof(List<Vector3>), typeof(ObjWrap<List<Vector3>>)},
            {typeof(List<Color>), typeof(ObjWrap<List<Color>>)},
        };


        public static string ValueToJson(object value)
        {
            Type type = value.GetType();
            Type wrapType = typeof(ObjWrap<>).MakeGenericType(type);
            var target = Activator.CreateInstance(wrapType);
            ((IObjWrap)target).SetObj(value);
            return JsonUtility.ToJson(target);
        }

        public static object JsonToValue(string json, Type type)
        {
            if (wrapTypeDict.ContainsKey(type))
            {
                Type wrapType = wrapTypeDict[type];
                var target = JsonUtility.FromJson(json, wrapType);
                return ((IObjWrap)target).GetObj();
            }
            else if (type.IsValueType())
            {
                Debug.LogError($"msut register {type.FullName} in wrapTypeDict, because it is value type!");
                return null;
            }
            else
            {
                Type wrapType = typeof(ObjWrap<>).MakeGenericType(type);
                wrapTypeDict.Add(type, wrapType);
                var target = JsonUtility.FromJson(json, wrapType);
                return ((IObjWrap)target).GetObj();
            }
        }

        public interface IObjWrap
        {
            object GetObj();
            void SetObj(object obj);
        }

        public class ObjWrap<T> : IObjWrap
        {
            public T obj;

            public object GetObj()
            {
                return obj;
            }

            public void SetObj(object obj)
            {
                this.obj = (T)obj;
            }
        }

        [Serializable]
        public class SerializedObjValue
        {
            public string key;
            public UnityEngine.Object value;
        }

        [Serializable]
        public class SerializedValue
        {
            public string key;
            public string jsonStr;
        }

        [HideInInspector]
        public string LuaScriptPath;
        [SerializeField]
        [HideInInspector]
        public List<SerializedObjValue> SerializedObjValues;
        [SerializeField]
        [HideInInspector]
        public List<SerializedValue> SerializedValues;

    
        public LuaTable LuaInstance
        {
            get{ return mLuaInstance; }
        }

        private LuaTable mLuaInstance;

        private Action<LuaTable> onAwakeFunc;
        private Action<LuaTable> onStartFunc;

        private Action<LuaTable> onEnableFunc;
        private Action<LuaTable> onDisableFunc;
        private Action<LuaTable> onDestroyFunc;

        private LuaUpdateAssistant onUpdateAssistant;
        private LuaFixedUpdateAssistant onFixedUpdateAssistant;
        private LuaLateUpdateAssistant onLateUpdateAssistant;

        public void SetTable(LuaTable luaInstance)
        {
            Clear();

            mLuaInstance = luaInstance;

            if (LuaInstance != null)
            {
                //注入自己
                LuaInstance.Set("owner", this);
                LuaInstance.Set("gameObject", gameObject);

                //其它参数注入
                LuaTable defineTable;
                LuaInstance.Get("_defineList", out defineTable);
                if (defineTable != null)
                {
                    Dictionary<string, Type> infoDict = new Dictionary<string, Type>();
                    string infoName;
                    Type infoType;
                    defineTable.ForEach<int, LuaTable>((index, infoTable) =>
                    {
                        infoTable.Get("name", out infoName);
                        infoTable.Get("type", out infoType);
                        infoDict.Add(infoName, infoType);
                    });
                    //注入object类型对象
                    foreach (var serializedValue in SerializedObjValues)
                    {
                        //看了下set的代码，没有注册任何object类型的pushfunc,所以最后调用的都是pushany，
                        //不用担心类型错误问题
                        if (infoDict.ContainsKey(serializedValue.key) && serializedValue.value != null)
                        {
                            LuaInstance.Set(serializedValue.key, serializedValue.value);
                        }
                    }
                    //注入其他类型
                    foreach (var serializedValue in SerializedValues)
                    {
                        if (infoDict.ContainsKey(serializedValue.key))
                        {
                            LuaInstance.Set(serializedValue.key, JsonToValue(serializedValue.jsonStr, infoDict[serializedValue.key]));
                        }
                    }
                }

                //回调注册
                var newWith = LuaInstance.Get<Action<LuaTable>>("newWith");
                newWith?.Invoke(LuaInstance);

                onAwakeFunc = LuaInstance.Get<Action<LuaTable>>("awake");
                onStartFunc = LuaInstance.Get<Action<LuaTable>>("start");
                onEnableFunc = LuaInstance.Get<Action<LuaTable>>("onEnable");
                onDisableFunc = LuaInstance.Get<Action<LuaTable>>("onDisable");
                onDestroyFunc = LuaInstance.Get<Action<LuaTable>>("onDestroy");

                var onUpdateFunc = LuaInstance.Get<Action<LuaTable>>("update");
                if (onUpdateFunc != null)
                {
                    onUpdateAssistant = gameObject.AddComponent<LuaUpdateAssistant>();
                    onUpdateAssistant.luaBehaviour = this;
                    onUpdateAssistant.assistFunc = onUpdateFunc;
                }

                var onFixedUpdateFunc = LuaInstance.Get<Action<LuaTable>>("fixedUpdate");
                if (onFixedUpdateFunc != null)
                {
                    onFixedUpdateAssistant = gameObject.AddComponent<LuaFixedUpdateAssistant>();
                    onFixedUpdateAssistant.luaBehaviour = this;
                    onFixedUpdateAssistant.assistFunc = onFixedUpdateFunc;
                }

                var onLateUpdateFunc = LuaInstance.Get<Action<LuaTable>>("lateUpdate");
                if (onLateUpdateFunc != null)
                {
                    onLateUpdateAssistant = gameObject.AddComponent<LuaLateUpdateAssistant>();
                    onLateUpdateAssistant.luaBehaviour = this;
                    onLateUpdateAssistant.assistFunc = onFixedUpdateFunc;
                }

            }
        }

        public void SetContent(string scriptContent)
        {
            if (string.IsNullOrEmpty(scriptContent))
                return;

            var rets = GetLuaEnv().DoString(scriptContent);
            var luaClass = (LuaTable)rets[0];

            //下面步骤是cls的new过程
            var clsNewFunc = luaClass.Get<Func<LuaTable,LuaTable>>("new");
            mLuaInstance = clsNewFunc.Invoke(luaClass); //这里会执行init,

            SetTable(mLuaInstance);
        }

        public void SetScript(string luaScriptPath)
        {
            if (string.IsNullOrEmpty(luaScriptPath))
                return;

            SetContent($"return require(\"{luaScriptPath}\")");
        }

        LuaEnv GetLuaEnv()
        {
            return LuaEngine.GetInstance().LuaEnv;
        }

        void Awake()
        {
            SetScript(LuaScriptPath);
            onAwakeFunc?.Invoke(LuaInstance);
        }

        void Start()
        {
            onStartFunc?.Invoke(LuaInstance);
        }

        void OnEnable()
        {
            if (onUpdateAssistant != null) onUpdateAssistant.enabled = true;
            if (onFixedUpdateAssistant != null) onFixedUpdateAssistant.enabled = true;
            if (onLateUpdateAssistant != null) onLateUpdateAssistant.enabled = true;

            onEnableFunc?.Invoke(LuaInstance);      
        }

        void OnDisable()
        {
            if (onUpdateAssistant != null) onUpdateAssistant.enabled = false;
            if (onFixedUpdateAssistant != null) onFixedUpdateAssistant.enabled = false;
            if (onLateUpdateAssistant != null) onLateUpdateAssistant.enabled = false;

            onDisableFunc?.Invoke(LuaInstance);
        }

        void OnDestroy()
        {
            onDestroyFunc?.Invoke(LuaInstance);

            Clear();
        }

        void Clear()
        {
            if (onUpdateAssistant != null) GameObject.Destroy(onUpdateAssistant);
            if (onFixedUpdateAssistant != null) GameObject.Destroy(onFixedUpdateAssistant);
            if (onLateUpdateAssistant != null) GameObject.Destroy(onLateUpdateAssistant);

            if (LuaInstance != null)
            {
                var delWith = LuaInstance.Get<Action<LuaTable>>("delWith");
                delWith?.Invoke(LuaInstance);

                LuaInstance.Set("owner", false);
                LuaInstance.Set("gameObject", false);
            }

            onAwakeFunc = null;
            onStartFunc = null;
            onUpdateAssistant = null;
            onFixedUpdateAssistant = null;
            onLateUpdateAssistant = null;
            onEnableFunc = null;
            onDisableFunc = null;

            mLuaInstance = null;
        }
    }

}