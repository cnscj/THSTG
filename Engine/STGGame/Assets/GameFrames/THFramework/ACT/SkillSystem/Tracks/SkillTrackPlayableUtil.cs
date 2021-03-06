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

            var skillTimelineSequence = new SkillTimelineSequence();
            skillTimelineSequence.name = playable.name;
            skillTimelineSequence.type = "_playable_";

            var skillTimelineData = new SkillTimelineData();
            
            foreach (var pb in bindings)
            {
                var track = pb.sourceObject as TrackAsset;
                if (track != null)
                {
                    SkillTimelineSequence skillTimelineTrack = new SkillTimelineSequence();
                    skillTimelineTrack.name = track.name;
                    skillTimelineTrack.type = "_track_";

                    foreach (TimelineClip clip in track.GetClips())
                    {
                        if(clip.asset is SkillTriggerPlayableClip)
                        {
                            var triggerClip = clip.asset as SkillTriggerPlayableClip;
                            var skillTimelineClip = new SkillTimelineClip();

                            skillTimelineClip.name = triggerClip.name;
                            skillTimelineClip.type = triggerClip.type;
                            skillTimelineClip.args = triggerClip.args;

                            skillTimelineClip.startTime = clip.start;
                            skillTimelineClip.durationTime = clip.duration;

                            skillTimelineTrack.AddClip(skillTimelineClip);
                        }
                    }
                    skillTimelineTrack.Refresh();
                    skillTimelineSequence.AddClip(skillTimelineTrack);
                }
            }
            skillTimelineSequence.Refresh();

            skillTimelineData.sequences = new SkillTimelineSequence[] { skillTimelineSequence };
            SkillTimelineData.SaveToFile(skillTimelineData, savePath);  //FIXME:貌似不支持多余3层的嵌套
        }

        public static void CreatePlayableByJson(string jsonPath, string playablePath)
        {
            if (string.IsNullOrEmpty(jsonPath))
                return;

            if (string.IsNullOrEmpty(playablePath))
                return;

            var timelineData = SkillTimelineData.LoadFromFile(jsonPath);
            if (timelineData != null)
            {
                var timelineAsset = TimelineAsset.CreateInstance<TimelineAsset>();
                AssetDatabase.CreateAsset(timelineAsset, playablePath);

                var playableInfo = timelineData.sequences[0]; 
                foreach (var trackInfo in playableInfo.clips)
                {
                    var trackSequence = (SkillTimelineSequence)trackInfo;//FIXME:这里转换存在问题,应该是与上面的问题一致
                    var timelineTrack = timelineAsset.CreateTrack<SkillTriggerPlayableTrack>(null, trackInfo.name);
                    foreach (var clipInfo in trackSequence.clips)
                    {
                        var timelineClip = timelineTrack.CreateClip<SkillTriggerPlayableClip>();
                        var triggerClip = timelineClip.asset as SkillTriggerPlayableClip;

                        timelineClip.displayName = clipInfo.name;
                        triggerClip.type = clipInfo.type;
                        triggerClip.name = clipInfo.name;
                        triggerClip.args = clipInfo.args;

                        timelineClip.start = clipInfo.startTime;
                        timelineClip.duration = clipInfo.durationTime;

                    }
                }
                AssetDatabase.SaveAssets();
            }

        }
    }
#endif




}
