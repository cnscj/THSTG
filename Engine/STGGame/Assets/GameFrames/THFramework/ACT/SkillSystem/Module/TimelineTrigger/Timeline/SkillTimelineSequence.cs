using System.Collections.Generic;
using XLibrary.Collection;

namespace THGame
{
    [System.Serializable]
    public class SkillTimelineSequence : SkillTimelineClip
    {
        public SkillTimelineSequence[] sequences;
        private SortedDictionary<int, HashSet<SkillTimelineSequence>> _scheduleSequences;

        private HashSet<SkillTimelineSequence> _schedulingSequences;
        private Queue<SkillTimelineSequence> _scheduledSequences;

        private MaxHeap<SkillTimelineSequence, int> _scheduleSequencesEndFrame;

        public int TotalCount => _scheduleSequences != null ?_scheduleSequences.Count : 0;
        public int ExecuteCount => _schedulingSequences != null ? _schedulingSequences.Count : 0;

        public SkillTimelineSequence()
        {
            type = "_sequence_";
        }

        public void AddSequence(SkillTimelineSequence sequence)
        {
            if (sequence == null)
                return;

            var scheduleSequences = GetSequenceSortDict();
            HashSet<SkillTimelineSequence> sequenceSet;
            if (!scheduleSequences.TryGetValue(sequence.StartFrame, out sequenceSet))
            {
                sequenceSet = new HashSet<SkillTimelineSequence>();
                scheduleSequences.Add(sequence.StartFrame, sequenceSet);
            }
            sequenceSet.Add(sequence);

            GetLengthHeap().Add(sequence, sequence.EndFrame);

            RefreshExecuteFrame();
        }

        public void RemoveSequence(SkillTimelineSequence sequence)
        {
            if (sequence == null)
                return;

            if (_scheduleSequences == null || _scheduleSequences.Count <= 0)
                return;

            if (_scheduleSequences.TryGetValue(sequence.StartFrame, out var sequenceSet))
            {
                sequenceSet.Remove(sequence);
                if (sequenceSet.Count <= 0)
                {
                    _scheduleSequences.Remove(sequence.StartFrame);
                }
            }

            _scheduleSequencesEndFrame?.Remove(sequence);

            RefreshExecuteFrame();
        }

        public void ClearSequences()
        {
            _scheduleSequences?.Clear();
            _schedulingSequences?.Clear();
            _scheduledSequences?.Clear();
            _scheduleSequencesEndFrame = null;

            StartFrame = 0;
            DurationFrame = 1;
        }

        public void Reset()
        {
            _schedulingSequences?.Clear();
            _scheduledSequences?.Clear();

            RefreshExecuteFrame();
        }

        public void Seek(int tickFrame)
        {
            Reset();

            if (_scheduleSequences == null || _scheduleSequences.Count <= 0)
                return;

            if (tickFrame < 0)
                return;

            foreach (var sequenceSet in _scheduleSequences.Values)
            {
                foreach(var sequence in sequenceSet)
                {
                    if (tickFrame > sequence.StartFrame && tickFrame <= sequence.EndFrame)
                    {
                        PushSequenceInSchedulingList(sequence);
                    }
                }
            }
        }

        public List<SkillTimelineSequence> GetSequences()
        {
            var sequenceList = new List<SkillTimelineSequence>();

            if (_scheduleSequences != null)
            {
                foreach (var sequenceSet in _scheduleSequences.Values)
                {
                    foreach (var sequence in sequenceSet)
                    {
                        sequenceList.Add(sequence);
                    }
                }
            }

            return sequenceList;
        }

        public void Refresh()
        {
            this.sequences = GetSequences().ToArray();
        }

        public void SetSequences(List<SkillTimelineSequence> sequences)
        {
            ClearSequences();
            foreach (var sequence in sequences)
            {
                AddSequence(sequence);
            }
        }

        protected void RefreshExecuteFrame()
        {
            DurationFrame = ((_scheduleSequencesEndFrame != null &&_scheduleSequencesEndFrame.Count > 0) ? _scheduleSequencesEndFrame.Max.Key.EndFrame : 0) + 1;
        }

        public override void OnUpdate(int tickFrame)
        {
            if (_scheduleSequences == null || _scheduleSequences.Count <= 0)
                return;

            if (tickFrame < 0)
                return;

            QuerySequencesUpdate(tickFrame);
            ExecuteSequencesUpdate(tickFrame);
            PurgeSequencesUpdate(tickFrame);
        }

        protected void QuerySequencesUpdate(int tickFrame)
        {
            //帧列表
            if (_scheduleSequences == null || _scheduleSequences.Count <= 0)
                return;

            if (_scheduleSequences.TryGetValue(tickFrame, out var sequenceList))
            {
                //扔进执行表执行
                foreach (var sequence in sequenceList)
                {
                    PushSequenceInSchedulingList(sequence);
                }
            }
        }

        protected void ExecuteSequencesUpdate(int tickFrame)
        {
            //持续执行
            if (_schedulingSequences == null || _schedulingSequences.Count <= 0)
                return;

            foreach (var sequence in _schedulingSequences)
            {
                if (tickFrame <= sequence.EndFrame)
                {
                    var subTickFrame = tickFrame - sequence.StartFrame;
                    sequence.Update(subTickFrame);//每帧执行
                }

                //检查结束
                if (tickFrame >= sequence.EndFrame)
                {
                    GetScheduledSequenceList().Enqueue(sequence); 
                }
            }
            
        }

        protected void PurgeSequencesUpdate(int tickFrame)
        {
            //检查是否结束
            if (_scheduledSequences == null)
                return;

            while (_scheduledSequences.Count > 0)
            {
                var sequence = _scheduledSequences.Dequeue();
                DequeueSequenceInSchedulingList(sequence);
            }
        }

        private void PushSequenceInSchedulingList(SkillTimelineSequence sequence)
        {
            var schedulingSequences = GetSchedulingSequenceList();
            if (!schedulingSequences.Contains(sequence))
            {
                schedulingSequences.Add(sequence);
                sequence.Start(((SkillTimelineDirector)Owner).gameObject);
            }
        }

        private void DequeueSequenceInSchedulingList(SkillTimelineSequence sequence)
        {
            if (_schedulingSequences == null || _schedulingSequences.Count <= 0)
                return;

            if (_schedulingSequences.Contains(sequence))
            {
                _schedulingSequences.Remove(sequence);
                sequence.End();
            }
        }

        ///
        private SortedDictionary<int, HashSet<SkillTimelineSequence>> GetSequenceSortDict()
        {
            _scheduleSequences = _scheduleSequences ?? new SortedDictionary<int, HashSet<SkillTimelineSequence>>();
            return _scheduleSequences;
        }

        private HashSet<SkillTimelineSequence> GetSchedulingSequenceList()
        {
            _schedulingSequences = _schedulingSequences ?? new HashSet<SkillTimelineSequence>();
            return _schedulingSequences;
        }

        private Queue<SkillTimelineSequence> GetScheduledSequenceList()
        {
            _scheduledSequences = _scheduledSequences ?? new Queue<SkillTimelineSequence>();
            return _scheduledSequences;
        }

        private MaxHeap<SkillTimelineSequence, int> GetLengthHeap()
        {
            _scheduleSequencesEndFrame = _scheduleSequencesEndFrame ?? new MaxHeap<SkillTimelineSequence, int>();
            return _scheduleSequencesEndFrame;
        }
    }

}
