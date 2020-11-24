using System;

namespace THGame
{
    public class TrackCallback : TimelineTrack
    {

        public Action onStart;
        public Action<int> onUpdate;
        public Action onEnd;

        public TrackCallback(int startTime, int length):base(startTime, length){}

        public override void Start()
        {
            onStart?.Invoke();
        }

        public override void Update(int tickTime)
        {
            int curFrame = tickTime - time;
            onUpdate?.Invoke(curFrame);
        }

        public override void End()
        {
            onEnd?.Invoke();
        }

    }
}