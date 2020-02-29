using FairyGUI;

namespace STGRuntime.UI
{

    public class FTweener : Wrapper<GTweener>
    {
        public void Kill()
        {
            _obj.Kill();
        }

    }

}
