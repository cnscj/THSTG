using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;
using Object = UnityEngine.Object;

namespace THGame
{
    public class SkillFactory
    {

    }

    public class SkillLoader
    {
        //音效加载器
        //特效加载器
        //动作加载器

        public virtual void LoadEffect(string assetKey,Action<Object> callback)
        {

        }

        public virtual void LoadAudio(string assetKey, Action<Object> callback)
        {

        }

        public virtual void LoadAction(string assetKey, Action<Object> callback)
        {

        }
    }

    public class SkillManager : Singleton<SkillManager>
    {
        private SkillFactory _factory;
        private SkillLoader _loader;

        public SkillFactory GetFactory()
        {
            return default;
        }

        public SkillLoader GetLoader()
        {
            return default;
        }
    }
}
