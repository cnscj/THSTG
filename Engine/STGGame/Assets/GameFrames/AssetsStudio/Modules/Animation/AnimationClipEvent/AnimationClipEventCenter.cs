using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;
using Object = UnityEngine.Object;

namespace ASGame
{
    //管理动画系统的所有事件
    public class AnimationClipEventCenter : Singleton<AnimationClipEventCenter>
    {

        public AnimationEvent AddClipEvent(AnimationClip clip, float time, string funcName)
        {
            if (clip != null && !string.IsNullOrEmpty(funcName))
            {
                var evt = new AnimationEvent();
                evt.time = time;
                evt.functionName = funcName;
                clip.AddEvent(evt);

                return evt;
            }
            return default;
        }

        public void RemoveClipEvent(AnimationClip clip, float time)
        {
            if (clip != null)
            {
                var evts = clip.events;
                if (evts != null && evts.Length > 0)
                {
                    List<AnimationEvent> evtList = new List<AnimationEvent>();
                    foreach (var evt in evts)
                    {
                        if (Mathf.Approximately(evt.time, time))
                        {
                            continue;
                        }
                        evtList.Add(evt);
                    }
                    clip.events = evtList.ToArray();
                }

            }
        }

        ///////////////////////
        private string GetEventMapKey(AnimationClip clip,float time)
        {
            return string.Format("{0}_{1}", clip.GetHashCode(), time);
        }

    }
}
