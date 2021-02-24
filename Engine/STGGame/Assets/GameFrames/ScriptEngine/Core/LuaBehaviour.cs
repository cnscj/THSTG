/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using UnityEngine;
using XLua;
using System;

namespace SEGame
{
    [System.Serializable]
    public class LuaInjection
    {
        public string name;
        public GameObject value;
    }

    [LuaCallCSharp]
    public class LuaBehaviour : MonoBehaviour
    {
        public TextAsset luaScript;
        public LuaInjection[] injections;

        internal static float lastGCTime = 0;
        internal const float gcInterval = 1;    //1 second 

        private Action luaStart;
        private Action luaUpdate;
        private Action luaOnDestroy;

        private LuaTable scriptEnv;

        void Create(LuaTable luaTable, LuaInjection[] injections)
        {
            Release();

            scriptEnv = luaTable;
            Action luaAwake = luaTable.Get<Action>("awake");
            luaTable.Get("start", out luaStart);
            luaTable.Get("update", out luaUpdate);
            luaTable.Get("onDestroy", out luaOnDestroy);

            if (injections != null && injections.Length > 0)
            {
                foreach (var injection in injections)
                {
                    luaTable.Set(injection.name, injection.value);
                }
            }

            if (luaAwake != null)
            {
                luaAwake();
            }
        }

        void Release()
        {
            if (luaOnDestroy != null)
            {
                luaOnDestroy();
            }
            luaOnDestroy = null;
            luaUpdate = null;
            luaStart = null;
            scriptEnv?.Dispose();
            injections = null;

            scriptEnv = null;
        }

        void Awake()
        {
            LoadFromTextAsset(this.luaScript,this.injections);
        }

        void LoadFromString(string context, LuaInjection[] injection)
        {
            LuaTable luaTable = GetLuaEnv().NewTable();
            // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
            LuaTable meta = GetLuaEnv().NewTable();
            meta.Set("__index", GetLuaEnv().Global);
            luaTable.SetMetaTable(meta);
            meta.Dispose();

            luaTable.Set("self", this);

            GetLuaEnv().DoString(context, "chunk", scriptEnv);

            Create(luaTable, injection);
        }


        void LoadFromTextAsset(TextAsset textAsset, LuaInjection[] injection)
        {
            if (!textAsset) return;
            LoadFromString(textAsset.text, injection);
        }

        // Use this for initialization
        void Start()
        {
            if (luaStart != null)
            {
                luaStart();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (luaUpdate != null)
            {
                luaUpdate();
            }
            if (Time.time - LuaBehaviour.lastGCTime > gcInterval)
            {
                GetLuaEnv().Tick();
                LuaBehaviour.lastGCTime = Time.time;
            }
        }

        void OnDestroy()
        {
            Release();
        }

        //all lua behaviour shared one luaenv only!
        LuaEnv GetLuaEnv()
        {
            return LuaEngine.GetInstance().LuaEnv;
        }
    }
}