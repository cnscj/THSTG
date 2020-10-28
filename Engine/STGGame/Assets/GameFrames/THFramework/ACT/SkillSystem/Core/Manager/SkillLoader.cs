using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SkillLoaderContext
    {
        public string loadType;
        public string skillName;
        public string assetKey;
    }

    public class SkillLoader
    {
        public virtual void LoadEffect(SkillLoaderContext context, Action<GameObject[]> callback)
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
}