using System;
using UnityEngine;

namespace STGService.UI
{
    public class TestExampleWindowView : FView
    {

        FButton nonModelPopWnd;
        FButton modelPopWnd1;
        FButton modelPopWnd2;

        FButton closeAllBtn;
        FButton close100Btn;

        public TestExampleWindowView():base("Test", "TestExampleWindowView")
        {
            
        }
        protected override void OnInitUI()
        {
            modelPopWnd1 = GetChild<FButton>("modelPopWnd1");
            modelPopWnd2 = GetChild<FButton>("modelPopWnd2");
            nonModelPopWnd = GetChild<FButton>("nonModelPopWnd");
            closeAllBtn = GetChild<FButton>("closeAllBtn");
            close100Btn = GetChild<FButton>("close100Btn");

            modelPopWnd1.OnClick((context) =>
            {
                ViewManager.GetInstance().Open<TestExampleWindowTest1Wnd>();
            });

            modelPopWnd2.OnClick((context) =>
            {
                ViewManager.GetInstance().Open<TestExampleWindowTest2Wnd>();
            });

            
            nonModelPopWnd.OnClick((context) =>
            {
                FView.Create<TestExampleWindowTest1Wnd>(this);

            });


            closeAllBtn.OnClick((context) =>
            {
                ViewManager.GetInstance().CloseAll();
            });
            close100Btn.OnClick((context) =>
            {
                ViewManager.GetInstance().CloseLayer(100);
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
