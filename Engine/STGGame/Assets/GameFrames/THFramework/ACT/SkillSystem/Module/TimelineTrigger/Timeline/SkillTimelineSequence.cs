using System.Collections.Generic;
using XLibrary.Collection;

namespace THGame
{
    public class SkillTimelineSequence : SkillTimelineClip
    {
        private SortedDictionary<int, HashSet<SkillTimelineClip>> _scheduleTracks = new SortedDictionary<int, HashSet<SkillTimelineClip>>();
        private MaxHeap<SkillTimelineClip, int> _scheduleTracksEndFrame = new MaxHeap<SkillTimelineClip, int>();
        private HashSet<SkillTimelineClip> _schedulingTracks = new HashSet<SkillTimelineClip>();
        private Queue<SkillTimelineClip> _scheduledTracks = new Queue<SkillTimelineClip>();

        public int TotalCount => _scheduleTracks.Count;
        public int ExecuteCount => _schedulingTracks.Count;

        public void AddTrack(SkillTimelineClip track)
        {
            if (track == null)
                return;

            HashSet<SkillTimelineClip> trackSet;
            if (!_scheduleTracks.TryGetValue(track.StartFrame, out trackSet))
            {
                trackSet = new HashSet<SkillTimelineClip>();
                _scheduleTracks.Add(track.StartFrame, trackSet);
            }
            trackSet.Add(track);

            _scheduleTracksEndFrame.Add(track, track.EndFrame);

            RefreshExecuteFrame();
        }

        public void RemoveTrack(SkillTimelineClip track)
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
            _scheduleTracksEndFrame = new MaxHeap<SkillTimelineClip, int>();
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

        public override void OnUpdate(int tickFrame)
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

        private void PushTrackInSchedulingList(SkillTimelineClip track)
        {
            if (!_schedulingTracks.Contains(track))
            {
                _schedulingTracks.Add(track);
                track.Start(((SkillTimelineDirector)Owner).gameObject);
            }
        }

        private void DequeueTrackInSchedulingList(SkillTimelineClip track)
        {
            if (_schedulingTracks.Contains(track))
            {
                _schedulingTracks.Remove(track);
                track.End();
            }
        }
    }

}
