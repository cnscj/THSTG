using System;
using STGGame;
using STGGame.UI;
using UnityEngine;

namespace XSTGGame.UI
{
    public class TestExampleWindowView : FView
    {
        FButton popWnd;
        public TestExampleWindowView():base("Test", "TestExampleWindowView")
        {
            
        }
        protected override void OnInitUI()
        {
            popWnd = GetChild<FButton>("popWnd");
            popWnd.OnClick((context) =>
            {
                ViewManager.GetInstance().Open<TestExampleWindowTest1Wnd>();
            });
        }

        protected override void OnInitEvent()
        {

        }

        protected override void OnEnter()
        {
            
        }

        protected override void OnExit()
        {

        }
    }

}
