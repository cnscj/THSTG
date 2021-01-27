using System.Collections.Generic;
using XLibrary.Collection;

namespace THGame
{
    public class SkillTimelineSequence : SkillTimelineTrack
    {
        private SortedDictionary<int, HashSet<SkillTimelineTrack>> _scheduleTracks = new SortedDictionary<int, HashSet<SkillTimelineTrack>>();
        private MaxHeap<SkillTimelineTrack, int> _scheduleTracksEndFrame = new MaxHeap<SkillTimelineTrack, int>();
        private MinHeap<SkillTimelineTrack, int> _scheduleTracksStartFrame = new MinHeap<SkillTimelineTrack, int>();
        private HashSet<SkillTimelineTrack> _schedulingTracks = new HashSet<SkillTimelineTrack>();
        private Queue<SkillTimelineTrack> _scheduledTracks = new Queue<SkillTimelineTrack>();

        public int TotalCount => _scheduleTracks.Count;
        public int ExecuteCount => _schedulingTracks.Count;

        public SkillTimelineSequence(int start) : base(start, 1) { }
        public SkillTimelineSequence() : base(0, 1) { }

        public void AddTrack(SkillTimelineTrack track)
        {
            if (track == null)
                return;

            HashSet<SkillTimelineTrack> trackSet;
            if (!_scheduleTracks.TryGetValue(track.StartFrame, out trackSet))
            {
                trackSet = new HashSet<SkillTimelineTrack>();
                _scheduleTracks.Add(track.StartFrame, trackSet);
            }
            trackSet.Add(track);

            _scheduleTracksEndFrame.Add(track, track.EndFrame);
            _scheduleTracksStartFrame.Add(track, track.StartFrame);

            RefreshExecuteFrame();
        }

        public void RemoveTrack(SkillTimelineTrack track)
        {
            if (track == null)
                return;

            if (_scheduleTracks.TryGetValue(track.StartFrame, out var trackSet))
            {
                trackSet.Remove(track);
                if (trackSet.Count <= 0)
                {
                    _scheduleTracks.Remove(track.StartFrame);
                }
            }

            _scheduleTracksEndFrame.Remove(track);
            _scheduleTracksStartFrame.Remove(track);

            RefreshExecuteFrame();
        }

        public void Clear()
        {
            _scheduleTracks.Clear();
            _scheduleTracksEndFrame = new MaxHeap<SkillTimelineTrack, int>();
            _scheduleTracksStartFrame = new MinHeap<SkillTimelineTrack, int>();
            _schedulingTracks.Clear();
            _scheduledTracks.Clear();
        }

        public void Play(int frame)
        {
            if (_scheduleTracks == null || _scheduleTracks.Count <= 0)
                return;

            _schedulingTracks.Clear();
            _scheduledTracks.Clear();

            foreach(var trackSet in _scheduleTracks.Values)
            {
                foreach(var track in trackSet)
                {
                    if (frame > track.StartFrame && frame <= track.EndFrame)
                    {
                        if (!_schedulingTracks.Contains(track))
                        {
                            _schedulingTracks.Add(track);
                            track.IsExecuting = true;
                        }
                    }
                }
            }
        }

        protected void RefreshExecuteFrame()
        {
            DurationFrame = (_scheduleTracksEndFrame.Count > 0 ? _scheduleTracksEndFrame.Max.Key.EndFrame : 0) + 1;
        }

        protected override void OnUpdate(int timeTick)
        {
            if (_scheduleTracks == null || _scheduleTracks.Count <= 0)
                return;

            int curFrame = timeTick;
            if (curFrame < 0)
                return;

            QueryTracksUpdate(curFrame);
            ExecuteTracksUpdate(curFrame);
            PurgeTracksUpdate(curFrame);
        }

        protected void QueryTracksUpdate(int tickFrame)
        {
            //帧列表
            if (_scheduleTracks.TryGetValue(tickFrame, out var trackList))
            {
                //扔进执行表执行
                foreach (var track in trackList)
                {
                    if (!_schedulingTracks.Contains(track))
                    {
                        _schedulingTracks.Add(track);
                        track.IsExecuting = true;
                    }
                }
            }
        }

        protected void ExecuteTracksUpdate(int tickFrame)
        {
            //持续执行
            if (_schedulingTracks.Count > 0)
            {
                foreach (var track in _schedulingTracks)
                {
                    var curFrame = tickFrame - track.StartFrame;
                    if (curFrame <= track.EndFrame)
                    {
                        track.Update(curFrame);//每帧执行
                    }

                    //检查结束
                    if (curFrame >= track.EndFrame)
                    {
                        _scheduledTracks.Enqueue(track); 
                    }
                }
            }
        }

        protected void PurgeTracksUpdate(int tickFrame)
        {
            //检查是否结束
            while (_scheduledTracks.Count > 0)
            {
                var track = _scheduledTracks.Dequeue();
                _schedulingTracks.Remove(track);
                track.IsExecuting = false;
            }
        }

    }

}
