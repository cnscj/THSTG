﻿using System;
using THGame.UI;
using UnityEngine;

namespace STGRuntime.UI
{
    public class TestExampleWindowTest1Wnd : FViewWindow
    {
        FButton btn1;
        FButton btn2;
        FButton btn3;
        public TestExampleWindowTest1Wnd():base("Test", "TestExampleWindowTest1Wnd")
        {
            
        }
        protected override void OnInitUI()
        {
            btn1 = GetChild<FButton>("btn1");
            btn2 = GetChild<FButton>("n4");
            btn3 = GetChild<FButton>("n5");
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
