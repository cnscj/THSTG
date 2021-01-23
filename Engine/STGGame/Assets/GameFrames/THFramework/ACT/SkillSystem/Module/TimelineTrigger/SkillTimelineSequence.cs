using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Collection;

namespace THGame
{
    public class SkillTimelineSequence : SkillTimelineTrack
    {
        public Action onStart;
        public Action onEnd;

        private SortedDictionary<int, HashSet<SkillTimelineTrack>> _scheduleTracks = new SortedDictionary<int, HashSet<SkillTimelineTrack>>();
        private MaxHeap<SkillTimelineTrack, int> _scheduleTracksEndTime = new MaxHeap<SkillTimelineTrack, int>();
        private HashSet<SkillTimelineTrack> _schedulingTracks = new HashSet<SkillTimelineTrack>();
        private Queue<SkillTimelineTrack> _scheduledTracks = new Queue<SkillTimelineTrack>();

        public SkillTimelineSequence(int startFrame, int durationTime):base(startFrame, durationTime) { }


        public void AddTrack(SkillTimelineTrack track)
        {
            if (track == null)
                return;

            HashSet<SkillTimelineTrack> trackSet;
            if (!_scheduleTracks.TryGetValue(track.StartTime, out trackSet))
            {
                trackSet = new HashSet<SkillTimelineTrack>();
                _scheduleTracks.Add(track.StartTime, trackSet);
            }
            trackSet.Add(track);

            _scheduleTracksEndTime.Add(track, track.EndTime);
            StartTime = _scheduleTracksEndTime.Max.Key.EndTime;
        }

        public void RemoveTrack(SkillTimelineTrack track)
        {
            if (track == null)
                return;

            if (_scheduleTracks.TryGetValue(track.StartTime, out var jobSet))
            {
                jobSet.Remove(track);
                if (jobSet.Count <= 0)
                {
                    _scheduleTracks.Remove(track.StartTime);
                }
            }

            _scheduleTracksEndTime.Remove(track);
            DurationTime = (_scheduleTracksEndTime.Count > 0 ? _scheduleTracksEndTime.Max.Key.EndTime : 0);
        }

        public void Clear()
        {
            _scheduleTracks.Clear();
            _scheduleTracksEndTime = new MaxHeap<SkillTimelineTrack, int>();
            _schedulingTracks.Clear();
            _scheduledTracks.Clear();
        }

        public override void Start()
        {
            onStart?.Invoke();
        }

        public override void End()
        {
            onEnd?.Invoke();
        }

        public override void Update(int timeTick)
        {
            if (_scheduleTracks == null || _scheduleTracks.Count <= 0)
                return;

            int curFrame = timeTick - StartTime;

            //帧列表
            if (_scheduleTracks.TryGetValue(curFrame, out var jobList))
            {
                //扔进执行表执行
                foreach (var track in jobList)
                {
                    _schedulingTracks.Add(track);
                    track.Start();
                }
            }

            //持续执行
            if (_schedulingTracks.Count > 0)
            {
                foreach (var track in _schedulingTracks)
                {
                    //每帧执行
                    track.Update(curFrame);

                    //检查结束
                    if (curFrame >= track.EndTime)
                    {
                        _scheduledTracks.Enqueue(track);
                        track.End();
                    }
                }
            }

            //检查是否结束
            while (_scheduledTracks.Count > 0)
            {
                var track = _scheduledTracks.Dequeue();
                _schedulingTracks.Remove(track);
            }
        }
    }

}
