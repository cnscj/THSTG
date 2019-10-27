using System;
using System.Collections.Generic;
using UnityEngine;

namespace STGGame.UI
{
    public class TestExampleView : FViewTabBar
    {
        List<ViewParams> layers = new List<ViewParams>()
        {
            new ViewParams(){cls = typeof(TestExampleTextView),title = "文本"},
            new ViewParams(){cls = typeof(TestExampleButtonView),title = "按钮"},
            new ViewParams(){cls = typeof(TestExampleWindowView),title = "窗口"},
        };
        FButton closeBtn;

        public TestExampleView():base("Test", "TestExampleView")
        {
            _layers = layers;
        }

        protected override void OnInitUI()
        {
            closeBtn = GetChild<FButton>("closeBtn");
            closeBtn.OnClick((context) =>
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
