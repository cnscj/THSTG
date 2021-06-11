/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    public class LuaBehaviourx : MonoBehaviour
    {
        public TextAsset luaScript;
        public LuaInjection[] injections;

        internal static float lastGCTime = 0;
        internal const float gcInterval = 1;    //1 second 

        private Action luaStart;
        private Action luaUpdate;
        private Action luaOnDestroy;

        private LuaTable scriptEnv;


        public void Set(LuaTable clsIns)
        {
            scriptEnv = GetLuaEnv().NewTable();

            // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
            LuaTable meta = GetLuaEnv().NewTable();
            meta.Set("__index", GetLuaEnv().Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            var luaClass = (LuaTable)clsIns;
            scriptEnv.Set("gameObject", this);

            var luaStart = luaClass.Get<Action<LuaTable>>("start");

            //scriptEnv.Get("start", out luaStart);
            scriptEnv.Get("update", out luaUpdate);
            scriptEnv.Get("onDestroy", out luaOnDestroy);
            if (luaStart != null)
            {
                luaStart(luaClass);
            }
        }

        void Awake()
        {
            if (!luaScript)
                return;

            scriptEnv = GetLuaEnv().NewTable();

            // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
            LuaTable meta = GetLuaEnv().NewTable();
            meta.Set("__index", GetLuaEnv().Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            scriptEnv.Set("gameObject", this);
            foreach (var injection in injections)
            {
                scriptEnv.Set(injection.name, injection.value);
            }

            var rets = GetLuaEnv().DoString(luaScript.text, "chunk", scriptEnv);
            var luaClass = (LuaTable)rets[0];

            var luaAwake = luaClass.Get<Action<LuaTable>>("awake");
            scriptEnv.Get("start", out luaStart);
            scriptEnv.Get("update", out luaUpdate);
            scriptEnv.Get("onDestroy", out luaOnDestroy);

            if (luaAwake != null)
            {
                luaAwake(luaClass);
            }
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
            if (Time.time - lastGCTime > gcInterval)
            {
                GetLuaEnv().Tick();
                lastGCTime = Time.time;
            }
        }

        void OnDestroy()
        {
            luaOnDestroy?.Invoke();
            luaOnDestroy = null;
            luaUpdate = null;
            luaStart = null;
            scriptEnv.Dispose();
            injections = null;
        }

        //all lua behaviour shared one luaenv only!
        LuaEnv GetLuaEnv()
        {
            return LuaEngine.GetInstance().LuaEnv;
        }
    }
}