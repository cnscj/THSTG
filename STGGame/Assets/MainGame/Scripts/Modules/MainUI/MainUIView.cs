using System.Collections.Generic;

namespace STGGame.UI
{
    public class MainUIView : FView
    {
        MainUIAvatar avatarCom;

        public MainUIView() : base("MainUI", "MainUIView")
        {
            
        }

        protected override void OnInitUI()
        {
            avatarCom = GetChild<MainUIAvatar>("avatarCom");

            avatarCom.SetText("@@@@");
        }

        protected override void OnInitEvent()
        {
            AddEventListener(EventType.TEST_MAIN_UI_UPDATE, _updateLayer);
        }

        protected override void OnEnter()
        {

        }

        protected override void OnExit()
        {

        }

        private void _updateLayer(int e, object args)
        {

        }
    }
}


