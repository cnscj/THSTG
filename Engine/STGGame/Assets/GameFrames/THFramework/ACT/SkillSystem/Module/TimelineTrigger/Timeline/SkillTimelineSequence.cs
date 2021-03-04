using System.Collections.Generic;
using XLibrary.Collection;

namespace THGame
{
    public class SkillTimelineSequence : SkillTimelineClip
    {
        private SortedDictionary<int, HashSet<SkillTimelineClip>> _scheduleClips;

        private HashSet<SkillTimelineClip> _schedulingClips;
        private Queue<SkillTimelineClip> _scheduledClips;

        private MaxHeap<SkillTimelineClip, int> _scheduleClipsEndFrame;

        public int TotalCount => _scheduleClips != null ?_scheduleClips.Count : 0;
        public int ExecuteCount => _schedulingClips != null ? _schedulingClips.Count : 0;

        public void AddClip(SkillTimelineClip clip)
        {
            if (clip == null)
                return;

            var scheduleClips = GetClipSortDict();
            HashSet<SkillTimelineClip> clipSet;
            if (!scheduleClips.TryGetValue(clip.StartFrame, out clipSet))
            {
                clipSet = new HashSet<SkillTimelineClip>();
                scheduleClips.Add(clip.StartFrame, clipSet);
            }
            clipSet.Add(clip);

            GetLengthHeap().Add(clip, clip.EndFrame);

            RefreshExecuteFrame();
        }

        public void RemoveClip(SkillTimelineClip clip)
        {
            if (clip == null)
                return;

            if (_scheduleClips == null || _scheduleClips.Count <= 0)
                return;

            if (_scheduleClips.TryGetValue(clip.StartFrame, out var clipSet))
            {
                clipSet.Remove(clip);
                if (clipSet.Count <= 0)
                {
                    _scheduleClips.Remove(clip.StartFrame);
                }
            }

            _scheduleClipsEndFrame?.Remove(clip);

            RefreshExecuteFrame();
        }

        public void ClearClips()
        {
            _scheduleClips?.Clear();
            _scheduleClipsEndFrame = null;
            _schedulingClips?.Clear();
            _scheduledClips?.Clear();

            StartFrame = 0;
            DurationFrame = 1;
        }

        public override void Reset()
        {
            _schedulingClips?.Clear();
            _scheduledClips?.Clear();

            RefreshExecuteFrame();
        }

        public override void Seek(int tickFrame)
        {
            _schedulingClips?.Clear();
            _scheduledClips?.Clear();

            if (_scheduleClips == null || _scheduleClips.Count <= 0)
                return;

            if (tickFrame < 0)
                return;

            foreach (var clipSet in _scheduleClips.Values)
            {
                foreach(var clip in clipSet)
                {
                    if (tickFrame > clip.StartFrame && tickFrame <= clip.EndFrame)
                    {
                        PushClipInSchedulingList(clip);
                    }
                }
            }
        }

        public List<SkillTimelineClip> ToList()
        {
            var clipList = new List<SkillTimelineClip>();

            if (_scheduleClips != null)
            {
                foreach (var clipSet in _scheduleClips.Values)
                {
                    foreach (var clip in clipSet)
                    {
                        clipList.Add(clip);
                    }
                }
            }

            return clipList;
        }

        protected void RefreshExecuteFrame()
        {
            DurationFrame = ((_scheduleClipsEndFrame != null &&_scheduleClipsEndFrame.Count > 0) ? _scheduleClipsEndFrame.Max.Key.EndFrame : 0) + 1;
        }

        public override void OnUpdate(int tickFrame)
        {
            if (_scheduleClips == null || _scheduleClips.Count <= 0)
                return;

            if (tickFrame < 0)
                return;

            QueryClipsUpdate(tickFrame);
            ExecuteClipsUpdate(tickFrame);
            PurgeClipsUpdate(tickFrame);
        }

        protected void QueryClipsUpdate(int tickFrame)
        {
            //帧列表
            if (_scheduleClips == null || _scheduleClips.Count <= 0)
                return;

            if (_scheduleClips.TryGetValue(tickFrame, out var clipList))
            {
                //扔进执行表执行
                foreach (var clip in clipList)
                {
                    PushClipInSchedulingList(clip);
                }
            }
        }

        protected void ExecuteClipsUpdate(int tickFrame)
        {
            //持续执行
            if (_schedulingClips == null || _schedulingClips.Count <= 0)
                return;

            foreach (var clip in _schedulingClips)
            {
                if (tickFrame <= clip.EndFrame)
                {
                    var subTickFrame = tickFrame - clip.StartFrame;
                    clip.Update(subTickFrame);//每帧执行
                }

                //检查结束
                if (tickFrame >= clip.EndFrame)
                {
                    GetScheduledClipList().Enqueue(clip); 
                }
            }
            
        }

        protected void PurgeClipsUpdate(int tickFrame)
        {
            //检查是否结束
            if (_scheduledClips == null)
                return;

            while (_scheduledClips.Count > 0)
            {
                var clip = _scheduledClips.Dequeue();
                DequeueClipInSchedulingList(clip);
            }
        }

        private void PushClipInSchedulingList(SkillTimelineClip clip)
        {
            var schedulingClips = GetSchedulingClipList();
            if (!schedulingClips.Contains(clip))
            {
                schedulingClips.Add(clip);
                clip.Start(((SkillTimelineDirector)Owner).gameObject);
            }
        }

        private void DequeueClipInSchedulingList(SkillTimelineClip clip)
        {
            if (_schedulingClips == null || _schedulingClips.Count <= 0)
                return;

            if (_schedulingClips.Contains(clip))
            {
                _schedulingClips.Remove(clip);
                clip.End();
            }
        }

        ///
        private SortedDictionary<int, HashSet<SkillTimelineClip>> GetClipSortDict()
        {
            _scheduleClips = _scheduleClips ?? new SortedDictionary<int, HashSet<SkillTimelineClip>>();
            return _scheduleClips;
        }

        private HashSet<SkillTimelineClip> GetSchedulingClipList()
        {
            _schedulingClips = _schedulingClips ?? new HashSet<SkillTimelineClip>();
            return _schedulingClips;
        }

        private Queue<SkillTimelineClip> GetScheduledClipList()
        {
            _scheduledClips = _scheduledClips ?? new Queue<SkillTimelineClip>();
            return _scheduledClips;
        }

        private MaxHeap<SkillTimelineClip, int> GetLengthHeap()
        {
            _scheduleClipsEndFrame = _scheduleClipsEndFrame ?? new MaxHeap<SkillTimelineClip, int>();
            return _scheduleClipsEndFrame;
        }
    }

}
