
using XLibrary.Collection;

namespace THGame
{
    [System.Serializable]
    public class SkillTimelinePlayable
    {
        public string name;
        public SkillTimelineSequence sequence;

        private readonly SkillTimelineContext _context = new SkillTimelineContext();
        public SerializationDictionary<string, SkillTimelineSequence> dict;
        public int StartFrame
        {
            get
            {
                return (sequence != null) ? sequence.StartFrame : 0;
            }
        }

        public int EndFrame
        {
            get
            {
                return (sequence != null) ? sequence.EndFrame : 0;
            }
        }

        public void Seek(int curFrameCount)
        {
            if (sequence == null)
                return;

            var tickFrame = curFrameCount - sequence.StartFrame;
            sequence.Seek(tickFrame);
        }

        public void Update(int curFrameCount,SkillTimelineDirector director)
        {
            if (sequence == null)
                return;

            var tickFrame = curFrameCount - sequence.StartFrame;

            _context.tick = tickFrame;
            _context.owner = director.gameObject;

            if (tickFrame == sequence.StartFrame)//XXX:首次执行包括起始帧
            {
                sequence.Start(_context);
            }
            sequence.Update(_context);
            if (curFrameCount >= sequence.EndFrame)
            {
                sequence.End(_context);
            }
        }

        public void Reset()
        {
            if (sequence == null)
                return;

            sequence.Reset();
        }
       
    }
}
