using System;
using STGGame.UI;
using UnityEngine;

namespace XSTGGame.UI
{
    public class TestExampleWindowTest1Wnd : FViewWindow
    {
        FButton btn1;
        public TestExampleWindowTest1Wnd():base("Test", "TestExampleWindowTest1Wnd")
        {
            
        }
        protected override void OnInitUI()
        {
            btn1 = GetChild<FButton>("btn1");
            btn1.OnClick((context) =>
            {
                Close();
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
