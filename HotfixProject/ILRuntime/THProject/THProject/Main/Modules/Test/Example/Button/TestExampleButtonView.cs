using System;
using STGGame.UI;
using UnityEngine;

namespace XSTGGame.UI
{
    public class TestExampleButtonView : FView
    {
        FButton btn1;
        public TestExampleButtonView():base("Test", "TestExampleButtonView")
        {
            
        }
        protected override void OnInitUI()
        {
            btn1 = GetChild<FButton>("btn1");
            btn1.OnClick((context) =>
            {
                btn1.SetText("被点击了");
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
