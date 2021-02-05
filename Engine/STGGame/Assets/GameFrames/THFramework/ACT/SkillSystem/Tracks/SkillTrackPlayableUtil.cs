using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using THGame.Skill;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace THGame
{
    public static class SkillTrackPlayableUtil
    {
#if UNITY_EDITOR
        public static void ExportPlayableToJson(string playblePath, string savePath)
        {
            if (string.IsNullOrEmpty(playblePath))
                return;

            if (string.IsNullOrEmpty(savePath))
                return;

            var playable = AssetDatabase.LoadAssetAtPath<TimelineAsset>(playblePath);
            var bindings = playable.outputs;

            foreach (var pb in bindings)
            {
                var track = pb.sourceObject as TrackAsset;
                if (track != null)
                {
                    foreach (TimelineClip clip in track.GetClips())
                    {
                        //TODO:
                        if(clip.asset is SkillTriggerPlayableClip)
                        {
                            var newClip = clip.asset as SkillTriggerPlayableClip;
                            Debug.Log(newClip.type);
                        }
                        
                        Debug.Log("name:" + clip.displayName + "时间:" + clip.duration);
                    }
                }
            }

        }

        public static void CreatePlayableByJson(string jsonPath, string playablePath)
        {
            if (string.IsNullOrEmpty(jsonPath))
                return;

            if (string.IsNullOrEmpty(playablePath))
                return;


        }
    }
#endif




}
