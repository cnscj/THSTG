using System.Collections.Generic;
using XLibrary.Collection;

namespace THGame
{
    public class SkillTimelineSequence : SkillTimelineTrack
    {
        private SortedDictionary<int, HashSet<SkillTimelineTrack>> _scheduleTracks = new SortedDictionary<int, HashSet<SkillTimelineTrack>>();
        private MaxHeap<SkillTimelineTrack, int> _scheduleTracksEndFrame = new MaxHeap<SkillTimelineTrack, int>();
        private HashSet<SkillTimelineTrack> _schedulingTracks = new HashSet<SkillTimelineTrack>();
        private Queue<SkillTimelineTrack> _scheduledTracks = new Queue<SkillTimelineTrack>();

        public SkillTimelineDirector Director;
        public int TotalCount => _scheduleTracks.Count;
        public int ExecuteCount => _schedulingTracks.Count;


        public SkillTimelineSequence(float startTime = 0 ,int durationTime = -1) : base(startTime, durationTime) { }

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

            RefreshExecuteFrame();
        }

        public void ClearTracks()
        {
            _scheduleTracks.Clear();
            _scheduleTracksEndFrame = new MaxHeap<SkillTimelineTrack, int>();
            _schedulingTracks.Clear();
            _scheduledTracks.Clear();

            StartFrame = 0;
            DurationFrame = 1;
        }

        public override void Reset()
        {
            _schedulingTracks.Clear();
            _scheduledTracks.Clear();

            RefreshExecuteFrame();
        }

        public override void Seek(int tickFrame)
        {
            _schedulingTracks.Clear();
            _scheduledTracks.Clear();

            if (_scheduleTracks == null || _scheduleTracks.Count <= 0)
                return;

            if (tickFrame < 0)
                return;

            foreach (var trackSet in _scheduleTracks.Values)
            {
                foreach(var track in trackSet)
                {
                    if (tickFrame > track.StartFrame && tickFrame <= track.EndFrame)
                    {
                        PushTrackInSchedulingList(track);
                    }
                }
            }
        }

        protected void RefreshExecuteFrame()
        {
            DurationFrame = (_scheduleTracksEndFrame.Count > 0 ? _scheduleTracksEndFrame.Max.Key.EndFrame : 0) + 1;
        }

        protected override void OnUpdate(int tickFrame)
        {
            if (_scheduleTracks == null || _scheduleTracks.Count <= 0)
                return;

            if (tickFrame < 0)
                return;

            QueryTracksUpdate(tickFrame);
            ExecuteTracksUpdate(tickFrame);
            PurgeTracksUpdate(tickFrame);
        }

        protected void QueryTracksUpdate(int tickFrame)
        {
            //帧列表
            if (_scheduleTracks.TryGetValue(tickFrame, out var trackList))
            {
                //扔进执行表执行
                foreach (var track in trackList)
                {
                    PushTrackInSchedulingList(track);
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
                    if (tickFrame <= track.EndFrame)
                    {
                        var subTickFrame = tickFrame - track.StartFrame;
                        track.Update(subTickFrame);//每帧执行
                    }

                    //检查结束
                    if (tickFrame >= track.EndFrame)
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
                DequeueTrackInSchedulingList(track);
            }
        }

        private void PushTrackInSchedulingList(SkillTimelineTrack track)
        {
            if (!_schedulingTracks.Contains(track))
            {
                _schedulingTracks.Add(track);
                track.Start(Director);
            }
        }

        private void DequeueTrackInSchedulingList(SkillTimelineTrack track)
        {
            if (_schedulingTracks.Contains(track))
            {
                _schedulingTracks.Remove(track);
                track.End();
            }
        }
    }

}
