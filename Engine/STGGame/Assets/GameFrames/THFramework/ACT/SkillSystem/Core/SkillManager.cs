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
        public virtual void PlayEffect()
        {

        }

        public virtual void PlayAudio()
        {

        }

        public virtual void PlayAction()
        {

        }
    }

    public class SkillLoader
    {
        //音效加载器
        //特效加载器
        //动作加载器
        //TODO:简单的加载器可以根据动作名,自动加载对应资源
        public virtual void LoadEffect(SkillLoaderContext context,Action<GameObject[]> callback)
        {

        }

        public virtual void LoadAudio(SkillLoaderContext context, Action<AudioClip[]> callback)
        {

        }

        public virtual void LoadAction(SkillLoaderContext context, Action<AnimationClip[]> callback)
        {

        }

        public virtual void LoadConfig(SkillLoaderContext context, Action<SkillData> callback)
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
