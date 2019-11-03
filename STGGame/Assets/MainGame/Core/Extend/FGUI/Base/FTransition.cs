using FairyGUI;

namespace STGGame.UI
{

    public class FTransition : Wrapper<Transition>
    {
        public void Play()
        {
            _obj.Play();
        }

        public void Play(PlayCompleteCallback onComplete)
        {
            _obj.Play(onComplete);
        }

        public void Stop()
        {
            _obj.Stop();
        }

        public bool IsPlaying()
        {
            return _obj.playing;
        }
    }

}
