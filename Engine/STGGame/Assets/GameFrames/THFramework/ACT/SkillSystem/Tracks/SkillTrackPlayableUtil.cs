using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using THGame.Skill;
using System;
using XLibrary;

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

            List<SkillTimelineAsset> sequence = new List<SkillTimelineAsset>();
            foreach (var pb in bindings)
            {
                var track = pb.sourceObject as TrackAsset;
                if (track != null)
                {
                    foreach (TimelineClip clip in track.GetClips())
                    {
                        
                        if(clip.asset is SkillTriggerPlayableClip)
                        {
                            var triggerClip = clip.asset as SkillTriggerPlayableClip;
                            var asset = new SkillTimelineAsset();

                            asset.type = triggerClip.type;
                            asset.args = triggerClip.args;

                            asset.startTime = (float)clip.start;
                            asset.durationTime = (float)clip.duration;

                            sequence.Add(asset);
                        }

                        //Debug.Log("name:" + clip.displayName+ "开始:" + clip.start + "时间:" + clip.duration);
                    }
                }
            }

            var timelineData = new SkillTimelineData();
            timelineData.sequence = sequence.ToArray();

            SkillTimelineData.SaveToFile(timelineData,savePath);
        }

        public static void CreatePlayableByJson(string jsonPath, string playablePath)
        {
            if (string.IsNullOrEmpty(jsonPath))
                return;

            if (string.IsNullOrEmpty(playablePath))
                return;

            var timelineData = SkillTimelineData.LoadFromFile(jsonPath);

            //TODO:
        }
    }
#endif




}
