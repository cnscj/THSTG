using System;
using UnityEngine;

namespace STGRuntime.UI
{
    public class TestExampleWindowTest2Wnd : FViewWindow
    {
        FButton btn1;
        public TestExampleWindowTest2Wnd():base("Test", "TestExampleWindowTest2Wnd")
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
